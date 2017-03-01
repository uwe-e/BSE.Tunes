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
    public class FeaturedAlbumsUserControlViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ItemsGroupViewModel m_itemsGroup;
        private ICommand m_navigateToPageCommand;
        private ICommand m_selectItemCommand;
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
        public ICommand NavigateToPageCommand => m_navigateToPageCommand ?? (m_navigateToPageCommand = new RelayCommand<object>(vm => NavigateTo()));
        public ICommand SelectItemCommand => m_selectItemCommand ?? (m_selectItemCommand = new RelayCommand<ItemViewModel>(SelectItem));
        public ICommand RightTappedCommand => m_rightTappedCommand ?? (m_rightTappedCommand = new RelayCommand<ItemViewModel>(TestFunction));
        #endregion

        #region MethodsPublic
        public FeaturedAlbumsUserControlViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadData();
            }
        }
        public virtual void NavigateTo()
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumsPage));
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
        private void SelectItem(ItemViewModel item)
        {
            NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        private void TestFunction(ItemViewModel item)
        {
            this.IsOpen = true;
        }
        #endregion
    }
}
