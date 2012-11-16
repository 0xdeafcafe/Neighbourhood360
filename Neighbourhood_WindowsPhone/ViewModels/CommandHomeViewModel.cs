using Neighbourhood_WindowsPhone.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neighbourhood_WindowsPhone.ViewModels
{
    public class CommandHomeViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<StorageStructures.Favorite> Favouites = new ObservableCollection<StorageStructures.Favorite>();

        public void UpdateFavourites()
        {
            Favouites.Clear();
            foreach (StorageStructures.Favorite fav in App.StorageVault.Favorites)
            {
                Favouites.Add(fav);
                NotifyPropertChanged("Favourites");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
