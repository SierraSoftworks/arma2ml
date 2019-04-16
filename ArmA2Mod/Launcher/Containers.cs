using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace ArmA2Mod
{
    public enum LaunchResults
    {
        Success,
        Failure
    }

    public enum Games
    {
        ArmA2 = 0,
        OperationArrowhead = 1,
        CombinedOperations = 2
    }

    public enum LaunchTypes
    {
        Steam,
        Release,
        Beta,
        Latest
    }

    public enum SelectionEngines
    {
        Exact,
        Wildcard,
        RegularExpressions
    }

    public class LaunchParameters
    {
        public LaunchParameters()
        {
            Game = Games.ArmA2;
            LaunchType = LaunchTypes.Release;
            ModSelectionFilters = new List<ModSelector>();
            AdditionalArguments = new List<string>();
            AdditionalModDirectories = new List<string>();
            Server = GameServer.Singleplayer;
        }

        public Games Game
        {
            get;
            set;
        }

        public LaunchTypes LaunchType
        { get; set; }

        public List<ModSelector> ModSelectionFilters
        {
            get;
            set;
        }

        public List<string> AdditionalArguments
        {
            get;
            set;
        }

        public List<string> AdditionalModDirectories
        { get; set; }

        public GameServer Server
        { get; set; }
    }



    public class ModSelector
    {
        public ModSelector(SelectionEngines engine, string selector, bool exclusion)
        {
            Engine = engine;
            Selector = selector;
            ExclusionFilter = exclusion;
        }

        public SelectionEngines Engine
        {
            get;
            private set;
        }

        public string Selector
        { get; private set; }

        public bool ExclusionFilter
        { get; set; }

        internal IEnumerable<string> Match(IEnumerable<string> values)
        {
            if(Engine == SelectionEngines.RegularExpressions)
            {
                Regex regex = new Regex(Selector, RegexOptions.Compiled);
                return values.Where(_ => regex.IsMatch(_));
            }
            else if (Engine == SelectionEngines.Exact)
            {
                return values.Where(_ => _.Equals(Selector, StringComparison.InvariantCultureIgnoreCase));
            }
            else if (Engine == SelectionEngines.Wildcard)
            {
                string[] parts = Selector.Split(new string[] {"*"}, StringSplitOptions.None);
                string regexSelector = "";
                for (int i = 0; i < parts.Length; i++)
                    regexSelector += Regex.Escape(parts[i]) + ".*";
                regexSelector.Remove(regexSelector.Length - 2);

                Regex regex = new Regex(regexSelector, RegexOptions.Compiled);
                return values.Where(_ => regex.IsMatch(_));
            }

            return null;
        }
                
    }

    public struct GameServer
    {
        public static GameServer Singleplayer
        {
            get
            {
                return new GameServer() { Address = ".", Port = 0, Password = "" };
            }
        }

        public string Address
        { get; set; }

        public int Port
        { get; set; }

        public string Password
        { get; set; }
    }

    public class LaunchEventArgs : EventArgs
    {
        private LaunchEventArgs(LaunchParameters parameters, LaunchResults result, string message, List<string> selectedMods, List<string> excludedMods, List<ModSelector> failedSelectors)
        {
            Parameters = parameters;
            Result = result;
            Message = message;
            SelectedMods = selectedMods;
            ExcludedMods = excludedMods;
            FailedSelectors = failedSelectors;
        }

        private LaunchEventArgs(LaunchParameters parameters, LaunchResults result, string message, DateTime launched, DateTime closed, List<string> selectedMods, List<string> excludedMods)
        {
            Parameters = parameters;
            Result = result;
            Message = message;
            LaunchTime = launched;
            CloseTime = closed;
            SelectedMods = selectedMods;
            ExcludedMods = excludedMods;
            FailedSelectors = new List<ModSelector>();
        }

        public static LaunchEventArgs ForFailure(LaunchParameters parameters, string errorMessage)
        {
            return new LaunchEventArgs(parameters, LaunchResults.Failure, errorMessage, new List<string>(), new List<string>(), new List<ModSelector>());
        }

        public static LaunchEventArgs ForMissingMods(LaunchParameters parameters, string errorMessage, List<string> selectedMods, List<string> excludedMods, List<ModSelector> failedSelections)
        {
            return new LaunchEventArgs(parameters, LaunchResults.Failure, errorMessage, selectedMods, excludedMods, failedSelections);
        }

        public static LaunchEventArgs ForSuccess(LaunchParameters parameters, DateTime launched, DateTime closed, List<string> selectedMods, List<string> excludedMods)
        {
            return new LaunchEventArgs(parameters, LaunchResults.Success, "Game was successfully launched", launched, closed, selectedMods, excludedMods);
        }

        public LaunchResults Result
        {
            get;
            private set;
        }

        public DateTime LaunchTime
        { get; private set; }

        public DateTime CloseTime
        { get; private set; }

        public List<string> SelectedMods
        { get; private set; }

        public List<string> ExcludedMods
        { get; private set; }

        public List<ModSelector> FailedSelectors
        { get; private set; }

        public string Message
        {
            get;
            private set;
        }

        public LaunchParameters Parameters
        {
            get;
            private set;
        }
    }
}
