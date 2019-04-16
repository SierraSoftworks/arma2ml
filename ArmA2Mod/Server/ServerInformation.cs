using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SierraLib.ByteManipulation;
using System.ComponentModel;
using System.Threading;

namespace ArmA2Mod.Server
{
    class ServerInformation : IEnumerable<string>
    {

        public ServerInformation()
        {
            m_parameters = new SortedList<string, string>();
        }

        public const int Timeout = 1000;


        public event EventHandler ServerStatusReceived = null;

        public event EventHandler ServerUnavailable = null;

        public event EventHandler ServerInvalid = null;

        public string this[string key]
        {
            get
            {
                return m_parameters[key];
            }
        }

        private SortedList<string, string> m_parameters;

        public void GetServerStatus(string address)
        {
            try
            {
                string[] addr = address.Split(':');

                IPAddress ip = null;
                int port = 2302;

                if (addr.Length == 1)
                {
                    //No port
                    ip = Dns.GetHostEntry(addr[0]).AddressList[0];
                }
                else if (addr.Length == 2)
                {
                    ip = Dns.GetHostEntry(addr[0]).AddressList[0];
                    port = Convert.ToInt32(addr[1]);
                }

                IPEndPoint ipEndPoint = new IPEndPoint(ip, port);

                //Connect to the server
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);

                //Create the request packet
                byte[] request = new byte[0];

                //We are using Gamespy v2 so packets start with:
                //0xFE 0xFD 0x00
                CreationFunctions.AddByte(ref request, 0xFE);
                CreationFunctions.AddByte(ref request, 0xFD);
                CreationFunctions.AddByte(ref request, 0x00);

                //Now we add a ping string, this can be whatever we like
                CreationFunctions.AddData(ref request, Encoding.ASCII.GetBytes("ArmA2ML"));

                //Now we add the bytes indicating what we want
                byte yes = 0xFF;
                byte no = 0x00;
                CreationFunctions.AddByte(ref request, yes); //Server info
                CreationFunctions.AddByte(ref request, no); //Server rules
                CreationFunctions.AddByte(ref request, no); //Players


                //Send the request
                try
                {
                    sock.SendTo(request, (EndPoint)ipEndPoint);
                }
                catch
                {
                    if (ServerUnavailable != null)
                        ServerUnavailable(this, new EventArgs());
                }

                //Creates a buffer for holding the reply
                byte[] reply = new byte[100 * 1024];

                //Now we get the reply
                EndPoint endPoint = (EndPoint)ipEndPoint;
                sock.ReceiveFrom(reply, ref endPoint);
                sock.Close();

                //Now we process the reply
                byte[] tempParam = null;
                int offset = 0;
                while (NextParam(reply, offset, out tempParam) > 0)
                {
                    offset += tempParam.Length;

                    string keyName = Encoding.ASCII.GetString(tempParam);

                    offset += NextParam(reply, offset, out tempParam);

                    string keyValue = Encoding.ASCII.GetString(tempParam);

                    m_parameters.Add(keyName, keyValue);

                    tempParam = null;
                }

                if (ServerStatusReceived != null)
                    ServerStatusReceived(this, new EventArgs());
            }
            catch
            {
                if (ServerInvalid != null)
                    ServerInvalid(this, new EventArgs());
            }
        }

        public void GetServerStatusAsync(string address)
        {
            ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        string[] addr = address.Split(':');

                        IPAddress ip = null;
                        int port = 2302;

                        if (addr.Length == 1)
                        {
                            //No port
                            ip = Dns.GetHostEntry(addr[0]).AddressList[0];
                        }
                        else if (addr.Length == 2)
                        {
                            ip = Dns.GetHostEntry(addr[0]).AddressList[0];
                            port = Convert.ToInt32(addr[1]);
                        }

                        IPEndPoint ipEndPoint = new IPEndPoint(ip, port);

                        //Connect to the server
                        Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout);

                        //Create the request packet
                        byte[] request = new byte[0];

                        //We are using Gamespy v2 so packets start with:
                        //0xFE 0xFD 0x00
                        CreationFunctions.AddByte(ref request, 0xFE);
                        CreationFunctions.AddByte(ref request, 0xFD);
                        CreationFunctions.AddByte(ref request, 0x00);

                        //Now we add a ping string, this can be whatever we like
                        CreationFunctions.AddByte(ref request, 0x04);
                        CreationFunctions.AddByte(ref request, 0x05);
                        CreationFunctions.AddByte(ref request, 0x06);
                        CreationFunctions.AddByte(ref request, 0x07);

                        //Now we add the bytes indicating what we want
                        byte yes = 0xFF;
                        byte no = 0x00;
                        CreationFunctions.AddByte(ref request, yes); //Server info
                        CreationFunctions.AddByte(ref request, no); //Server rules
                        CreationFunctions.AddByte(ref request, no); //Players

                        //Send the request
                        try
                        {
                            sock.SendTo(request, (EndPoint)ipEndPoint);
                        }
                        catch
                        {
                            if (ServerUnavailable != null)
                                ServerUnavailable(this, new EventArgs());
                        }

                        //Creates a buffer for holding the reply
                        byte[] reply = new byte[100 * 1024];

                        //Now we get the reply
                        EndPoint endPoint = (EndPoint)ipEndPoint;
                        sock.ReceiveFrom(reply, ref endPoint);
                        sock.Close();

                        //Now we process the reply
                        byte[] tempParam = null;
                        int offset = 0;

                        lock (m_parameters)
                        {
                            offset = 5;
                            while (NextParam(reply, offset, out tempParam) > 0)
                            {
                                offset += tempParam.Length + 1;

                                string keyName = Encoding.ASCII.GetString(tempParam);

                                offset += NextParam(reply, offset, out tempParam) + 1;

                                string keyValue = Encoding.ASCII.GetString(tempParam);

                                m_parameters.Add(keyName, keyValue);

                                tempParam = null;
                            }

                        }
                        if (ServerStatusReceived != null)
                            ServerStatusReceived(this, new EventArgs());
                    }
                    catch
                    {
                        if (ServerInvalid != null)
                            ServerInvalid(this, new EventArgs());
                    }

                }, address);
        }

        public int Count
        {
            get { return m_parameters.Count; }
        }

        private int NextParam(byte[] reply,int offset, out byte[] param)
        {
            for (int i = offset; i < reply.Length; i++)
            {
                if (reply[i] == 0)
                {
                    param = new byte[i - offset];
                    Array.Copy(reply, offset, param, 0, i - offset);
                    return i - offset;
                }
            }

            param = new byte[0];
            return 0;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string item in m_parameters.Keys)
                yield return item + " - " + m_parameters[item];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (string item in m_parameters.Keys)
                yield return item + " - " + m_parameters[item];
        }
    }
}
