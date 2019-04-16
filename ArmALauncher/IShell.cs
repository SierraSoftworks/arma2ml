using System.Collections.ObjectModel;
using System.Collections.Generic;
using BISLib;
namespace ArmALauncher {
    public interface IShell
    {
        bool IsBusy
        { get; }

        string BusyText
        { get; }

        IEnumerable<Game> Games { get; }

        void EditShortcut(PinnedConfiguration configuration);
        void RemoveShortcut(PinnedConfiguration configuration);

        ObservableCollection<PinnedConfiguration> PinnedLaunchers
        { get; set; }

        void LaunchGame(PinnedConfiguration configuration);
        
        void LaunchWebsite();
        void LaunchEmail();
    }
}
