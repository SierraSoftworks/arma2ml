using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ArmA2Mod.Launcher
{
    public class ArmA2Launcher
    {
        /// <summary>
        /// Triggered after a call to <see cref="StartGameAsync"/> completes
        /// </summary>
        public event EventHandler<LaunchEventArgs> StartGameCompleted = null;

        /// <summary>
        /// Starts the game specified in the <paramref name="parameters"/> 
        /// and waits for it to exit before returning
        /// </summary>
        /// <param name="parameters">
        /// The options to use for launching the game
        /// </param>
        /// <returns>
        /// The result of the call to this method
        /// </returns>
        public static LaunchEventArgs StartGame(LaunchParameters parameters)
        {
            return StartGameInternal(parameters);
        }

        /// <summary>
        /// Starts the game specified in the <paramref name="parameters"/>
        /// asynchronously, returning immediately.
        /// </summary>
        /// <param name="parameters">
        /// The options to use for launching the game
        /// </param>
        public void StartGameAsync(LaunchParameters parameters)
        {
            ThreadPool.QueueUserWorkItem(AsyncLaunchThread, parameters);
        }

        #region Asynchronous Launches

        private void AsyncLaunchThread(object context)
        {
            LaunchParameters parameters = context as LaunchParameters;

            var result = StartGameInternal(parameters);

            if (StartGameCompleted != null)
                StartGameCompleted(this, result);
        }

        #endregion


        #region Synchronous Launches

        private static LaunchEventArgs StartGameInternal(LaunchParameters parameters)
        {
            if (parameters.LaunchType == LaunchTypes.Release && !IsGamePresent(parameters.Game))
                return LaunchEventArgs.ForFailure(parameters, "Game executable could not be located, please ensure that the game is installed");

            LaunchTypes equivalentLaunch = GetEquivalentLaunch(parameters.Game, parameters.LaunchType);

            ProcessStartInfo startInfo = GetBaseStartInfo(parameters.Game, equivalentLaunch);

            string modArguments = GetBaseModArguments(parameters.Game, equivalentLaunch);

            List<string> searchDirectories = new List<string>();
            switch (parameters.Game)
            {
                case Games.ArmA2:
                    searchDirectories.Add(ArmA2Directory);
                    break;
                case Games.OperationArrowhead:
                case Games.CombinedOperations:
                    searchDirectories.Add(OperationArrowheadDirectory);
                    break;
            }
            searchDirectories.AddRange(parameters.AdditionalModDirectories);


            List<string> selectedMods = new List<string>();
            List<string> excludedMods = new List<string>();
            List<ModSelector> failedSelections = new List<ModSelector>();
            failedSelections.AddRange(parameters.ModSelectionFilters);

            foreach (string searchPath in searchDirectories)
            {
                IEnumerable<string> childFolders = new DirectoryInfo(searchPath).GetDirectories().Select(_ => _.Name);

                foreach (ModSelector selector in parameters.ModSelectionFilters)
                {
                    var result = selector.Match(childFolders);

                    if (result != null && result.Count() > 0)
                        failedSelections.Remove(selector);

                    foreach (string selectedMod in result)
                        if (!selector.ExclusionFilter && !selectedMods.Contains(selectedMod))
                            selectedMods.Add(selectedMod);
                        else if (selector.ExclusionFilter && !excludedMods.Contains(selectedMod))
                            excludedMods.Add(selectedMod);
                }
            }

            foreach (string exclude in excludedMods)
                if (selectedMods.Contains(exclude))
                    selectedMods.Remove(exclude);

            if (failedSelections.Count > 0)
                return LaunchEventArgs.ForMissingMods(parameters, "Could not find some of the mods that were specified", selectedMods, excludedMods, failedSelections);

            foreach (string mod in selectedMods)
                modArguments += mod + ";";

            if (modArguments.Length > 0)
            {
                modArguments = modArguments.Remove(modArguments.Length - 1);

                //Now append the -mod to the beginning and enclose in quotation marks if necessary
                modArguments = "-mod=" + modArguments;

                if (equivalentLaunch == LaunchTypes.Steam || modArguments.Contains(' '))
                    modArguments = "\"" + modArguments + "\"";

                startInfo.Arguments += modArguments;
            }

            DateTime startTime = DateTime.Now;
            Process gameInstance = Process.Start(startInfo);
            gameInstance.WaitForExit();
            DateTime closeTime = DateTime.Now;

            return LaunchEventArgs.ForSuccess(parameters, startTime, closeTime, selectedMods, excludedMods);
        }

        #endregion

        #region Constants

        private static readonly string[] CoreGameFolders = new string[]
        {
            "AddOns",
            "BAF",
            "BattlEye",
            "BESetup",
            "beta",
            "Campaigns",
            "Common",
            "DirectX",
            "DLCsetup",
            "Dta",
            "Expansion",
            "Keys",
            "Missions",
            "MPMissions",
            "PMC",
            "userconfig"
        };
        
        #endregion

        #region Paths

        private static string __steamDirectory = null;
        public static string SteamDirectory
        {
            get
            {
                if (__steamDirectory == null)
                {                   
                    try
                    {
                        if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Valve\\Steam"))
                        {
                            __steamDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Valve\\Steam\\InstallPath").ToString();
                        }
                        else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam"))
                        {
                            __steamDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam\\InstallPath").ToString();
                        }
                    }
                    catch
                    {   }
                }

                return __steamDirectory;
            }
        }

        private static string __arma2Directory = null;
        public static string ArmA2Directory
        {
            get
            {
                if (__arma2Directory == null)
                {
                    if (IsGamePresent(Games.ArmA2, Environment.CurrentDirectory))
                        __arma2Directory = Environment.CurrentDirectory;

                    if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2"))
                    {
                        __arma2Directory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();
                    }
                    else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2"))
                    {
                        __arma2Directory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();
                    }
                }
                return __arma2Directory;
            }
        }

        private static string __oaDirectory = null;
        public static string OperationArrowheadDirectory
        {
            get
            {
                if (__oaDirectory == null)
                {
                    if (IsGamePresent(Games.OperationArrowhead, Environment.CurrentDirectory))
                        __oaDirectory = Environment.CurrentDirectory;

                    if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA"))
                    {
                        __oaDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();
                    }
                    else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA"))
                    {
                        __oaDirectory = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();
                    }
                }
                return __oaDirectory;
            }
        }

        #endregion

        #region Executables
        
        private static bool IsGamePresent(Games game)
        {
            if (game == Games.ArmA2)
            {
                if (IsGamePresent(game, Environment.CurrentDirectory))
                    return true;

                return IsGamePresent(game, ArmA2Directory);
                    
            }
            else if (game == Games.OperationArrowhead)
            {
                if (IsGamePresent(game, Environment.CurrentDirectory))
                    return true;

                return IsGamePresent(game, OperationArrowheadDirectory);
            }
            else if (game == Games.CombinedOperations)
            {
                return IsGamePresent(Games.ArmA2) && IsGamePresent(Games.OperationArrowhead);
            }
            return false;
        }

        private static bool IsGamePresent(Games game, string dir)
        {
            if (!Directory.Exists(dir))
                return false;

            if (game == Games.ArmA2)
            {
                if (!Directory.Exists(dir))
                    return false;
                foreach (string file in Directory.GetFiles(dir, "*.exe"))
                    if (new FileInfo(file).Name.Equals("arma2.exe", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                return false;
            }
            else if (game == Games.OperationArrowhead)
            {
                foreach (string file in Directory.GetFiles(dir, "*.exe"))
                    if (new FileInfo(file).Name.Equals("arma2oa.exe", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                return false;
            }
            else if (game == Games.CombinedOperations)
                throw new InvalidOperationException("Cannot call this method to test a specific directory for Combined Operations");
            
            return false;
        }

        private static Version GetAssemblyVersion(string file)
        {
            FileVersionInfo fi = FileVersionInfo.GetVersionInfo(file);

            return new Version(fi.ProductVersion);
        }

        private static LaunchTypes GetEquivalentLaunch(Games game, LaunchTypes launch)
        {
            if (launch != LaunchTypes.Latest)
                return launch;

            Version releaseVersion = null;
            Version betaVersion = null;

            switch (game)
            {
                case Games.ArmA2:
                    releaseVersion = GetAssemblyVersion(Path.Combine(ArmA2Directory, "arma2.exe"));
                    betaVersion = GetAssemblyVersion(Path.Combine(ArmA2Directory, "beta", "arma2.exe"));
                    break;
                case Games.OperationArrowhead:
                case Games.CombinedOperations:
                    releaseVersion = GetAssemblyVersion(Path.Combine(OperationArrowheadDirectory, "arma2oa.exe"));
                    betaVersion = GetAssemblyVersion(Path.Combine(OperationArrowheadDirectory, "Expansion", "beta", "arma2oa.exe"));
                    break;
            }

            if (releaseVersion == null || betaVersion == null || releaseVersion >= betaVersion)
                return LaunchTypes.Release;
            else
                return LaunchTypes.Beta;

        }

        private static ProcessStartInfo GetBaseStartInfo(Games game, LaunchTypes launch)
        {
            Contract.Requires<ArgumentException>(launch != LaunchTypes.Latest, "Function requires equivalent launch type to be computed first, cannot recieve 'Latest' as an option");

            ProcessStartInfo startInfo = null;

            switch (game)
            {
                case Games.ArmA2:
                    if (launch == LaunchTypes.Release)
                        startInfo = new ProcessStartInfo(Path.Combine(ArmA2Directory, "arma2.exe"));
                    else if (launch == LaunchTypes.Beta)
                        startInfo = new ProcessStartInfo(Path.Combine(ArmA2Directory, "beta", "arma2.exe"));
                    else if (launch == LaunchTypes.Steam)
                        startInfo = new ProcessStartInfo(Path.Combine(SteamDirectory, "steam.exe"), "-applaunch 33910 ");

                    startInfo.WorkingDirectory = ArmA2Directory;
                    startInfo.UseShellExecute = true;

                    break;
                case Games.OperationArrowhead:
                case Games.CombinedOperations:
                    if (launch == LaunchTypes.Release)
                        startInfo = new ProcessStartInfo(Path.Combine(OperationArrowheadDirectory, "arma2oa.exe"));
                    else if (launch == LaunchTypes.Beta)
                        startInfo = new ProcessStartInfo(Path.Combine(OperationArrowheadDirectory, "Expansion", "beta", "arma2oa.exe"));
                    else if (launch == LaunchTypes.Steam)
                        startInfo = new ProcessStartInfo(Path.Combine(SteamDirectory, "steam.exe"), "-applaunch 33930 ");

                    startInfo.WorkingDirectory = OperationArrowheadDirectory;
                    startInfo.UseShellExecute = false;

                    break;
                default:
                    startInfo = null;
                    break;
            }

            return startInfo;
        }

        #endregion

        #region Generators

        private static string GetAdditionalArguments(List<string> additionalArgs)
        {
            Contract.Requires<ArgumentNullException>(additionalArgs != null);

            string values = "";
            foreach (string arg in additionalArgs)
                values += arg + " ";
            return values;
        }

        private static string GetBaseModArguments(Games game, LaunchTypes launch)
        {
            Contract.Requires<ArgumentException>(launch != LaunchTypes.Latest, "Function requires equivalent launch type to be computed first, cannot recieve 'Latest' as an option");

            string args = "";
            switch (game)
            {
                case Games.ArmA2:
                    if (launch == LaunchTypes.Beta)
                        args += "beta;";
                    break;

                case Games.OperationArrowhead:
                    if (launch == LaunchTypes.Beta)
                        args += "Expansion/beta;";
                    break;

                case Games.CombinedOperations:

                    if (launch == LaunchTypes.Beta)
                        args += "Expansion/beta;";

                    if (ArmA2Directory != OperationArrowheadDirectory)
                        args += ArmA2Directory + ";";

                    args += "Expansion;ca;";
                    break;
            }

            return args;
        }
        
        #endregion
    }
}
