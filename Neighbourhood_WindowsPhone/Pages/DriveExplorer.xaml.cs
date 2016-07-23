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
using System.Collections.ObjectModel;

namespace Neighbourhood_WindowsPhone.Pages
{
	public partial class DriveExplorer : PhoneApplicationPage
	{
		public class DirectoryObject
		{
			public string Name { get; set; }
			public string FriendlySize { get; set; }
			public Visibility ShowSize { get; set; }

			public bool IsDirectory { get; set; }
			public bool IsXEX { get; set; }

			public XBDM.DirectoryObject Base { get; set; }
		}

		private ObservableCollection<DirectoryObject> filesInDirectory = new ObservableCollection<DirectoryObject>();
		private IList<string> directoryHistory = new List<string>();
		XBDM.Drive _drive;

		public DriveExplorer()
		{
			InitializeComponent();

			_drive = App.TempStorageDRIVE;
			directoryHistory.Add(_drive.DrivePath);

			this.DataContext = filesInDirectory;

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
			string currentDirectory = directoryHistory[directoryHistory.Count - 1];
			lblFullPath.Text = currentDirectory;
			IList<XBDM.DirectoryObject> directoryObjects = App.XBDM.GetDirList(currentDirectory);

			filesInDirectory.Clear();
			foreach (XBDM.DirectoryObject directoryObject in directoryObjects)
			{
				DirectoryObject obj = new DirectoryObject();

				obj.IsXEX = directoryObject.IsXEX;
				obj.IsDirectory = directoryObject.IsDirectory;
				obj.Name = directoryObject.Name;
				obj.Base = directoryObject;

				if (!obj.IsDirectory)
				{
					obj.FriendlySize = XDevKit.Helpers.GetFriendlySizeName(directoryObject.SizeLo);
					obj.ShowSize = System.Windows.Visibility.Visible;
				}
				else
				{
					obj.FriendlySize = "";
					obj.ShowSize = System.Windows.Visibility.Collapsed;
				}

				filesInDirectory.Add(obj);
			}
		}

		private void lbFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lbFiles.SelectedItem != null)
			{
				DirectoryObject directoryObject = lbFiles.SelectedItem as DirectoryObject;
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

		private void appbarGoBack_Click(object sender, EventArgs e)
		{
			App.RemovePageFromBackstack = true;
			NavigationService.Navigate(new Uri("/Pages/CommandHome.xaml", UriKind.Relative));
		}
		private void appbarRefresh_Click(object sender, EventArgs e)
		{
			LoadDirectory();
		}

		private void contextFav_Click(object sender, RoutedEventArgs e)
		{
			ListBoxItem lbi = lbFiles.ItemContainerGenerator.ContainerFromItem((sender as ContextMenu).DataContext) as ListBoxItem;

			DirectoryObject directoryObj = lbi.Content as DirectoryObject;

			if (directoryObj.IsXEX)
			{
				// Fav XEX
				App.StorageVault.Favorites.Add(new Resources.StorageStructures.Favorite()
				{
					Directory = directoryHistory[directoryHistory.Count - 1],
					Name = directoryObj.Name,
					Path = directoryHistory[directoryHistory.Count - 1] + directoryObj.Name,
					IsXEX = true
				});
			}
			else
			{
				// Fav Directory
				App.StorageVault.Favorites.Add(new Resources.StorageStructures.Favorite()
				{
					Directory = directoryHistory[directoryHistory.Count - 1],
					Name = directoryObj.Name,
					Path = directoryHistory[directoryHistory.Count - 1] + directoryObj.Name,
					IsXEX = false
				});
			}

			App.StorageVault.SaveSettings();
		}
	}
}