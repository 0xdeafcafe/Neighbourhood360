using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.IO.IsolatedStorage;

namespace Neighbourhood_WindowsPhone.Resources
{
    public class StorageStructures
    {
        public class Favorite
        {
            public string Directory { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
            public bool IsXEX { get; set; }
        }
    }

    public class StorageVault : IVault
    {
        private IsolatedStorageSettings applicationSettings = IsolatedStorageSettings.ApplicationSettings;

        public IList<StorageStructures.Favorite> Favorites = new List<StorageStructures.Favorite>();

        public void GetSettings()
        {
            if (applicationSettings.Contains("Favourites"))
                Favorites = (IList<StorageStructures.Favorite>)applicationSettings["Favourites"];
        }
        public void SaveSettings()
        {
            if (applicationSettings.Contains("Favourites"))
                applicationSettings["Favourites"] = Favorites;
            else
                applicationSettings.Add("Favourites", Favorites);

            applicationSettings.Save();
        }
    }
}
