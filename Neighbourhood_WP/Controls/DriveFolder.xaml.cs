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
    public partial class DriveFolder : UserControl
    {
        public DriveFolder(XBDM.DirectoryObject directoryObject)
        {
            InitializeComponent();

            lblFolderName.Text = directoryObject.Name;

            // Save Tag
            this.Tag = directoryObject;
        }
    }
}
