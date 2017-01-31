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
    public class FeaturedItemsBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ItemsGroupViewModel m_itemsGroup;
        private ICommand m_navigateToPageCommand;
        private ICommand m_selectItemCommand;
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
        public ICommand SelectItemCommand => m_selectItemCommand ?? (m_selectItemCommand = new RelayCommand<ItemViewModel>(SelectItem));
        #endregion
        
        #region MethodsPublic
        public FeaturedItemsBaseViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadData();
            }
        }
        public virtual void NavigateTo()
        {
            //NavigationService.NavigateAsync(typeof(Views.AlbumsPage));
        }

        public virtual async void LoadData()
        {

        }

        public virtual void SelectItem(ItemViewModel item)
        {
            //NavigationService.NavigateAsync(typeof(Views.AlbumDetailPage), item.Data);
        }
        #endregion

    }
}
