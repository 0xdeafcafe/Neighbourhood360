using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Neighbourhood_WindowsPhone.Resources;
using System.Text.RegularExpressions;

namespace Neighbourhood_WindowsPhone
{
    public partial class Startup : PhoneApplicationPage
    {
        // Constructor
        public Startup()
        {
            InitializeComponent();

#if DEBUG
            txtIP.Text = "192.168.1.68";
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.CredentialVault != null &&
                !String.IsNullOrEmpty(App.CredentialVault.ConsoleIP))
                txtIP.Text = App.CredentialVault.ConsoleIP;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            string ip = "";
            int port = 0;

            // Check IP Format
            string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
            Regex regexIP = new Regex(pattern);
            Match matchIP = regexIP.Match(ip = txtIP.Text);
            if (!matchIP.Success || !int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("The IP address or Port you entered is not valid.", "Invalid IP/Port", MessageBoxButton.OK);
                return;
            }

            // aight, port and IP are all good. Let's do dis.
            App.XBDM.ConsoleIP = ip;
            App.XBDM.ConsolePort = port;
            App.XBDM.Disconnect();
            if (App.XBDM.Connect())
            {
                // Connection was good, so store data
                App.CredentialVault.ConsoleIP = ip;
                App.CredentialVault.ConsolePort = port;
                App.CredentialVault.SaveSettings();

                // And then Proceed
                NavigationService.Navigate(new Uri("/Pages/CommandHome.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Unable to connect to debug xbox. Check it is turned on and connected to the same network.", "Connection Error", MessageBoxButton.OK);
                return;
            }
        }
    }
}