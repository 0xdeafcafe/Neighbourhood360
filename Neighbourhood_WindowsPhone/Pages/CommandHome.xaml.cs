﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using XDevKit;
using System.Windows.Media;
using System.Threading.Tasks;
using Neighbourhood_WindowsPhone.Controls;
using System.Threading;
using Neighbourhood_WindowsPhone.ViewModels;

namespace Neighbourhood_WindowsPhone.Pages
{
    public partial class CommandHome : PhoneApplicationPage
    {
        private XBDM _xbdm = new XBDM();
        private CommandHomeViewModel PageViewModel = new CommandHomeViewModel();

        public CommandHome()
        {
            InitializeComponent();

            this.DataContext = PageViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Check if we need to remove a page from the Backstack
            if (App.RemovePageFromBackstack)
            {
                NavigationService.RemoveBackEntry();
                App.RemovePageFromBackstack = false;
            }

            // Update XBDM
            _xbdm = App.XBDM;

            // Refresh UI
            appbarRefresh_Click(null, null);

            // Update Favourites
            PageViewModel.UpdateFavourites();
        }

        private void UpdateConsoleInfo()
        {
            _xbdm.UpdateConsoleInfo();

            Dispatcher.BeginInvoke(new Action(delegate
                {
                    lblConsoleDebugName.Text = _xbdm.ConsoleData.ConsoleDebugName;
                    lblConsoleActiveTitle.Text = _xbdm.ConsoleData.ActiveTitle;
                    lblConsoleType.Text = _xbdm.ConsoleData.ConsoleType;

                    lblConsoleUnlockStatus.Text = _xbdm.ConsoleData.IsLocked ? "Locked" : "Unlocked";
                    if (_xbdm.ConsoleData.IsLocked)
                        lblConsoleUnlockStatus.Foreground = (Brush)new SolidColorBrush(Colors.Red);
                    else
                        lblConsoleUnlockStatus.Foreground = (Brush)new SolidColorBrush(Colors.Green);
                }));
        }
        private void UpdateDriveList()
        {
            _xbdm.GetDriveList();

            Dispatcher.BeginInvoke(new Action(delegate
                {
                    lbDrives.Items.Clear();
                    foreach (XBDM.Drive drive in _xbdm.ConsoleDrives)
                        lbDrives.Items.Add(new FilesystemDrive(drive));
                }));
        }

        private void btnCommandFreeze_Click(object sender, RoutedEventArgs e)
        {
            _xbdm.SendTextCommand("stop");
        }
        private void btnCommandUnFreeze_Click(object sender, RoutedEventArgs e)
        {
            _xbdm.SendTextCommand("go");
        }

        private void btnCommandColdReboot_Click(object sender, RoutedEventArgs e)
        {
            _xbdm.SendTextCommand("magicboot COLD");
        }
        private void btnCommandTitleReboot_Click(object sender, RoutedEventArgs e)
        {
            _xbdm.SendTextCommand("magicboot");
        }

		private void btnCommandActiveTitleReboot_Click(object sender, RoutedEventArgs e)
		{
			_xbdm.SendTextCommand(string.Format("magicboot title=\"{0}\" directory=\"{1}\"", _xbdm.ConsoleData.ActiveTitle, _xbdm.ConsoleData.ActiveTitle.Substring(0, _xbdm.ConsoleData.ActiveTitle.LastIndexOf("\\", StringComparison.Ordinal))));
		}

        private void appbarRefresh_Click(object sender, EventArgs e)
        {
            ThreadStart ts = delegate
            {

                // Try catch this shit, because sometimes it's not loaded into memory yet.. soon.
                try
                {
                    SystemTray.ProgressIndicator.Text = "Getting information from the console";
                    SystemTray.ProgressIndicator.IsIndeterminate = true;
                    SystemTray.ProgressIndicator.IsVisible = true;

                    appbarRefresh.IsEnabled = false;
                }
                catch { }

                UpdateConsoleInfo();
                UpdateDriveList();

                try
                {
                    appbarRefresh.IsEnabled = true;

                    SystemTray.ProgressIndicator.IsVisible = false;
                }
                catch { }
            };

            Thread thrd = new Thread(ts);
            thrd.Start();
        }

        private void lbDrives_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbDrives.SelectedItem != null)
            {
                App.TempStorageDRIVE = (XBDM.Drive)((FilesystemDrive)(lbDrives.SelectedItem)).Tag;
                NavigationService.Navigate(new Uri("/Pages/DriveExplorer.xaml", UriKind.Relative));
            }
        }
        private void lbFavs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFavs.SelectedItem != null)
            {

            }
        }

        private void appbarAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AboutNeighbourhood.xaml", UriKind.Relative));
        }
    }
}