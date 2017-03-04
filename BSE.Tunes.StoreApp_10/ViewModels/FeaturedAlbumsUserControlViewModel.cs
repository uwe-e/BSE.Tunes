using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class FeaturedAlbumsUserControlViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private ICommand m_rightTappedCommand;
        private bool m_isOpen;
        #endregion

        #region Properties
        public bool IsOpen
        {
            get
            {
                return this.m_isOpen;
            }
            set
            {
                this.m_isOpen = value;
                RaisePropertyChanged("IsOpen");
            }
        }
        public ICommand RightTappedCommand => m_rightTappedCommand ?? (m_rightTappedCommand = new RelayCommand<GridPanelItemViewModel>(TestFunction));
        #endregion

        #region MethodsPublic
        public override async void LoadData()
        {
            var newestAlbums = await DataService.GetNewestAlbums(20);
            if (newestAlbums != null)
            {
                foreach (var album in newestAlbums)
                {
                    if (album != null)
                    {
                        Items.Add(new GridPanelItemViewModel
                        {
                            Title = album.Title,
                            Subtitle = album.Artist.Name,
                            Data = album,
                            ImageSource = DataService.GetImage(album.AlbumId, true)
                        });
                    }
                }
            }
        }
        public override void SelectItem(GridPanelItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        public override void NavigateTo()
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumsPage));
        }
        #endregion

        #region MethodsPrivate
        private void TestFunction(GridPanelItemViewModel item)
        {
            this.IsOpen = true;
        }
        #endregion
    }
}
