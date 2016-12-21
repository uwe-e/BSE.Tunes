using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AlbumSectionViewModel : ViewModelBase
    {
        private ItemsGroupViewModel m_itemsGroup;
        #region Properties
        public ItemsGroupViewModel ItemsGroup
        {
            get
            {
                return this.m_itemsGroup;
            }
            set
            {
                this.m_itemsGroup = value;
                RaisePropertyChanged("ItemsGroup");
            }
        }
        #endregion

        #region MethodsPublic
        public AlbumSectionViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadData();
            }
        }
        #endregion

        #region MethodsPrivate
        private async void LoadData()
        {
            this.ItemsGroup = new ItemsGroupViewModel();
            var newestAlbums = await DataService.GetNewestAlbums(20);
            if (newestAlbums != null)
            {
                foreach (var album in newestAlbums)
                {
                    if (album != null)
                    {
                        this.ItemsGroup.Items.Add(new ItemViewModel
                        {
                            Title = album.Title,
                            Subtitle = album.Artist.Name,
                            Data = album,
                            ImageSource = DataService.GetImage(album.AlbumId, true)
                        });
                    }
                }
//                this.IsBusy = false;
            }
        }
        #endregion
    }
}
