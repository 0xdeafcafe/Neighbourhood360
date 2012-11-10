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

namespace Neighbourhood_WP.Controls
{
    public partial class FilesystemDrive : UserControl
    {
        public FilesystemDrive(XBDM.Drive drive)
        {
            InitializeComponent();

            lblDriveName.Text = drive.DriveFriendlyName;
            lblDrivePath.Text = drive.DrivePath;
            lblDriveFreeSpace.Text = XDevKit.Helpers.GetFriendlySizeName(drive.FreeSpace) + " (" + drive.FreeSpace.ToString() + " bytes)";
            lblDriveCapacity.Text = XDevKit.Helpers.GetFriendlySizeName(drive.TotalSpace) + " (" + drive.TotalSpace.ToString() + " bytes)";

            // Save Tag
            this.Tag = drive;
        }
    }
}
