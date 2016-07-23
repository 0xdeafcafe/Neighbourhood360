using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neighbourhood_WindowsPhone.Resources
{
	public interface IVault
	{
		void GetSettings();
		void SaveSettings();
	}
}
