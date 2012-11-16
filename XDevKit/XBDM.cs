using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace XDevKit
{
    public class XBDM
    {
        private Socket _socket = null;
        private const int CONNECTION_TIMEOUT_MILLISECONDS = 10000; // 7000 milliseconds
        private const int MAX_BUFFER_SIZE = 1020; // 1 kb
        private const int MAX_RETRY_COUNT = 5;

        private string _consoleIP = null;
        private int _consolePort = 730;
        private bool _isConnected = false;
        private ManualResetEvent _pausingThread = new ManualResetEvent(false);

        public String ConsoleIP
        {
            get { return _consoleIP; }
            set { _consoleIP = value; }
        }
        public int ConsolePort
        {
            get { return _consolePort; }
            set { _consolePort = value; }
        }
        public DebugConsoleData ConsoleData = new DebugConsoleData();
        public IList<Drive> ConsoleDrives = new List<Drive>();

        public class DebugConsoleData
        {
            public string ConsoleDebugName { get; set; }
            public string ConsoleType { get; set; }
            public string ActiveTitle { get; set; }
            public bool IsLocked { get; set; }

            public int Timestamp { get; set; }
            public int Checksum { get; set; }
        }


        public XBDM() { }

        public bool Connect()
        {
            if (!_isConnected)
            {
                if (_consoleIP == "" || _consoleIP == null)
                    return false;

                _isConnected = false;
                DnsEndPoint hostEntry = new DnsEndPoint(_consoleIP, _consolePort);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = hostEntry;
                socketEventArg.Completed += (o, args) =>
                    {
                        _pausingThread.Set();
                    };
                _pausingThread.Reset();
                _socket.ConnectAsync(socketEventArg);
                _pausingThread.WaitOne(CONNECTION_TIMEOUT_MILLISECONDS);

                // Send test command
                _isConnected = IsConnected();

                return _isConnected;
            }
            else
                return true;
        }
        private bool IsConnected()
        {
            string command = "\r\n";
            bool response = false;

            SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
            socketEventArgs.RemoteEndPoint = _socket.RemoteEndPoint;
            socketEventArgs.UserToken = null;

            socketEventArgs.Completed += (o, args) =>
            {
                if (args.SocketError == SocketError.Success)
                    response = true;
            };
            byte[] commandBytes = Encoding.ASCII.ToBytes(command);
            socketEventArgs.SetBuffer(commandBytes, 0, commandBytes.Length);

            _pausingThread.Reset();
            _socket.SendAsync(socketEventArgs);
            _pausingThread.WaitOne(CONNECTION_TIMEOUT_MILLISECONDS);

            return response;
        }

        public bool Disconnect()
        {
            try
            {
                if (_isConnected)
                {
                    _isConnected = false;

                    return true;
                }
                else
                    return true;
            }
            catch { return false; }
        }

        public void SendTextCommand(string command, bool includeConnectionTest = true)
        {
            if (includeConnectionTest)
            {
                if (!_isConnected)
                    Connect();
                if (!_isConnected)
                    return;
            }

            command = command.ToUpper() + "\r\n";

            SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
            socketEventArgs.RemoteEndPoint = _socket.RemoteEndPoint;
            socketEventArgs.UserToken = null;

            socketEventArgs.Completed += (o, args) =>
                {
                    if (args.SocketError == SocketError.Success)
                    {
                        string output = Encoding.ASCII.GetString(args.Buffer);
                        _pausingThread.Set();
                    }
                    else
                        throw new Exception(args.SocketError.ToString());
                };
            byte[] commandBytes = Encoding.ASCII.ToBytes(command);
            socketEventArgs.SetBuffer(commandBytes, 0, commandBytes.Length);

            _pausingThread.Reset();
            _socket.SendAsync(socketEventArgs);
            _pausingThread.WaitOne(CONNECTION_TIMEOUT_MILLISECONDS);
        }

        public enum ResponseTypes
        {
            Singleline = 200,
            Connected  = 201,
            Multiline  = 202,
            OtherSingleLineidontevenknow = 420
        }
        public string GetFromTextCommand(string command, string mustFind, bool canBeEmpty, ResponseTypes responseType, bool includeConnectionTest = true)
        {
            try
            {
                if (includeConnectionTest)
                {
                    if (!_isConnected)
                        Connect();
                    if (!_isConnected)
                        return null;
                }

                int retries = 0;
            retry:

                SendTextCommand(command, includeConnectionTest);

                string response = GetFromTextCommand(includeConnectionTest);

                if (!response.StartsWith(((int)responseType).ToString() + "-"))
                    if (retries >= MAX_RETRY_COUNT)
                        return null;
                    else
                    {
                        retries++;
                        goto retry;
                    }

                if (responseType == ResponseTypes.Multiline)
                {
                    while (!response.EndsWith(".\r\n"))
                    {
                        string newResponse = GetFromTextCommand(includeConnectionTest);

                        response += newResponse;
                    }

                    if (!response.StartsWith("202"))
                        response = "";

                    if (canBeEmpty && response.EndsWith(".\r\n"))
                        return response;

                    if (!response.Contains(mustFind))
                    {
                        if (retries >= MAX_RETRY_COUNT)
                            return null;
                        else
                        {
                            retries++;
                            goto retry;
                        }
                    }
                }

                return response;
            }
            catch { return null; }
        }
        private string GetFromTextCommand(bool includeConnectionTest = true)
        {
            if (!_isConnected)
                Connect();
            if (!_isConnected)
                return null;

            string response = "";

            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
            socketEventArg.UserToken = null;

            socketEventArg.SetBuffer(new Byte[MAX_BUFFER_SIZE], 0, MAX_BUFFER_SIZE);

            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                if (e.SocketError == SocketError.Success)
                {
                    response = Encoding.ASCII.GetString(e.Buffer);
                    response = response.Trim('\0');
                }
                else
                    throw new Exception(e.SocketError.ToString());

                _pausingThread.Set();
            });

            _pausingThread.Reset();
            _socket.ReceiveAsync(socketEventArg);
            _pausingThread.WaitOne(CONNECTION_TIMEOUT_MILLISECONDS);

            return response;
        }

        #region Console Info
        public void UpdateConsoleInfo()
        {
            // Get Debug Name
            GetConsoleDebugName();

            // Get Console Type
            GetConsoleType();

            // Get Console BoxID
            GetConsoleBoxID();

            // Get Console Running Info
            GetConsoleRunningInfo();
        }

        private void GetConsoleDebugName()
        {
            try
            {
                string response = GetFromTextCommand("dbgname", "", false, ResponseTypes.Singleline);//.Split('\n')[1].Remove(0, 5).Replace("\r", "");

                if (response == null || response == "")
                    throw new Exception();

                if (response.ToLower().Contains("connected"))
                    response = response.Split('\n')[1].Remove(0, 5).Replace("\r", "");
                else
                    response = response.Remove(0, 5).Replace("\r", "");

                ConsoleData.ConsoleDebugName = response.Replace("\n", "");
            }
            catch { ConsoleData.ConsoleDebugName = "Unable to get debug name"; }
        }
        private void GetConsoleType()
        {
            try
            {
                string response = GetFromTextCommand("consoletype", "", false, ResponseTypes.Singleline);

                if (response == null || response == "")
                    throw new Exception();

                ConsoleData.ConsoleType = response.Split('\n')[0].Remove(0, 5).Replace("\r", "");
            }
            catch { ConsoleData.ConsoleType = "Unable to get console type"; }
        }
        private void GetConsoleBoxID()
        {
            try
            {
                string response = GetFromTextCommand("boxid", "", false, ResponseTypes.OtherSingleLineidontevenknow);

                if (response == null || response == "")
                    throw new Exception();

                ConsoleData.IsLocked = !(response.ToLower().Contains("box is not locked"));
            }
            catch { ConsoleData.IsLocked = false; }
        }
        private void GetConsoleRunningInfo()
        {
            string response = GetFromTextCommand("xbeinfo running", "name=", false, ResponseTypes.Multiline);

            if (response == null || response == "")
                throw new Exception();

            else if (response.Contains("timestamp") && response.Contains("checksum") && response.Contains("name"))
            {
                string[] parsed = response.Replace("\r", "").Split('\n');


                // Get Active Title
                ConsoleData.ActiveTitle = parsed[2].Replace("name=", "").Replace("\"", "");

                // Get Timestamp and Checksum
                string[] wtfisthis = parsed[1].Split(' ');
                ConsoleData.Timestamp = int.Parse(wtfisthis[0].Replace("timestamp=0x", ""), System.Globalization.NumberStyles.HexNumber);
                ConsoleData.Checksum = int.Parse(wtfisthis[1].Replace("checksum=0x", ""), System.Globalization.NumberStyles.HexNumber);
            }
        }
        #endregion

        #region FileSystem
        public class Drive
        {
            public string DriveFriendlyName { get; set; }
            public string DrivePath { get; set; }

            public ulong FreeSpace { get; set; }
            public ulong TotalSpace { get; set; }
        }
        public class DirectoryObject
        {
            public string Name { get; set; }

            public ulong SizeHi { get; set; }
            public ulong SizeLo { get; set; }

            public ulong CreateHi { get; set; }
            public ulong CreateLo { get; set; }

            public ulong ChangeHi { get; set; }
            public ulong ChangeLo { get; set; }

            public bool IsDirectory { get; set; }
            public bool IsXEX { get; set; }
        }

        public void GetDriveList()
        {
            ConsoleDrives.Clear();

            /*
            202- multiline response follows\r\ndrivename=\"E\"\r\ndrivename=\"DEVKIT\"\r\ndrivename=\"HDD\"\r\ndrivename=\"FLASH\"\r\n.\r\n 
            */
            string[] response = GetFromTextCommand("drivelist", "drivename", false, ResponseTypes.Multiline).Replace("\r", "").Split('\n');

            if (response == null || response[0] == "")
                throw new Exception("Unable to get list of drives");

            foreach (string drive in response)
            {
                if (!drive.StartsWith("202"))
                {
                    string drivePath = drive.Replace("\"", "").Replace("drivename=", "");

                    /*
                    202- multiline response follows\r\nfreetocallerlo=0x00000000 freetocallerhi=0x00000000 totalbyteslo=0x00f80000 totalbyteshi=0x00000000 totalfreebyteslo=0x00000000 totalfreebyteshi=0x00000000\r\n.\r\n
                     * 
                     * Capacity: totalbyteslo
                     * Free Space: freetocallerlo
                    */
                    if (drivePath != "." && drivePath != "")
                    {
                        response = GetFromTextCommand(string.Format("drivefreespace name=\"{0}\"", drivePath + ":\\"), "freetocallerlo", false, ResponseTypes.Multiline).Replace("\r", "").Split('\n');

                        if (response == null || response[0] == "")
                            throw new Exception("Unable to get drive size");

                        string[] sizeTypes = response[1].Split(' ');

                        ConsoleDrives.Add(new Drive()
                        {
                            DrivePath = drivePath + @":\",
                            DriveFriendlyName = Helpers.GetFriendlyNameFromPath(drivePath),
                            FreeSpace = ulong.Parse(sizeTypes[0].Replace("freetocallerlo=0x", ""), System.Globalization.NumberStyles.HexNumber),
                            TotalSpace = ulong.Parse(sizeTypes[2].Replace("totalbyteslo=0x", ""), System.Globalization.NumberStyles.HexNumber)
                        });
                    }
                }
            }
        }
        public IList<DirectoryObject> GetDirList(string directory)
        {
            IList<DirectoryObject> direcObject = new List<DirectoryObject>();

            string response = null;
            for(int i=0;i<MAX_RETRY_COUNT;i++)
            {
                string response0 = GetFromTextCommand(string.Format("dirlist name=\"{0}\"", directory), "sizelo=", true, ResponseTypes.Multiline);
                string response1 = GetFromTextCommand(string.Format("dirlist name=\"{0}\"", directory), "sizelo=", true, ResponseTypes.Multiline);

                if (response0 == response1)
                {
                    response = response0;
                    break;
                }
            }

            if (response == null || response == "")
                throw new Exception();

            string[] responseA = response.Replace("\r", "").Split('\n');

            foreach (string part in responseA)
            {
                if (part.StartsWith("name="))
                {
                    bool isInQuotes = false;

                    List<char> newString = new List<char>();
                    foreach (char chr in part)
                    {
                        char newchr = chr;

                        if (chr == '"')
                            isInQuotes = !isInQuotes;

                        if (chr == ' ' && isInQuotes)
                            newchr = '>';

                        newString.Add(newchr);
                    }

                    string[] details = new string(newString.ToArray()).Replace("\"", "").Split(' ');

                    direcObject.Add(new DirectoryObject()
                    {
                        Name = details[0].Replace("name=", "").Replace(">", " "),

                        SizeHi = ulong.Parse(details[1].Replace("sizehi=0x", ""), System.Globalization.NumberStyles.HexNumber),
                        SizeLo = ulong.Parse(details[2].Replace("sizelo=0x", ""), System.Globalization.NumberStyles.HexNumber),

                        CreateHi = ulong.Parse(details[3].Replace("createhi=0x", ""), System.Globalization.NumberStyles.HexNumber),
                        CreateLo = ulong.Parse(details[4].Replace("createlo=0x", ""), System.Globalization.NumberStyles.HexNumber),

                        ChangeHi = ulong.Parse(details[5].Replace("changehi=0x", ""), System.Globalization.NumberStyles.HexNumber),
                        ChangeLo = ulong.Parse(details[6].Replace("changelo=0x", ""), System.Globalization.NumberStyles.HexNumber),

                        IsDirectory = (details.Length == 8),
                        IsXEX = (details[0].Replace("name=", "").ToLower().EndsWith(".xex"))
                    });
                }
            }

            return direcObject;
        }
        public void LaunchXEX(string xexPath, string xexDirectory)
        {
            try
            {
                SendTextCommand(string.Format("magicboot title=\"{0}\" directory=\"{1}\"", xexPath, xexDirectory));
            }
            catch
            {
                MessageBox.Show("Unable to launch xex.", "oops :(", MessageBoxButton.OK);
            }
        }
        #endregion
    }
}