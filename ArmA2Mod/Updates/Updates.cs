using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace SierraLib.Settings.Updates
{
    public sealed class Updates
    {
        public const string UpdateURL = "https://cdn.sierrasoftworks.com/arma2ml/Updates.xml";

        static BackgroundWorker bwGet;

        private static event EventHandler<UpdateEventArgs> m_newUpdate = null;
        public static event EventHandler<UpdateEventArgs> NewUpdateAvailable
        {
            add
            {
                m_newUpdate = (EventHandler<UpdateEventArgs>)Delegate.Combine(m_newUpdate, value);
            }
            remove
            {
                m_newUpdate = (EventHandler<UpdateEventArgs>)Delegate.Remove(m_newUpdate, value);
            }
        }

        private static event EventHandler m_updateDownloaded = null;
        public static event EventHandler UpdateDownloaded
        {
            add { m_updateDownloaded = (EventHandler)Delegate.Combine(m_updateDownloaded, value); }
            remove { m_updateDownloaded = (EventHandler)Delegate.Remove(m_updateDownloaded, value); }
        }

        public static void GetUpdates()
        {
            bwGet = new BackgroundWorker();
            bwGet.DoWork += (o, e) =>
            {
                Update newU = null;
                Update[] oldU = null;

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(UpdateURL);
                    StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(sr);

                

                    XmlNode node = xDoc.SelectSingleNode("Updates").SelectSingleNode("Latest");


                    XmlNode nde;
                    string name = "", description = "", changelog = "", download = "", info = "";

                    nde = node.SelectSingleNode("Name");
                if(nde != null)
                    name = nde.InnerText;

                nde = node.SelectSingleNode("Description");
                if (nde != null)
                    description = nde.InnerText;

                nde = node.SelectSingleNode("ChangeLog");
                if (nde != null)
                    changelog = nde.InnerText;

                nde = node.SelectSingleNode("Download");
                if (nde != null)
                    download = nde.InnerText;

                nde = node.SelectSingleNode("Info");
                if (nde != null)
                    info = nde.InnerText;

                    newU = new Update(
                        new Version(node.Attributes["version"].Value), 
                        name,
                        description, 
                        changelog,
                        download, 
                        info);

                    node = xDoc.SelectSingleNode("Updates").SelectSingleNode("Archive");

                    List<Update> old = new List<Update>();

                    XmlNodeList clients = node.SelectNodes("Application");

                    foreach (XmlNode client in clients)
                    {

                        nde = client.SelectSingleNode("Name");
                        if (nde != null)
                            name = nde.InnerText;

                        nde = client.SelectSingleNode("Description");
                        if (nde != null)
                            description = nde.InnerText;

                        nde = client.SelectSingleNode("ChangeLog");
                        if (nde != null)
                            changelog = nde.InnerText;

                        nde = client.SelectSingleNode("Download");
                        if (nde != null)
                            download = nde.InnerText;

                        nde = client.SelectSingleNode("Info");
                        if (nde != null)
                            info = nde.InnerText;

                        old.Add(
                            new Update(
                                new Version(client.Attributes["version"].Value),
                                name,
                                description, 
                                changelog,
                                download, 
                                info));
                    }

                    oldU = old.ToArray();

                    e.Result = new UpdateEventArgs(newU, oldU);
            };

            bwGet.RunWorkerCompleted += (o, e) =>
                {
                    if (e.Error == null)
                    {
                        UpdateEventArgs args = (UpdateEventArgs)e.Result;
                        if (m_newUpdate != null)
                            m_newUpdate(null, args);
                    }
                };

            bwGet.RunWorkerAsync();

        }


        public static void DownloadUpdate(Update update, FileInfo destination)
        {
            BackgroundWorker bwDownload = new BackgroundWorker();
            bwDownload.DoWork += (o, e) =>
            {
                string download = ((string[])e.Argument)[0];
                string file = ((string[])e.Argument)[1];

                WebClient client = new WebClient();
                client.DownloadFile(download, file);
                client.Dispose();
            };

            bwDownload.RunWorkerCompleted += (o, e) =>
            {
                if (m_updateDownloaded != null)
                    m_updateDownloaded(update, new EventArgs());
            };

            bwDownload.RunWorkerAsync(new string[] { update.DownloadLink, destination.FullName });
        }

        public static void DownloadUpdate(string updateAddress, FileInfo destination)
        {
            BackgroundWorker bwDownload = new BackgroundWorker();
            bwDownload.DoWork += (o, e) =>
            {
                string download = ((string[])e.Argument)[0];
                string file = ((string[])e.Argument)[1];

                WebClient client = new WebClient();
                client.DownloadFile(download, file);
                client.Dispose();
            };

            bwDownload.RunWorkerCompleted += (o, e) =>
            {
                if (m_updateDownloaded != null)
                    m_updateDownloaded(updateAddress, new EventArgs());
            };

            bwDownload.RunWorkerAsync(new string[] { updateAddress, destination.FullName });
        }
    }

    public sealed class UpdateEventArgs : EventArgs
    {
        private Update newUpdate;
        public Update NewVersion
        {
            get { return newUpdate; }
        }

        private Update[] oldUpdates;
        public Update[] OldVersions
        { get { return oldUpdates; } }

        public UpdateEventArgs(Update newVersion, Update[] oldVersions)
        {
            newUpdate = newVersion;
            oldUpdates = oldVersions;
        }
    }

    public sealed class Update
    {
        private Version version;
        private string name;
        private string description;
        private string changelog;
        private string downloadLink;
        private string infoLink;

        public Version ClientVersion
        {
            get { return version; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public string ChangeLog
        { get { return changelog; } }

        public string DownloadLink
        {
            get { return downloadLink; }
        }

        public string InfoLink
        { get { return infoLink; } }

        public Update(Version vers, string _name, string _desc, string _changelog, string download,string info)
        {
            version = vers;
            name = _name;
            description = _desc;
            changelog = _changelog;
            downloadLink = download;
            infoLink = info;
        }
    }
}
