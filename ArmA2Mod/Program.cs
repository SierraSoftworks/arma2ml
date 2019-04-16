using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using PXitCore.CommandLineParsing;
using SierraLib.Analytics.Common;
using SierraLib.Analytics.Common.Data;
using SierraLib.Analytics.Common.Helpers;
using System.Net;
using ArmA2Mod.Launcher;

namespace ArmA2Mod
{
    [Application(
        HelpOptions=new string[] {"help","?"},
        OptionPrefixes = new string[] {"-","/"},
        OptionValueSplitters = new char[] {' ','=',':'},
        ThrowParsingExceptions = false)]
    
    [Option(
        "about",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Help",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Display the message detailing information about the application",
        Name = "about",
        Order = 99,
        ShortDescription = "Display the about message",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "engine",
        AllowMultiple = false,
        FormatDisplay = "regex/normal/explicit",
        FormatPattern = "(^regex$|^normal$|^explicit$)",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Specify the search engine to use when looking for mods.",
        Name = "engine",
        Order = 99,
        ShortDescription = "Specify search engine.",
        Value = "regex",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]


    #region "Game Version Selectors"

    [Option(
        "beta",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch the Beta version of ArmA2",
        Name = "beta",
        ShortDescription = "Launch Beta",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "newest",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch the newest version of the game (Beta or Stable)",
        Name = "newest",
        ShortDescription = "Launch the newest game version",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]
    #endregion


    [Option(
        "update",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Check for updates",
        Name = "update",
        ShortDescription = "Check for updates",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MayHaveValue)]

    [Option(
        "copy",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Copies the ArmA 2 startup parameters to the clipboard",
        Name = "copy",
        ShortDescription = "Copy parameters to clipboard",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MayHaveValue)]

    [Option(
        "silent",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Do not display the console's output",
        Name = "silent",
        ShortDescription = "No text output",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "web",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch the Sierra Softworks Project page for ArmA 2 Mod Launcher",
        Name = "web",
        ShortDescription = "Show website",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "y",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Do not prompt for user input",
        Name = "y",
        ShortDescription = "No prompts",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "mock",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Do not launch the ArmA 2 executable but run through the other steps",
        Name = "mock",
        ShortDescription = "Don't launch ArmA 2 but perform all other operations.",
        Value = "",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "debug",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Help",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Display information to assist with debuging problems with the application",
        Name = "debug",
        ShortDescription = "Display debug information",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "nocolour",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Disables coloured text in the console",
        Name = "nocolour",
        ShortDescription = "Disabled coloured text",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "openfolder",
        AllowMultiple = false,
        FormatDisplay = "",
        FormatPattern = "",
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Opens the ArmA 2 folder",
        Name = "openfolder",
        ShortDescription = "Open the ArmA 2 folder",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]


    [Option(
        "mod",
        AllowMultiple = true,
        FormatDisplay = "[RegEx]",
        FormatPattern = "",
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch ArmA2 with the specified mod or mods selected by the regular expression.",
        Name = "mod",
        ShortDescription = "Launch ArmA2 with the specified mod",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option(
        "file",
        AllowMultiple = true,
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "A file that holds a list of mods to launch",
        Name = "file",
        ShortDescription = "Mods list",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option(
        "option",
        AllowMultiple = true,
        FormatDisplay = "[Param]",
        FormatPattern = "",
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch ArmA2 with the specified startup parameter.",
        Name = "option",
        ShortDescription = "Launch ArmA2 with this parameter in the startup",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option(
        "paramsfile",
        AllowMultiple = false,
        Group = "Application Options",
        IsAnonymous = true,
        IsOptional = true,
        LongDescription = "A file that holds a list of command line parameters that should be used",
        Name = "paramsfile",
        ShortDescription = "Parameters list",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]


    #region Operation Arrowhead

    [Option(
        "oa",
        AllowMultiple = false,
        Group = "Operation Arrowhead",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch ArmA 2: Operation Arrowhead",
        Name = "oa",
        ShortDescription = "Operation Arrowhead",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option(
        "co",
        AllowMultiple = false,
        Group = "Operation Arrowhead",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch ArmA 2: Operation Arrowhead Combined Operations",
        Name = "co",
        ShortDescription = "Combined Operations",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option("modfolder",
        AllowMultiple = true,
        Group = "Application Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Adds a folder other than the current game's folder to the list of folders which will be searched for mods",
        Name = "modfolder",
        ShortDescription = "Adds a mod folder to the search location list",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option("ga",
        Group = "Application Options",
        AllowMultiple  = false,
        FormatDisplay = "off|on|full",
        FormatPattern = "off|on|full",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Opt out of Google Analytics tracking for updates by setting this to off",
        Name = "ga",
        ShortDescription = "Opt out of Google Analytics",
        Value = "on",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option("server",
        Group="Launch Options",
        AllowMultiple = false,
        FormatDisplay = "address | address:port",
        FormatPattern = "[0-9]?[0-9]?[0-9](\\.[0-9]?[0-9]?[0-9]){3}(:[0-9]+)?",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Specifies the server address or address:port combination to connect to.",
        Name = "server",
        ShortDescription = "The server to connect to",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option("password",
        AllowMultiple = false,
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "The password for the server you are connecting to (if any).",
        Name = "password",
        ShortDescription = "The password for the server",
        ValuePresence = OptionAttributeValuePresence.MayHaveValue)]

    [Option("serverlist",
        AllowMultiple = false,
        FormatDisplay = "http://address | file path",
        Group = "Server List Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Specifies the address or path to a server list file",
        Name = "serverlist",
        ShortDescription = "The address or path to a server list",
        ValuePresence = OptionAttributeValuePresence.MustHaveValue)]

    [Option("showserverdetails",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Displays the status information from the server that the user specified",
        Group = "Server List Options",
        Name = "showserverdetails",
        ShortDescription = "Show server details",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    [Option("waitforemptyslot",
        IsAnonymous = false,
        IsOptional = true,
        Group = "Server List Options",
        LongDescription = "Causes the application to pospone joining the server until a player spot becomes available",
        ShortDescription = "Waits for a slot to open on the server before starting the game",
        Name = "waitforemptyslot",
        ValuePresence = OptionAttributeValuePresence.MayHaveValue)]

    #endregion

    [Option(
        "steam",
        AllowMultiple = false,
        Group = "Launch Options",
        IsAnonymous = false,
        IsOptional = true,
        LongDescription = "Launch ArmA 2 with the Steam overlay",
        Name = "steam",
        ShortDescription = "Launch ArmA 2 through Steam",
        ValuePresence = OptionAttributeValuePresence.MustNotHaveValue)]

    class Program : CommandLineBase
    {
        static string[] SystemFolders = new string[] { 
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
            "userconfig"};

        static string[] ServerMods = new string[] {
            "Arma 2",
            "Arma 2: Operation Arrowhead",
            "Arma 2: British Armed Forces",
            "Arma 2: British Armed Forces (Lite)",
            "Arma 2: Private Military Company",
            "Arma 2: Private Military Company (Lite)"
        };

        static string AppVersion = "1.8.6.0";


        const string ArmA2BetaPath = "beta";
        const string OABetaPath = "Expansion\\beta";

        [STAThread]
        static void Main(string[] args)
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            AppVersion = Helper.GetAssemblyVersion(assm).ToString();

            new Program(Environment.CommandLine);             
        }
        
       

        static ConsoleColor red = ConsoleColor.Red;
        static ConsoleColor blue = ConsoleColor.Blue;
        static ConsoleColor green = ConsoleColor.Green;
        static ConsoleColor gray = ConsoleColor.Gray;
        static ConsoleColor yellow = ConsoleColor.Yellow;
        static ConsoleColor cyan = ConsoleColor.Cyan;

        public Program(string args)
            : base(args)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
                {
                    if (Options["ga"].Value.Equals("on", StringComparison.InvariantCultureIgnoreCase) || Options["ga"].Value.Equals("full", StringComparison.InvariantCultureIgnoreCase))
                    {

                        ConfigurationSettings.GoogleAccountCode = "UA-9682191-2";

                        GoogleEvent gaEvent = new GoogleEvent(
                            "http://www.sierrasoftworks.com",
                            "Application Crashes",
                            ((Exception)e.ExceptionObject).Message,
                            "ArmA 2 Mod Launcher - " + AppVersion,
                            -1);

                        TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

                        GoogleTracking.FireTrackingEventAsync(tracking);
                    }

                    WriteLine("Unhandled Exception Occured:\n" + ((Exception)e.ExceptionObject).Message,red);
                    WriteLine("Press Any Key To Exit...", red);
                    Console.ReadKey();
                    Environment.FailFast("Unhandled Exception");
                };

            if (Options["paramsfile"].IsPresent)
            {
                if (File.Exists(Options["paramsfile"].Value))
                {
                    List<string> tmp = TextLoader.Process(Options["paramsfile"].Value);
                    string file = "";
                    foreach (string str in tmp)
                        file += str + " ";

                    //EDIT 1.8 - Changed to add args, may override
                    new Program(args.Replace(Options["paramsfile"].Value,"").Replace("\"\"","") + " " + file.Trim());
                    return;
                }
            }

            if (Options["nocolour"].IsPresent)
            {
                Console.ResetColor();
                red = Console.ForegroundColor;
                blue = Console.ForegroundColor;
                green = Console.ForegroundColor;
                gray = Console.ForegroundColor;
                yellow = Console.ForegroundColor;
                cyan = Console.ForegroundColor;
            }

            
            ShowHeader();
            if (ShowingHelp)
            {
                if (!Options["y"].IsPresent)
                {
                    WriteLine("Press Any Key to Exit...", red);
                    Console.ReadKey();
                }
                return;
            }

            

            if (Options["web"].IsPresent)
            {
                Process.Start("http://sierrasoftworks.com/arma2ml");
                return;
            }

            if (Options["update"].IsPresent)
            {
                Update();
                return;
            }

            //NewLogic();
            LibraryLogic();

        }

        private void Update()
        {
            if (Options["update"].IsPresent && !Options["update"].HasValue)
            {

                WriteLine("Checking for Updates", blue);

                bool downloaded = false;

                SierraLib.Settings.Updates.Updates.NewUpdateAvailable += (o, e) =>
                {
                    if (e.NewVersion.ClientVersion > new Version(AppVersion))
                    {
                        WriteLine("There is an update available", green);
                        Write("Your Application Version: ");
                        Write(AppVersion + "\n", yellow);
                        Write("New Application Version: ");
                        Write(e.NewVersion.ClientVersion.ToString() + "\n", green);
                        WriteLine();
                        WriteLine("Description:", blue);
                        WriteLine(e.NewVersion.Description);
                        WriteLine();
                        WriteLine("Change Log:", blue);
                        WriteLine(e.NewVersion.ChangeLog);
                        WriteLine();
                        Write("Would you like to download the update? [y/n]: ", green);
                        if (Options["y"].IsPresent || Console.ReadKey(false).Key == ConsoleKey.Y)
                        {
                            WriteLine();
                            //Download the update

                            string file = Path.Combine(new FileInfo(StartupPath).Directory.FullName, "ArmA2ML_updated.exe");

                            if (File.Exists(file))
                            {
                                if (Options["debug"].IsPresent)
                                    WriteLine("Removing Failed Backup File [ArmA2ML_updated.exe]...");
                                File.Delete(file);
                            }

                            SierraLib.Settings.Updates.Updates.UpdateDownloaded += (o2, e2) =>
                            {
                                WriteLine("Download complete, begining update.");
                                //Start the new update's process and then exit this app
                                Process.Start(file, "-update=\"[1]" + StartupPath + "\"" + ((Options["debug"].IsPresent) ? " -debug" : "" + " -ga=" + Options["ga"].Value));
                                downloaded = true;
                            };


                            WriteLine("\nDownloading Update...");
                            SierraLib.Settings.Updates.Updates.DownloadUpdate(e.NewVersion, new FileInfo(file));
                        }
                        else
                        {
                            //Don't Download the Update
                            WriteLine("You have chosen not to download the update.", red);
                            WriteLine("Press Any Key to Exit...", red);
                            Console.ReadKey();
                            downloaded = true;
                        }
                    }
                    else
                    {
                        WriteLine();
                        WriteLine("No Updates Available", green);

                        Write("Your Application Version: ");
                        Write(AppVersion + "\n", green);
                        Write("Latest Application Version: ");
                        Write(e.NewVersion.ClientVersion.ToString() + "\n", green);

                        WriteLine("Press Any Key To Exit...", green);
                        Console.ReadKey();
                        downloaded = true;
                    }
                };

                SierraLib.Settings.Updates.Updates.GetUpdates();

                if (Options["ga"].Value.Equals("on", StringComparison.InvariantCultureIgnoreCase) || Options["ga"].Value.Equals("full", StringComparison.InvariantCultureIgnoreCase))
                {

                    if (Options["debug"].IsPresent)
                        WriteLine("[Analytics] - Sending Update Started Notification", green);


                    ConfigurationSettings.GoogleAccountCode = "UA-9682191-2";

                    GoogleEvent gaEvent = new GoogleEvent(
                        "http://www.sierrasoftworks.com",
                        "Application Updates",
                        "Update Started",
                        "ArmA 2 Mod Launcher - " + AppVersion,
                        1);

                    TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

                    GoogleTracking.FireTrackingEventAsync(tracking);

                    if (Options["debug"].IsPresent)
                    {
                        WriteLine("[Analytics] - Notification Sent", green);
                    }
                }
                while (!downloaded)
                {
                    Thread.Sleep(1);
                }

                return;
            }

            if (Options["update"].IsPresent && Options["update"].HasValue)
            {
                if (Options["update"].Value.StartsWith("[1]",StringComparison.Ordinal))
                {
                    if (Options["debug"].IsPresent)
                    {
                        WriteLine("[debug] - Startup Parameters:", blue);

                        foreach (IOption option in Options)
                        {
                            if(option.IsPresent)
                                WriteLine("   - " + option.Name + ": " + option.Value);
                        }

                    }


                    FileInfo old = new FileInfo(Options["update"].Value.Remove(0, 3));
                    string tmp = old.Name.Remove(old.Name.Length - old.Extension.Length);

                    if (Options["debug"].IsPresent)
                        WriteLine("Old Application File: " + old.Name);

                    Process[] list = Process.GetProcessesByName(tmp);

                    if (Options["debug"].IsPresent && list.Length > 0)
                        WriteLine("Waiting for old application to exit...");

                    while (list.Length > 0)
                    {
                        list = Process.GetProcessesByName(tmp);
                        Thread.Sleep(10);
                    }

                    if (Options["debug"].IsPresent && File.Exists(Path.Combine(old.Directory.FullName, "ArmA2ML_old.exe")))
                        WriteLine("Removing previously backed up application files...");

                    if (File.Exists(Path.Combine(old.Directory.FullName, "ArmA2ML_old.exe")))
                        File.Delete(Path.Combine(old.Directory.FullName, "ArmA2ML_old.exe"));

                    if (old.Exists)
                    {
                        if (Options["debug"].IsPresent)
                            WriteLine("Making backup of old application");
                        old.MoveTo(Path.Combine(old.Directory.FullName, "ArmA2ML_old.exe"));
                    }

                    old = new FileInfo(Options["update"].Value.Remove(0, 3));


                    FileInfo newFile = new FileInfo(StartupPath);

                    if (Options["debug"].IsPresent)
                        WriteLine("Copying new application to correct path");

                    newFile.CopyTo(old.FullName);

                    if (Options["ga"].Value.Equals("on", StringComparison.InvariantCultureIgnoreCase) || Options["ga"].Value.Equals("full", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(Options["debug"].IsPresent)
                            WriteLine("[Analytics] - Sending Update Complete notification",green);
                        ConfigurationSettings.GoogleAccountCode = "UA-9682191-2";

                        GoogleEvent gaEvent = new GoogleEvent(
                            "http://www.sierrasoftworks.com",
                            "Application Updates",
                            "Update Completed",
                            "ArmA 2 Mod Launcher - " + AppVersion,
                            1);

                        
                        TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

                        GoogleTracking.FireTrackingEventAsync(tracking);
                        if (Options["debug"].IsPresent)
                            WriteLine("[Analytics] - Notification Sent", green);
                    }

                    if (Options["debug"].IsPresent)
                    {
                        WriteLine("Starting cleanup process\nPress any key to continue...");

                        Console.ReadKey();
                    }
                    
                    Process.Start(old.FullName, "-update=\"[2]" + StartupPath + "\"" + ((Options["debug"].IsPresent) ? " -debug" : "") + " -ga=" + Options["ga"].Value);

                    return;
                }
                else if (Options["update"].Value.StartsWith("[2]",StringComparison.Ordinal))
                {
                    if (Options["debug"].IsPresent)
                    {
                        WriteLine("[debug] - Startup Parameters:", blue);

                        foreach (IOption option in Options)
                        {
                            if (option.IsPresent)
                                WriteLine("   - " + option.Name + ": " + option.Value);
                        }

                    }


                    FileInfo old = new FileInfo(Options["update"].Value.Remove(0, 3));
                    string tmp = old.Name.Remove(old.Name.Length - old.Extension.Length);

                    if (Options["debug"].IsPresent)
                        WriteLine("Old Application File: " + old.Name);

                    Process[] list = Process.GetProcessesByName(tmp);

                    if (Options["debug"].IsPresent && list.Length > 0)
                        WriteLine("Waiting for old application to exit...");

                    while (list.Length > 0)
                    {
                        list = Process.GetProcessesByName(tmp);
                        Thread.Sleep(10);
                    }

                    if (old.Exists)
                    {
                        old.Delete();
                    }

                    WriteLine("Application Successfully Updated", green);
                    WriteLine("Press Any Key To Exit...", green);
                    Console.ReadKey();
                    return;
                }
            }
        }

        private void NewLogic()
        {
            bool beta = false;
            bool newest = false;
            bool debug = false;
            bool silent = false;
            bool yes = false;
            bool useRegex = (Options["engine"].Value == "regex");
            bool useExplicit = (Options["engine"].Value == "explicit");
            bool oa = Options["oa"].IsPresent;
            bool co = Options["co"].IsPresent;
            bool steam = Options["steam"].IsPresent;
            string steamParameters = "";

            Console.Title = "ArmA2 Mod Launcher";

            PrintHeader();

            debug = Options["debug"].IsPresent;
            beta = Options["beta"].IsPresent;
            silent = Options["silent"].IsPresent;
            yes = Options["y"].IsPresent;
            newest = Options["newest"].IsPresent;

            SendGAStart(oa,co);

            string arma2dir = "";
            string oadir = "";
            string steampath = "";

            ShowInitialConsoleText(debug,silent);

            if (oa || co)
                oadir = Environment.CurrentDirectory;
            else
                arma2dir = Environment.CurrentDirectory;


            LoadRegistry(ref arma2dir,ref oadir, debug,silent,yes,oa,co);

            if (steam)
            {

                if (!silent && (beta || newest))
                {
                    WriteLine("You cannot launch the beta or newest version of this game using Steam", red);
                    WriteLine("Please run this launcher without the -beta and -newest options when using Steam");
                    if (!yes)
                    {
                        WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                    return;
                }

                //Load steam path from registry (if any)
                steampath = LoadSteamRegistry();
                if (steampath == "")
                {
                    WriteLine("Steam path not present in Registry", red);
                    if (!yes)
                    {
                        WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                    return;
                }

                //Set the steam parameters
                if (oa || co)
                    steamParameters = "-applaunch 33930"; //ArmA 2: Operation Arrowhead
                else
                    steamParameters = "-applaunch 33910"; //ArmA 2
            }

            if (arma2dir == "" && oadir == "")
                WriteLine("ArmA 2 and ArmA 2: OA paths not present in Registry", red);

            if (oadir == "" && OAPresent())
                oadir = Environment.CurrentDirectory;
            

            if (arma2dir == "" && ArmA2Present())
                arma2dir = Environment.CurrentDirectory;

            if (oa && !OAPresent() && !OAPresent(oadir))
            {
                WriteLine("ArmA 2: OA is not present in the current directory", red);
                if (!yes)
                {
                    WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                return;
            }
            else if (co && !OAPresent() && !OAPresent(oadir) && !ArmA2Present() && !ArmA2Present(arma2dir))
            {
                WriteLine("ArmA 2: CO is missing either Operation Arrowhead or ArmA 2", red);
                if (!yes)
                {
                    WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                return;
            }
            else if (!ArmA2Present() && !ArmA2Present(arma2dir))
            {
                WriteLine("ArmA 2 is not present in the current directory", red);
                if (!yes)
                {
                    WriteLine("Press any key to exit...");
                    Console.ReadKey();
                }
                return;
            }

            if (debug && !silent)
            {
                if(arma2dir != "")
                    WriteLine("[debug] - ArmA2 Path: " + arma2dir, blue);
                if(oadir != "")
                    WriteLine("[debug] - ArmA2: OA Path: " + oadir, blue);
                if (steampath != "")
                    WriteLine("[debug] - Steam Path: " + steampath, blue);
                WriteLine();
            }


            if (Options["openfolder"].IsPresent)
                System.Diagnostics.Process.Start("explorer.exe",Environment.CurrentDirectory);

            List<string> optionsList = new List<string>();
            List<string> modsList = new List<string>();
            List<string> missingmodsList = new List<string>();
            List<string> duplicatemodsList = new List<string>();
            List<string> excludeModsList = new List<string>();
            List<string> serverModsList = new List<string>();
            
            //Prevent System Folders from being listed as mods
            excludeModsList.AddRange(SystemFolders);

            List<string> modSearchFolders = new List<string>();
            
            if (oa || co)
                modSearchFolders.Add(oadir);
            else
                modSearchFolders.Add(arma2dir);

            foreach (string folder in Options["modfolder"].Values)
                if (Directory.Exists(folder))
                    modSearchFolders.Add(folder);

            //Add each option to a list for processing.
            foreach (string value in Options["option"].Values)
            {
                if(!optionsList.Contains(value))
                    optionsList.Add(value);
            }

            List<string> modSearches = new List<string>();
            if (Options["file"].IsPresent)
            {
                modSearches.AddRange(TextLoader.Process(Options["file"].Values));
            }

            modSearches.AddRange(Options["mod"].Values);





            if (Options["server"].IsPresent)
            {
                if (debug)
                    WriteLine("[debug] - Contacting server to retrieve information", blue);

                Server.ServerInformation serverInfo = new Server.ServerInformation();

                bool hasInfo = false;
                bool serverUnavailable = false;

                serverInfo.ServerStatusReceived += (o, e) =>
                {
                    hasInfo = true;
                    
                    if (debug)
                        WriteLine("[debug] - Server has returned information", blue);
                };

                serverInfo.ServerInvalid += (o, e) =>
                    {
                        hasInfo = true;
                        if (debug)
                            WriteLine("[debug] - The server did not return any information.", blue);
                    };

                serverInfo.ServerUnavailable += (o, e) =>
                    {
                        if (debug)
                        {
                            WriteLine("[debug] - The application was unable to contact the server.", red);
                            WriteLine("          This may be due to the server being down and will not respond when the user tries to connect", red);
                        }

                        WriteLine("Server not available", red);
                        WriteLine("Press Any Key to Exit...", red);
                        serverUnavailable = true;

                    };

                serverInfo.GetServerStatusAsync(Options["server"].Value);


                while (!hasInfo && !serverUnavailable)
                    Thread.Sleep(1);

                if (serverUnavailable)
                {
                    Console.ReadKey();
                    return;
                }

                //Get the mods
                if(serverInfo.Count > 0 && serverInfo["mod"] != null)
                    serverModsList.AddRange(serverInfo["mod"].Split(';'));

                if (Options["showserverdetails"].IsPresent && serverInfo.Count > 0)
                {
                    WriteLine("Server Info:", blue);
                    WriteLine("  Server Name: " + serverInfo["hostname"]);
                    WriteLine("  Current Players: " + serverInfo["numplayers"] + "/" + serverInfo["maxplayers"]);
                    WriteLine("  Dedicated Server: " + (serverInfo["dedicated"] == "1" ? "Yes" : "No"));
                    WriteLine("  BattleEye: " + (serverInfo["sv_battleye"] == "1" ? "Yes" : "No"));
                    WriteLine("  Password: " + (serverInfo["password"] == "0" ? "No" : "Yes"));
                    WriteLine("  Game Version: " + serverInfo["gamever"]);

                    
                    WriteLine("  Mods:");
                    foreach (string mod in serverModsList)
                    {
                        WriteLine("    " + mod);
                    }

                    WriteLine();
                }
                else if (Options["showserverdetails"].IsPresent)
                {
                    WriteLine("Server did not return any information",red);
                }
            }
            

            //Remove any server game description mods (These can't be searched for
            foreach (string mod in ServerMods)
                if (serverModsList.Contains(mod))
                {
                    if (mod == "Arma 2: Operation Arrowhead" && (!oa || !co) && !OAPresent() && !OAPresent(oadir))
                    {
                        //Can't join an OA server if we aren't running OA or CO
                        if (!silent)
                        {
                            WriteLine("You cannot join an Operation Arrowhead server without running Operation Arrowhead", red);
                            WriteLine("Press Any Key to Exit...", red);
                        }
                        if (!yes)
                            Console.ReadLine();

                        return;
                    }
                        

                    serverModsList.Remove(mod);
                }

            //Process each mod search and select the relevant mod folders.
            foreach (string value in modSearches)
            {
                bool exclude = false;

                string tmp = value;
                if (tmp.StartsWith("!"))
                {
                    exclude = true;
                    tmp = tmp.Remove(0, 1);
                }

                bool added = false;
                if (useRegex)
                {

                    Regex regex = new Regex(tmp);
                    foreach (string modDir in modSearchFolders)
                    {
                        foreach (DirectoryInfo dir in new DirectoryInfo(modDir).GetDirectories())
                        {
                            if (regex.IsMatch(dir.Name))
                            {
                                if (!modsList.Contains(dir.Name) && !exclude)
                                {
                                    added = true;
                                    if (modDir == Environment.CurrentDirectory)
                                        modsList.Add(dir.Name);
                                    else
                                        modsList.Add(dir.FullName);
                                }
                                else
                                {
                                    if (!duplicatemodsList.Contains(dir.Name) && !exclude)
                                        duplicatemodsList.Add(dir.Name);
                                    else if (exclude)
                                        excludeModsList.Add(dir.Name);
                                    added = true;
                                }
                            }
                        }
                    }
                }
                else if (!useExplicit && !useRegex)
                {
                    foreach (string modDir in modSearchFolders)
                    {
                        foreach (DirectoryInfo dir in new DirectoryInfo(modDir).GetDirectories())
                        {
                            if (!modsList.Contains(dir.Name) && !exclude)
                            {
                                added = true;
                                if (modDir == Environment.CurrentDirectory)
                                    modsList.Add(dir.Name);
                                else
                                    modsList.Add(dir.FullName);
                            }
                            else
                            {
                                if (!duplicatemodsList.Contains(dir.Name) && !exclude)
                                    duplicatemodsList.Add(dir.Name);
                                else if (exclude)
                                    excludeModsList.Add(dir.Name);
                                added = true;
                            }
                        }
                    }
                }
                else if(useExplicit)
                {
                    foreach (string modDir in modSearchFolders)
                    {
                        foreach (DirectoryInfo dir in new DirectoryInfo(modDir).GetDirectories(tmp, SearchOption.TopDirectoryOnly))
                        {
                            if (dir.Name.Equals(tmp, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (!modsList.Contains(dir.Name) && !exclude)
                                {
                                    added = true;
                                    if (modDir == Environment.CurrentDirectory)
                                        modsList.Add(dir.Name);
                                    else
                                        modsList.Add(dir.FullName);
                                }
                                else
                                {
                                    if (!duplicatemodsList.Contains(dir.Name) && !exclude)
                                        duplicatemodsList.Add(dir.Name);
                                    else if (exclude)
                                        excludeModsList.Add(dir.Name);
                                    added = true;
                                }
                            }
                        }
                    }
                }

                //If the search did not find any mods then add it to the missing mod list.
                if (!added)
                {
                    missingmodsList.Add(value);
                }

            }


            //Now process the searches for server mods            
            foreach (string tmp in serverModsList)
            {
                bool added = false;

                foreach (string modDir in modSearchFolders)
                {
                    foreach (DirectoryInfo dir in new DirectoryInfo(modDir).GetDirectories())
                    {
                        if (dir.Name.Equals(tmp, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (!modsList.Contains(dir.Name))
                            {
                                added = true;
                                if (modDir == Environment.CurrentDirectory)
                                    modsList.Add(dir.Name);
                                else
                                    modsList.Add(dir.FullName);
                            }
                            else
                            {                                
                                added = true;
                            }
                        }
                    }
                }

                if (!added)
                {
                    missingmodsList.Add(tmp);
                }
            }




            //Now remove all of the mods that should not be included from the mods lists.
            foreach (string exclude in excludeModsList)
            {

                if (modsList.Contains(exclude))
                {
                    modsList.Remove(exclude);
                    if(!excludeModsList.Contains(exclude))
                        excludeModsList.Add(exclude);
                }
                if (missingmodsList.Contains(exclude))
                {
                    missingmodsList.Remove(exclude);
                    if (!excludeModsList.Contains(exclude))
                        excludeModsList.Add(exclude);
                }

                //And do so for ones located outside the game folder
                bool removed = true;
                while (removed)
                {
                    removed = false;
                    foreach (string mod in modsList)
                        if (new DirectoryInfo(mod).Name == exclude)
                        {
                            modsList.Remove(mod);
                            if (!excludeModsList.Contains(exclude))
                                excludeModsList.Add(exclude);
                            removed = true;
                            break;
                        }
                }

                removed = true;
                while (removed)
                {
                    removed = false;
                    foreach (string mod in missingmodsList)
                        if (new DirectoryInfo(mod).Name == exclude)
                        {
                            missingmodsList.Remove(mod);

                            if (!excludeModsList.Contains(exclude))
                                excludeModsList.Add(exclude);
                            removed = true;
                            break;
                        }
                }
            }


            if (newest)
            {
                CheckBeta(ref beta,oa, co,silent, arma2dir, oadir);
            }


            if (beta && !silent)
            {
                WriteLine("Beta Version will be launched");
                WriteLine();
            }

            //Print the options that will be passed to ArmA 2 for the user
            if (optionsList.Count > 0 && !silent)
            {
                WriteLine("Options:", blue);
                foreach (string option in optionsList)
                {
                    WriteLine("   - " + option);
                }
                WriteLine();
            }

            
            //Print the mods that will be loaded for the user
            if (modsList.Count > 0 && !silent)
            {
                WriteLine("Mods:", blue);
                foreach (string option in modsList)
                {
                    WriteLine("   - " + option);
                }
            }
            if(excludeModsList.Count > 0 && !silent)
            {
                foreach (string mod in excludeModsList)
                {
                    if(Array.IndexOf<string>(SystemFolders, mod) == -1)
                        WriteLine("   - " + mod + " [Excluded]", cyan);
                }
            }
            if (duplicatemodsList.Count > 0 && !silent)
            {
                foreach (string mod in duplicatemodsList)
                {
                    WriteLine("   - " + mod + " [Duplicate]",yellow);
                }
            }
            if (missingmodsList.Count > 0 && !silent)
            {
                foreach (string mod in missingmodsList)
                {
                    WriteLine("   - " + mod + " [Missing]", red);
                }
                WriteLine();
            }


            //Generate the startup parameters string
            string parameters = "";
            foreach (string option in optionsList)
                parameters += "-" + option + " ";

            //If a server was specified then add that (and the port)
            //to the launch parameters
            if (Options["server"].IsPresent)
            {
                string[] split = Options["server"].Value.Split(':');
                if(split.Length == 1)
                    parameters += "-server=\"" + split[0] + "\" ";
                else
                    parameters += "-server=\"" + split[0] + "\" -port=" + split[1] + " ";

            }

            //If a password was specified then add that as well
            if (Options["password"].IsPresent)
            {
                if (Options["password"].HasValue)
                    parameters += "-password=" + Options["password"].Value + " ";
                else
                {
                    //Get the password from the user
                    Write("Please enter the password for connecting to the server: ", green);
                    ConsolePasswordInput.ConsolePasswordInput inp = new ConsolePasswordInput.ConsolePasswordInput();
                    string pass = "";
                    inp.PasswordInput(ref pass, 128);
                    inp = null;

                    if (pass.Length > 0)
                        parameters += "-password=" +pass + " ";

                }
            }


            //If there are mods then we should begin adding them
            //otherwise we should just add beta, or CO as needed.
            if (modsList.Count > 0)
            {
                string modParams = "-mod=";


                //If beta is selected then it should always be loaded as if it were a mod
                if (beta && !oa && !co)
                    modParams += ArmA2BetaPath + ";";
                else if (beta)
                    modParams += OABetaPath + ";";

                //Add combined operations paths
                if (co && arma2dir != "" && arma2dir != oadir)
                    modParams += arma2dir + ";Expansion;ca;";
                else if (co && arma2dir != "")
                    modParams += "";

                foreach (string mod in modsList)
                    modParams += mod + ";";



                modParams = modParams.Remove(modParams.Length - 1);

                if (modParams.Contains(" ") & !steam)
                    modParams = "\"" + modParams + "\"";

                parameters += modParams;
            }
            else
            {
                //If beta is selected it should always be loaded as a mod.
                if (beta && !co && !oa)
                {
                    parameters += "-mod=" + ArmA2BetaPath;
                }
                else if (beta && !co)
                {
                    parameters += "-mod=" + OABetaPath;
                }

                else if (beta && co && arma2dir != "" && arma2dir != oadir)
                {
                    parameters += (steam ? "" : "\"") + "-mod=" + OABetaPath + ";" + arma2dir + ";Expansion;ca" + (steam ? "" : "\"");

                }
                else if (beta && co && arma2dir != "")
                {
                    parameters += "-mod=" + OABetaPath;
                }
                else if (!beta && co && arma2dir != oadir)
                {
                    parameters += (steam ? "" : "\"") + "-mod=" + arma2dir + ";Expansion;ca" + (steam ? "" : "\"");
                }
                else if (!beta && co && arma2dir == oadir)
                {
                    //Don't add anything here since the paths seem to be combined
                }
            }
                      
            
            //If debug mode is activated then show the parameters string
            if (debug && !silent)
            {
                WriteLine("[debug] - Launch Parameters:", blue);
                WriteLine(parameters, cyan);
            }


            //If copy mode is active then copy the parameters to the clipboard
            if (Options["copy"].IsPresent)
                SetClipboardText(parameters);

            if (Options["copy"].IsPresent && debug)
                WriteLine("[debug] - Parameters Copied to Clipboard", yellow);

            //Wait for the server to have an open slot if the user has asked for this
            if (Options["server"].IsPresent && Options["waitforemptyslot"].IsPresent)
            {
                Server.ServerInformation serverInfo = new Server.ServerInformation();

                bool available = false;
                bool hasWarnedUser = false;
                bool canConnect = true;

                int refreshTime = 5000;
                if (Options["waitforemptyslot"].HasValue)
                    refreshTime = Convert.ToInt32(Options["waitforemptyslot"].Value);

                serverInfo.ServerInvalid += (o, e) =>
                    {
                        //Server is not available
                        //Continue waiting
                        if (!hasWarnedUser)
                        {
                            Write("The server is unavailable. Would you like to wait for it to become available? [y/n]: ", red);
                            if (yes)
                            {
                                hasWarnedUser = true;
                                Thread.Sleep(refreshTime);
                                serverInfo.GetServerStatusAsync(Options["server"].Value);
                                return;
                            }
                            
                            if (Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                hasWarnedUser = true;
                                Thread.Sleep(refreshTime);
                                serverInfo.GetServerStatusAsync(Options["server"].Value);
                            }
                            else
                            {
                                hasWarnedUser = true;
                                canConnect = false;
                                available = true;
                            }
                        }
                        else
                        {
                            Thread.Sleep(refreshTime);
                            serverInfo.GetServerStatusAsync(Options["server"].Value);
                        }
                    };

                serverInfo.ServerUnavailable += (o, e) =>
                    {
                        //Server is not available
                        //Continue waiting
                        if (!hasWarnedUser)
                        {
                            Write("The server is unavailable. Would you like to wait for it to become available? [y/n]: ", red);
                            if (yes)
                            {
                                hasWarnedUser = true;
                                Thread.Sleep(refreshTime);
                                serverInfo.GetServerStatusAsync(Options["server"].Value);
                                return;
                            }
                            if (Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                hasWarnedUser = true;
                                Thread.Sleep(refreshTime);
                                serverInfo.GetServerStatusAsync(Options["server"].Value);
                            }
                            else
                            {
                                hasWarnedUser = true;
                                canConnect = false;
                                available = true;
                            }
                        }
                        else
                        {
                            Thread.Sleep(refreshTime);
                            serverInfo.GetServerStatusAsync(Options["server"].Value);
                        }
                    };

                serverInfo.ServerStatusReceived += (o, e) =>
                    {
                        if (serverInfo.Count > 0)
                        {
                            int currentPlayers = Convert.ToInt32(serverInfo["numplayers"]);
                            int maxPlayers = Convert.ToInt32(serverInfo["maxplayers"]);

                            if (currentPlayers < maxPlayers)
                            {
                                available = true;
                                canConnect = true;
                            }
                        }
                        else
                        {
                            Thread.Sleep(refreshTime);
                            serverInfo.GetServerStatusAsync(Options["server"].Value);
                        }
                    };

                while (!available)
                {
                    Thread.Sleep(10);
                }

                if (!canConnect)
                {
                    if (!silent)
                    {
                        WriteLine("You have chosen not to continue waiting for the server to become available.");
                        WriteLine("The application will now exit");
                        if (!yes)
                        {
                            WriteLine("Press Any Key to Exit...");
                            Console.ReadKey();
                        }
                    }
                    return;
                }

                //Connect to the server
            }

            if (beta)
            {
                if (
                    (!oa && !co && Directory.Exists(Path.Combine(Environment.CurrentDirectory, ArmA2BetaPath))) ||
                    (Directory.Exists(Path.Combine(Environment.CurrentDirectory, OABetaPath)))
                    )
                {

                    System.Diagnostics.ProcessStartInfo process = null;

                    if (!oa && !co && !steam)
                        process = new System.Diagnostics.ProcessStartInfo(
                            Path.Combine(arma2dir, ArmA2BetaPath + "\\arma2.exe"),
                            parameters);
                    else if (!steam)
                        process = new ProcessStartInfo(
                            Path.Combine(oadir, OABetaPath + "\\arma2oa.exe"),
                            parameters);
                    else if (steam)
                    {
                        //We can't launch the beta from steam :(
                    }

                    if (oa || co) process.WorkingDirectory = oadir;
                    else process.WorkingDirectory = arma2dir;

                    process.UseShellExecute = false;

                    if (debug && !silent)
                    {
                        WriteLine("Executable Path:\n\t" + process.FileName, green);
                        WriteLine("Startup Directory :\n\t" + process.WorkingDirectory, green);
                    }

                    if (!silent)
                    {
                        if (!yes)
                        {
                            WriteLine("Press Any Key to Launch ArmA 2 Beta", green);
                            Console.ReadKey();
                        }
                        if (!Options["mock"].IsPresent)
                        {
                            Process prcs = Process.Start(process);

                            if (debug && !oa && !co)
                            {
                                WriteLine();
                                WriteLine(" [debug] - ArmA2 Started At: " + prcs.StartTime.ToString(), red);
                                prcs.WaitForExit();
                                WriteLine(" [debug] - ArmA2 Exited At: " + prcs.ExitTime.ToString(), red);
                            }

                            else if (debug && oa && !co)
                            {
                                WriteLine();
                                WriteLine(" [debug] - ArmA2: Operation Arrowhead Started At: " + prcs.StartTime.ToString(), red);
                                prcs.WaitForExit();
                                WriteLine(" [debug] - ArmA2: Operation Arrowhead Exited At: " + prcs.ExitTime.ToString(), red);
                            }
                            else if (debug && co)
                            {
                                WriteLine();
                                WriteLine(" [debug] - ArmA2: Combined Operations Started At: " + prcs.StartTime.ToString(), red);
                                prcs.WaitForExit();
                                WriteLine(" [debug] - ArmA2: Combined Operations Exited At: " + prcs.ExitTime.ToString(), red);
                            }
                        }
                    }
                }
                else
                {
                    if (!silent && !yes)
                    {
                        WriteLine("It appears that you do not have the ArmA2 beta installed. Please run this application without the \"-beta\" option.", red);
                        WriteLine("Press Any Key To Exit...");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                if (!silent && !yes && !oa && !co)
                {
                    WriteLine("Press Any Key to Launch ArmA 2", green);
                    Console.ReadKey();
                }

                if (!silent && !yes && oa && !co)
                {
                    WriteLine("Press Any Key to Launch ArmA 2: Operation Arrowhead", green);
                    Console.ReadKey();
                }

                if (!silent && !yes && co)
                {
                    WriteLine("Press Any Key to Launch ArmA 2: Combined Operations", green);
                    Console.ReadKey();
                }

                if (debug && !oa && !co && !Options["mock"].IsPresent & !steam)
                {
                    WriteLine();
                    WriteLine(" [debug] - ArmA2 Started At: " + DateTime.Now.ToString(), red);
                    System.Diagnostics.Process.Start(Path.Combine(arma2dir, "arma2.exe"), parameters).WaitForExit();
                    WriteLine(" [debug] - ArmA2 Exited At: " + DateTime.Now.ToString(), red);
                }

                else if (debug && oa && !co && !Options["mock"].IsPresent & !steam)
                {
                    WriteLine();
                    WriteLine(" [debug] - ArmA2: Operation Arrowhead Started At: " + DateTime.Now.ToString(), red);
                    System.Diagnostics.Process.Start(Path.Combine(oadir, "arma2oa.exe"), parameters).WaitForExit();
                    WriteLine(" [debug] - ArmA2: Operation Arrowhead Exited At: " + DateTime.Now.ToString(), red);
                }
                else if (debug && co && !Options["mock"].IsPresent & !steam)
                {
                    WriteLine();
                    WriteLine(" [debug] - ArmA2: Combined Operations Started At: " + DateTime.Now.ToString(), red);
                    System.Diagnostics.Process.Start(Path.Combine(oadir, "arma2oa.exe"), parameters).WaitForExit();
                    WriteLine(" [debug] - ArmA2: Combined Operations Exited At: " + DateTime.Now.ToString(), red);
                }
                else if (!oa && !co & !steam)
                {
                    if (!Options["mock"].IsPresent)
                        System.Diagnostics.Process.Start(Path.Combine(arma2dir, "arma2.exe"), parameters);
                }
                else if (!oa && !co && !Options["mock"].IsPresent & steam)
                {
                    if (debug && !silent)
                        WriteLine("Launching ArmA 2 using Steam", green);
                    System.Diagnostics.Process.Start(Path.Combine(steampath, "steam.exe"), steamParameters + " \"" + parameters + "\"");                 
                }
                else if (oa && !co && !Options["mock"].IsPresent & steam)
                {
                    if (debug && !silent)
                        WriteLine("Launching ArmA 2: Operation Arrowhead using Steam", green);
                    System.Diagnostics.Process.Start(Path.Combine(steampath, "steam.exe"), steamParameters + " \"" + parameters + "\"");
                }
                else if (co && !Options["mock"].IsPresent & steam)
                {
                    if (debug && !silent)
                        WriteLine("Launching ArmA 2: Combined Operations using Steam", green);
                    System.Diagnostics.Process.Start(Path.Combine(steampath, "steam.exe"), steamParameters + " \"" + parameters + "\"");
                }
                else
                {
                    if (!Options["mock"].IsPresent)
                        System.Diagnostics.Process.Start(Path.Combine(oadir , "arma2oa.exe"), parameters);
                }
            }


        }

        private void LibraryLogic()
        {
            bool beta = false;
            bool newest = false;
            bool debug = false;
            bool silent = false;
            bool yes = false;
            bool useRegex = (Options["engine"].Value == "regex");
            bool useExplicit = (Options["engine"].Value == "explicit");
            bool oa = Options["oa"].IsPresent;
            bool co = Options["co"].IsPresent;
            bool steam = Options["steam"].IsPresent;

            Console.Title = "ArmA2 Mod Launcher";

            PrintHeader();

            debug = Options["debug"].IsPresent;
            beta = Options["beta"].IsPresent;
            silent = Options["silent"].IsPresent;
            yes = Options["y"].IsPresent;
            newest = Options["newest"].IsPresent;

            SendGAStart(oa, co);

            ShowInitialConsoleText(debug, silent);

            LaunchParameters lParams = new LaunchParameters();
            lParams.AdditionalArguments.AddRange(Options["option"].Values);
            lParams.Game = co ? Games.CombinedOperations : (oa ? Games.OperationArrowhead : Games.ArmA2);
            lParams.LaunchType = steam ? LaunchTypes.Steam : (newest ? LaunchTypes.Latest : (beta ? LaunchTypes.Beta : LaunchTypes.Release));

            foreach (var selector in Options["mod"].Values)
            {
                bool exclude = selector[0] == '!';
                string sel = exclude ? selector.Remove(0,1) : selector;
                lParams.ModSelectionFilters.Add(
                    new ModSelector(
                        useExplicit ? SelectionEngines.Exact : (useRegex ? SelectionEngines.RegularExpressions : SelectionEngines.Wildcard),
                        sel, exclude
                        ));
            }

            var result = ArmA2Launcher.StartGame(lParams);

            switch(result.Result)
            {
                case LaunchResults.Success:
                    WriteLine("Game launched successfully", green);
                    break;

                case LaunchResults.Failure:
                    WriteLine("Failed to launch game", red);
                    WriteLine(result.Message, red);
                    break;
            }

            WriteLine("Press any key to exit...");
            Console.ReadKey();

        }

        private string LoadSteamRegistry()
        {
            try
            {
                if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Valve\\Steam"))
                {
                    return RegistryManager.GetValue("HKLM\\SOFTWARE\\Valve\\Steam\\InstallPath").ToString();
                }
                else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam"))
                {
                    return RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Valve\\Steam\\InstallPath").ToString();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private void LoadServerFile(string address, bool silent, bool debug)
        {
            if(File.Exists(address))
            {
                //Local address
            }
            else if(Uri.IsWellFormedUriString(address,UriKind.Absolute))
            {
                if (!silent)
                    WriteLine("Downloading Server Config file");

                //Web address
                WebClient client = new WebClient();
                string file = client.DownloadString(address);

                if (!silent && debug)
                    WriteLine("[debug] - Download complete, begining loading");




            }
        }

        private void LoadRegistry(ref string arma2dir, ref string oadir, bool debug, bool silent, bool yes,bool oa, bool co)
        {
            try
            {
                if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2"))
                {
                    arma2dir = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();
                    if (debug && !silent)
                        WriteLine("\n[debug] - ArmA 2 Path Successfully Loaded From Registry", green);
                }
                else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2"))
                {
                    arma2dir = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2\\Main").ToString();

                    if (debug && !silent)
                        WriteLine("\n[debug] - ArmA 2 Path Successfully Loaded From Registry", green);
                }

                if (IntPtr.Size == 4 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA"))
                {
                    oadir = RegistryManager.GetValue("HKLM\\SOFTWARE\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();
                    if (debug && !silent)
                        WriteLine("\n[debug] - ArmA 2: OA Path Successfully Loaded From Registry", green);
                }
                else if (IntPtr.Size == 8 && RegistryManager.IsPresent("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA"))
                {
                    oadir = RegistryManager.GetValue("HKLM\\SOFTWARE\\Wow6432Node\\Bohemia Interactive Studio\\ArmA 2 OA\\Main").ToString();

                    if (debug && !silent)
                        WriteLine("\n[debug] - ArmA 2: OA Path Successfully Loaded From Registry", green);
                }




                if ((oa || co) && oadir != "")
                {
                    if (Directory.Exists(oadir))
                        Environment.CurrentDirectory = oadir;
                    else
                    {
                        WriteLine("The ArmA 2: OA path specified in the Registry does not exist", red);
                        WriteLine("This could indicate that your game install is corrupted.", red);
                        WriteLine("Please reinstall ArmA 2: OA and make sure that your registry");
                        WriteLine("is set up correctly before attempting to run this launcher again");
                        if (!yes)
                        {
                            WriteLine("Press any key to exit...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(arma2dir))
                        Environment.CurrentDirectory = arma2dir;
                    else
                    {
                        WriteLine("The ArmA 2 path specified in the Registry does not exist", red);
                        WriteLine("This could indicate that your game install is corrupted.", red);
                        WriteLine("Please reinstall ArmA 2 and make sure that your registry");
                        WriteLine("is set up correctly before attempting to run this launcher again");
                        if (!yes)
                        {
                            WriteLine("Press any key to exit...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                    }
                }

                if (co && arma2dir == "" && !ArmA2Present())
                {
                    WriteLine("Unable to start Combined Operations if ArmA 2 is not present.", red);
                    WriteLine("Please make sure that you have correctly installed ArmA 2 and then try agian", red);
                    if (!yes)
                    {
                        WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                    return;
                }

            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    WriteLine("Error occured while loading game paths from the Registry:\n" + ex.Message, red);
                    if (!yes)
                    {
                        WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                    Environment.Exit(0);
                }

            }
        }

        private void ShowInitialConsoleText(bool debug, bool silent)
        {

            if (Options["about"].IsPresent)
            {
                ShowAbout();
                return;
            }

            if (debug && !silent)
            {
                WriteLine("[debug] - Command Line Arguments:", blue);
                foreach (IOption option in Options)
                {
                    if (option.IsPresent && !option.HasValue)
                    {
                        WriteLine("   - " + option.Name, gray);
                    }
                    else if (option.IsPresent && option.HasValue)
                    {
                        if (option.Values.Length == 1)
                            WriteLine("   - " + option.Name + ": " + option.Value, gray);
                        else
                        {
                            string tmpVal = "";
                            foreach (string val in option.Values)
                            {
                                tmpVal += val + "; ";
                            }
                            tmpVal = tmpVal.Remove(tmpVal.Length - 1);


                            WriteLine("   - " + option.Name + ": " + tmpVal, gray);
                        }
                    }
                }
            }

        }

        private void CheckBeta(ref bool beta, bool oa, bool co, bool silent, string arma2dir,string oadir)
        {

            if (!silent && !oa && !co)
            {
                WriteLine("ArmA 2 - Stable Version: " + GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, "arma2.exe")), green);
                try
                {
                    WriteLine("ArmA 2 - Beta Version: " + GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, ArmA2BetaPath + "\\arma2.exe")), green);
                }
                catch
                {
                    WriteLine("ArmA 2 - Beta Version not present", red);
                }
            }
            else if (!silent && (oa || co))
            {
                WriteLine("ArmA 2: OA - Stable Version: " + GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, "arma2oa.exe")), green);
                try
                {
                    WriteLine("ArmA 2: OA - Beta Version: " + GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, OABetaPath + "\\arma2oa.exe")), green);
                }
                catch
                {
                    WriteLine("ArmA 2: OA - Beta version not present", red);
                }
            }

            if (!oa && !co)
            {
                try
                {
                    if (GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory,  ArmA2BetaPath + "\\arma2.exe")) > GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, "arma2.exe")))
                        beta = true;
                    else
                        beta = false;
                }
                catch { beta = false; }
            }
            else
            {
                try
                {
                    if (GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, OABetaPath + "\\arma2oa.exe")) > GetAssemblyVersion(Path.Combine(Environment.CurrentDirectory, "arma2oa.exe")))
                        beta = true;
                    else
                        beta = false;

                }
                catch { beta = false; }
            }
        }

        private void SendGAStart(bool oa, bool co)
        {

            if (Options["ga"].Value.Equals("full", StringComparison.InvariantCultureIgnoreCase))
            {

                ConfigurationSettings.GoogleAccountCode = "UA-9682191-2";

                GoogleEvent gaEvent = new GoogleEvent(
                    "http://www.sierrasoftworks.com",
                    "Application Launches",
                    (oa ? "ArmA 2: Operation Arrowhead" : (co ? "ArmA 2: Combined Operations" : "ArmA 2")),
                    "ArmA 2 Mod Launcher - " + AppVersion,
                    1);

                TrackingRequest tracking = new RequestFactory().BuildRequest(gaEvent);

                GoogleTracking.FireTrackingEventAsync(tracking);
            }
        }

        private static bool OAPresent()
        {
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*.exe"))
                if (new FileInfo(file).Name.Equals("arma2oa.exe",StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
        private static bool OAPresent(string dir)
        {
            if (!Directory.Exists(dir))
                return false;
            
            foreach (string file in Directory.GetFiles(dir, "*.exe"))
                if (new FileInfo(file).Name.Equals("arma2oa.exe", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
        private static bool ArmA2Present()
        {
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*.exe"))
                if (new FileInfo(file).Name.Equals("arma2.exe",StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }
        private static bool ArmA2Present(string dir)
        {
            if (!Directory.Exists(dir))
                return false;
            foreach (string file in Directory.GetFiles(dir, "*.exe"))
                if (new FileInfo(file).Name.Equals("arma2.exe", StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        private static bool IsDLCPresent(string dlcName, string oadir)
        {
            if (OAPresent())
            {
                foreach (DirectoryInfo dir in new DirectoryInfo(Environment.CurrentDirectory).GetDirectories())
                {
                    if (dir.Name.Equals(dlcName))
                        return true;
                }
            }
            else
            {
                //We need to OA path
                foreach (DirectoryInfo dir in new DirectoryInfo(oadir).GetDirectories())
                {
                    if (dir.Name.Equals(dlcName))
                        return true;
                }
            }

            return false;
        }

        private static List<string> GetModList(List<string> RegExList)
        {
            DirectoryInfo[] modFolders = new DirectoryInfo(Environment.CurrentDirectory).GetDirectories();

            List<string> mods = new List<string>();

            foreach (string regex in RegExList)
            {
                Regex regEx = new Regex(regex, RegexOptions.Compiled);

                foreach (DirectoryInfo modFolder in modFolders)
                    if (regEx.IsMatch(modFolder.Name))
                        mods.Add(modFolder.Name);
            }

            return mods;
        }

        private static List<string> GetRegExList(string[] parameters)
        {
            List<string> mods = new List<string>();

            for (int i = 0; i < parameters.Length; i ++)
            {
                if (parameters[i].StartsWith("-mod") || parameters[i].StartsWith("/mod"))
                {
                    mods.Add(parameters[i].Split('=')[1]);
                }
            }

            return mods;
        }

        private static List<string> GetOptionsList(string[] parameters)
        {
            List<string> mods = new List<string>();

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].StartsWith("-option") || parameters[i].StartsWith("/option"))
                {
                    mods.Add(parameters[i].Split('=')[1]);
                }
            }

            return mods;
        }

        public override void ShowHelp()
        {
            base.ShowHelp();

            WriteLine();
            WriteLine("NOTE:", red);
            WriteLine("The application will automatically detect duplicate modifications, as well as missing mods.");
            WriteLine("If a duplicate mod is detected it will be highlighted in the Mods list in Yellow");
            WriteLine("If a missing mod is detected it will be highlighted in the Mods list in Red");
            WriteLine();
            WriteLine("You can exclude a mod from an inclusion filter by placing a \"!\" in front of its search");
            WriteLine("For example, you can select all mods except for ones with the word ACE:");
            WriteLine("  -mod=@ -mod=!ACE");
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = blue;
            Console.WriteLine(
                "ArmA 2 Mod Launcher - Version " + AppVersion
                );
            Console.WriteLine();
        }

        static void ShowAbout()
        {
            Console.ForegroundColor = gray;
            Console.WriteLine(
                "This application will allow you to launch ArmA 2 with a predefined set of modifications and options.\n" +
                "Its advantage is that it will allow you to specify modifications based on Regular Expressions which can vastly reduce the amount of text that has to be typed when selecting mods.\n" +
                "For example, if you want to load ArmA 2 with all the modifications that you have added simply add this to the command line:\n" +
                "    -mod=@.*\n" +
                "    (Provided that all of your mod folders have @ at the begining of their names)\n" +
                "\n" +
                "To launch Operation Arrowhead you can add -oa to the parameters list. For example:\n" +
                "    -oa\n" +
                "\n" +
                "To launch Combined Operations you can add -co to the parameters list. For example:\n" +
                "    -co\n" +
                "\n" +
                "This application can be run from any location, it will automatically detect the ArmA 2 directory and run appropriately.\n" +
                "\n" +
                "Some Examples:\n" +
                "   ACE:\n" +
                "      -option=noSplash -option=noFilePatching -option=showScriptingErrors -mod=@ACE.* -mod=CBA\n" +
                "   HiFi:\n" +
                "      -option=noSplash -mod=@HiFi.* -mod=CBA\n" +
                "\n" +
                "Good Luck and Happy Gaming"
                );
            Console.ReadKey();
        }

        static Version GetAssemblyVersion(string file)
        {
            FileVersionInfo fi = FileVersionInfo.GetVersionInfo(file);
            
            return new Version(fi.ProductVersion);
        }

        static void SetClipboardText(string text)
        {
            if (OpenClipboard(IntPtr.Zero))
            {
                EmptyClipboard();

                IntPtr ptr = Marshal.AllocHGlobal(Encoding.Default.GetByteCount(text) + 1);

                Marshal.Copy(Encoding.Default.GetBytes(text), 0, ptr, Encoding.Default.GetByteCount(text));

                SetClipboardData(1,ptr);

                CloseClipboard();
            }

        }

        [DllImport("User32.dll")]
        static extern bool OpenClipboard(IntPtr owner);

        [DllImport("User32.dll")]
        static extern bool EmptyClipboard();

        [DllImport("User32.dll")]
        static extern bool SetClipboardData(uint format, IntPtr memLocation);

        [DllImport("User32.dll")]
        static extern bool CloseClipboard();

    }
}
