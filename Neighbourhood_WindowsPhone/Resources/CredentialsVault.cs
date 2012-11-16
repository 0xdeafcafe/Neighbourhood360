using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neighbourhood_WindowsPhone.Resources
{
    public class CredentialsVault : IVault
    {
        private IsolatedStorageSettings applicationSettings = IsolatedStorageSettings.ApplicationSettings;
        
        private string _consoleIP;
        private int _consolePort = 730;

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

        public void GetSettings()
        {
            if (applicationSettings.Contains("DebugConsoleIP"))
                _consoleIP = applicationSettings["DebugConsoleIP"].ToString();

            if (applicationSettings.Contains("DebugConsolePort"))
                _consolePort = Convert.ToInt16(applicationSettings["DebugConsolePort"]);
        }
        public void SaveSettings()
        {
            if (applicationSettings.Contains("DebugConsoleIP"))
                applicationSettings["DebugConsoleIP"] = _consoleIP;
            else
                applicationSettings.Add("DebugConsoleIP", _consoleIP);

            if (applicationSettings.Contains("DebugConsolePort"))
                applicationSettings["DebugConsolePort"] = _consolePort;
            else
                applicationSettings.Add("DebugConsolePort", _consolePort);

            applicationSettings.Save();
        }
    }
}
