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

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
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
                MessageBox.Show("Invalid IP/Port", "The IP address or Port you entered is not valid.", MessageBoxButton.OK);
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
                MessageBox.Show("Connection Error", "Unable to connect to debug xbox. Check it is turned on and connected to the same network!", MessageBoxButton.OK);
                return;
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}