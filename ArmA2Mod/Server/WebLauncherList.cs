using System;
using System.Collections.Generic;
using System.Text;

namespace ArmA2Mod.Server
{
    class WebLauncherList : List<WebLauncher>,IEnumerable<WebLauncher>, IList<WebLauncher>
    {
        public WebLauncher this[string name]
        {
            get
            {
                foreach (WebLauncher launcher in this)
                    if (launcher.ServerName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                        return launcher;

                return null;
            }
            set
            {
                for (int i = 0; i < this.Count; i++)
                    if (this[i].ServerName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        this[i] = value;
                        return;
                    }

            }
        }
    }
}
