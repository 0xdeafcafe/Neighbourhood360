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
using System.ComponentModel;
using System.Threading;

namespace Neighbourhood_WindowsPhone
{
    public partial class Startup : PhoneApplicationPage
    {
        // Constructor
        public Startup()
        {
            InitializeComponent();

#if DEBUG
            txtIP.Text = "192.168.1.85";
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.CredentialVault != null &&
                !String.IsNullOrEmpty(App.CredentialVault.ConsoleIP))
                txtIP.Text = App.CredentialVault.ConsoleIP;
        }

        private bool _isConnectingBusy = false;
        private string _ip = "";
        private int _port = 0;
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnectingBusy)
            {
                // Create Pending UI
                gridMask.Opacity = 0.8;
                gridMask.Visibility = System.Windows.Visibility.Visible;
                ProgressIndicator progIndicator = new ProgressIndicator();
                progIndicator.IsIndeterminate = true;
                progIndicator.IsVisible = true;
                progIndicator.Text = "Connected to console...";
                SystemTray.SetProgressIndicator(this, progIndicator);

                // Get Port and IP from UI
                bool isValidInput = true;
                string pattern = @"\d\d?\d?\.\d\d?\d?\.\d\d?\d?\.\d\d?\d?";
                Regex regexIP = new Regex(pattern);
                Match matchIP = regexIP.Match(_ip = txtIP.Text);
                if (!matchIP.Success || !int.TryParse(txtPort.Text, out _port))
                {
                    MessageBox.Show("The IP address or Port you entered is not valid.", "Invalid IP/Port", MessageBoxButton.OK);
                    isValidInput = false;
                }

                // If input is valid, create thead and connect to console
                if (isValidInput)
                {
                    // Tell Application that we are currently connecting
                    _isConnectingBusy = true;

                    // Create Thread to store Connecting in
                    Thread thread = new Thread(new ThreadStart(ConnectToConsole));
                    thread.Start();
                }
                else
                {
                    // Unset IP and Port storage
                    _ip = "";
                    _port = 0;

                    // Remove Pending UI
                    gridMask.Opacity = 0.0;
                    gridMask.Visibility = System.Windows.Visibility.Collapsed;
                    progIndicator = new ProgressIndicator();
                    progIndicator.IsIndeterminate = false;
                    progIndicator.IsVisible = false;
                    SystemTray.SetProgressIndicator(this, progIndicator);

                    // Tell the application that we are no longer connecting
                    _isConnectingBusy = false;
                }
            }
        }

        void ConnectToConsole()
        {
            // aight, port and IP are all good. Let's do dis.
            App.XBDM.ConsoleIP = _ip;
            App.XBDM.ConsolePort = _port;
            App.XBDM.Disconnect();
            if (App.XBDM.Connect())
            {
                // Connection was good, so store data
                App.CredentialVault.ConsoleIP = _ip;
                App.CredentialVault.ConsolePort = _port;
                App.CredentialVault.SaveSettings();

                // And then Proceed
                Dispatcher.BeginInvoke(new Action(delegate
                {
                    NavigationService.Navigate(new Uri("/Pages/CommandHome.xaml", UriKind.Relative));
                }));
            }
            else
            {
                _ip = "";
                _port = 0;

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    MessageBox.Show("Unable to connect to debug xbox. Check it is turned on and connected to the same network.", "Connection Error", MessageBoxButton.OK);
                }));
            }

            Dispatcher.BeginInvoke(new Action(delegate
            {
                // Remove Pending UI
                gridMask.Opacity = 0.0;
                gridMask.Visibility = System.Windows.Visibility.Collapsed;
                ProgressIndicator progIndicator = new ProgressIndicator();
                progIndicator.IsIndeterminate = false;
                progIndicator.IsVisible = false;
                SystemTray.SetProgressIndicator(this, progIndicator);

                // Tell the application that we are no longer connecting
                _isConnectingBusy = false;
            }));
        }
    }
}