using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Neighbourhood_WindowsPhone.Pages
{
	public partial class AboutNeighbourhood : PhoneApplicationPage
	{
		public AboutNeighbourhood()
		{
			InitializeComponent();
		}

		private void btnGoToGithub_Click(object sender, RoutedEventArgs e)
		{
			WebBrowserTask wb = new WebBrowserTask();
			wb.Uri = new Uri("https://github.com/Xerax/Neighbourhood360");
			wb.Show();
		}
	}
}