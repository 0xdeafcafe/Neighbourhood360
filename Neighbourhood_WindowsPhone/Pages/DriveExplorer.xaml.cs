using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using XDevKit;
using Neighbourhood_WindowsPhone.Controls;

namespace Neighbourhood_WindowsPhone.Pages
{
    public partial class DriveExplorer : PhoneApplicationPage
    {
        private IList<string> directoryHistory = new List<string>();
        XBDM.Drive _drive;

        public DriveExplorer()
        {
            InitializeComponent();

            _drive = App.TempStorageDRIVE;

            directoryHistory.Add(_drive.DrivePath);

            if (_drive.DrivePath != _drive.DriveFriendlyName)
                lblDriveName.Text = _drive.DrivePath + " - " + _drive.DriveFriendlyName;
            else
                lblDriveName.Text = _drive.DrivePath;

            LoadDirectory();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            if (directoryHistory.Count > 1)
            {
                // Cancel back press
                e.Cancel = true;

                // Navigate down 1 directory History
                GoBackDirectoryPath();
            }
        }

        private void GoBackDirectoryPath()
        {
            // Remove current directory path
            directoryHistory.RemoveAt(directoryHistory.Count - 1);

            // Load the files from the directory
            LoadDirectory();
        }
        private void LoadDirectory()
        {
            lbFiles.IsEnabled = false;

            string currentDirectory = directoryHistory[directoryHistory.Count - 1];

            lblFullPath.Text = currentDirectory;

            IList<XBDM.DirectoryObject> directoryObjects = App.XBDM.GetDirList(currentDirectory);

            lbFiles.Items.Clear();
            foreach (XBDM.DirectoryObject directoryObject in directoryObjects)
            {
                if (directoryObject.IsDirectory)
                    lbFiles.Items.Add(new Controls.DriveFolder(directoryObject));
                else
                    lbFiles.Items.Add(new Controls.DriveFile(directoryObject));
            }

            lbFiles.IsEnabled = true;
        }

        private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFiles.SelectedItem != null)
            {
                XBDM.DirectoryObject directoryObject = null;
                if (lbFiles.SelectedItem is Controls.DriveFile)
                    directoryObject = (XBDM.DirectoryObject)((Controls.DriveFile)lbFiles.SelectedItem).Tag;
                else
                    directoryObject = (XBDM.DirectoryObject)((Controls.DriveFolder)lbFiles.SelectedItem).Tag;

                // DO DIS
                if (directoryObject.IsDirectory)
                {
                    directoryHistory.Add(directoryHistory[directoryHistory.Count - 1] + directoryObject.Name + "\\");
                    LoadDirectory();
                }
                else if (directoryObject.IsXEX)
                {
                    // Load XEX
                    App.XBDM.LaunchXEX(directoryHistory[directoryHistory.Count - 1] + directoryObject.Name, directoryHistory[directoryHistory.Count - 1]);
                }
            }
        }
    }
}