using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class FeaturedItemsBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ObservableCollection<GridPanelItemViewModel> m_items;
        private ICommand m_navigateToPageCommand;
        private ICommand m_selectItemCommand;
        #endregion

        #region Properties
        public virtual ObservableCollection<GridPanelItemViewModel> Items
        {
            get
            {
                if (m_items == null)
                {
                    m_items = new ObservableCollection<GridPanelItemViewModel>();
                }
                return m_items;
            }
        }
        public ICommand NavigateToPageCommand => m_navigateToPageCommand ?? (m_navigateToPageCommand = new RelayCommand<object>(vm => NavigateTo()));
        public ICommand SelectItemCommand => m_selectItemCommand ?? (m_selectItemCommand = new RelayCommand<GridPanelItemViewModel>(SelectItem));
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
        }

        public virtual void LoadData()
        {
        }

        public virtual void SelectItem(GridPanelItemViewModel item)
        {
        }
        public virtual string FormatNumberOfEntriesString(Playlist playlist)
        {
            int numberOfEntries = 0;
            if (playlist != null)
            {
                numberOfEntries = playlist.NumberEntries;
            }
            return string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, ResourceService.GetString("PlaylistItem_PartNumberOfEntries", "Songs"));
        }
        #endregion
    }
}
