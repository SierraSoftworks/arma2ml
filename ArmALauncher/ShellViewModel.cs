namespace ArmALauncher {
    using System.ComponentModel.Composition;
    using System.Collections.ObjectModel;
    using BISLib;
    using System.Windows;
    using Caliburn.Micro;
    using System.Collections.Generic;

    [Export(typeof(IShell))]    
    public class ShellViewModel : Caliburn.Micro.PropertyChangedBase,  IShell 
    {

        public ShellViewModel()
        {
            PinnedLaunchers = new ObservableCollection<PinnedConfiguration>();
            
            PinnedLaunchers.Add(new PinnedConfiguration("ArmA 2 (Steam)", new LaunchParameters()
            {
                Game = Game.ArmA2,
                LaunchType = LaunchTypes.Steam
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("ArmA 2 (Retail)", new LaunchParameters()
            {
                Game = Game.ArmA2,
                LaunchType = LaunchTypes.Release
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Operation Arrowhead (Steam)", new LaunchParameters()
            {
                Game = Game.OperationArrowhead,
                LaunchType = LaunchTypes.Steam
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Operation Arrowhead (Retail)", new LaunchParameters()
            {
                Game = Game.OperationArrowhead,
                LaunchType = LaunchTypes.Release
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Combined Operations (Steam)", new LaunchParameters()
            {
                Game = Game.CombinedOperations,
                LaunchType = LaunchTypes.Steam
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Combined Operations (Retail)", new LaunchParameters()
            {
                Game = Game.CombinedOperations,
                LaunchType = LaunchTypes.Release
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Take On Helicopters (Retail)", new LaunchParameters()
            {
                Game = Game.TakeOnHelicopters,
                LaunchType = LaunchTypes.Release
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Take On Helicopters (Steam)", new LaunchParameters()
            {
                Game = Game.TakeOnHelicopters,
                LaunchType = LaunchTypes.Steam
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Take On Helicopters: Rearmed (Retail)", new LaunchParameters()
            {
                Game = Game.TakeOnHelicoptersRearmed,
                LaunchType = LaunchTypes.Release
            }));

            PinnedLaunchers.Add(new PinnedConfiguration("Take On Helicopters: Rearmed (Steam)", new LaunchParameters()
            {
                Game = Game.TakeOnHelicoptersRearmed,
                LaunchType = LaunchTypes.Steam
            }));



        }

        public bool IsBusy
        { get; set; }

        public string BusyText
        { get; set; }


        #region Tab Visibilities

        public IEnumerable<Game> Games
        {
            get
            {
                if (Game.ArmA2.IsGamePresent(LaunchTypes.Release))
                    yield return Game.ArmA2;

                if (Game.OperationArrowhead.IsGamePresent(LaunchTypes.Release))
                    yield return Game.OperationArrowhead;

                if (Game.CombinedOperations.IsGamePresent(LaunchTypes.Release))
                    yield return Game.CombinedOperations;

                if (Game.TakeOnHelicopters.IsGamePresent(LaunchTypes.Release))
                    yield return Game.TakeOnHelicopters;

                if (Game.TakeOnHelicoptersRearmed.IsGamePresent(LaunchTypes.Release))
                    yield return Game.TakeOnHelicoptersRearmed;
            }
        }

        #endregion

        public ObservableCollection<PinnedConfiguration> PinnedLaunchers
        { get; set; }

        public void LaunchGame(PinnedConfiguration configuration)
        {
            BusyText = "Running " + configuration.Name;
            IsBusy = true;
            BISLauncher.StartGame(configuration.Parameters);
            IsBusy = false;
        }

        public void EditShortcut(PinnedConfiguration configuration)
        {

        }

        public void RemoveShortcut(PinnedConfiguration configuration)
        {
            PinnedLaunchers.Remove(configuration);
        }

        public void LaunchWebsite()
        {
            System.Diagnostics.Process.Start("http://sierrasoftworks.com");
        }

        public void LaunchEmail()
        {
            System.Diagnostics.Process.Start("mailto:contact@sierrasoftworks.com");
        }

    }
}
