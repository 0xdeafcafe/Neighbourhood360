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
    public partial class DriveFile : UserControl
    {
        public DriveFile(XBDM.DirectoryObject directoryObject)
        {
            InitializeComponent();

            lblFileName.Text = directoryObject.Name;
            lblFileSize.Text = XDevKit.Helpers.GetFriendlySizeName(directoryObject.SizeLo);

            // Save Tag
            this.Tag = directoryObject;
        }
    }
}