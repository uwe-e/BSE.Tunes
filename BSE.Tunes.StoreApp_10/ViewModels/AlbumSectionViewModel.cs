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
    public class AlbumSectionViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ItemsGroupViewModel m_itemsGroup;
        private ICommand m_navigateToPageCommand;
        #endregion

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
        public ICommand NavigateToPageCommand => m_navigateToPageCommand ?? (m_navigateToPageCommand = new RelayCommand<object>(vm => NavigateTo()));
        #endregion

        #region MethodsPublic
        public AlbumSectionViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                NavigationService = NavigationService ?? Template10.Common.WindowWrapper.Current().NavigationServices.FirstOrDefault();
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
        #endregion
    }
}
