using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Managers;
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
    public class FeaturedItemsBaseViewModel : SearchSuggestionsViewModel
    {
        #region FieldsPrivate
        private ObservableCollection<ListViewItemViewModel> m_items;
        private ICommand m_navigateToPageCommand;
        private ICommand m_selectItemCommand;
        private ICommand m_playAllCommand;
        private ICommand m_showAddToPlaylistDialogCommand;
        private ICommand m_openFlyoutCommand;
        #endregion

        #region Properties
        public virtual ObservableCollection<ListViewItemViewModel> Items => m_items ?? (m_items = new ObservableCollection<ListViewItemViewModel>());
        public ICommand NavigateToPageCommand => m_navigateToPageCommand ?? (m_navigateToPageCommand = new RelayCommand<object>(vm => NavigateTo()));
        public ICommand SelectItemCommand => m_selectItemCommand ?? (m_selectItemCommand = new RelayCommand<GridPanelItemViewModel>(SelectItem));
        public ICommand PlayAllCommand => m_playAllCommand ?? (m_playAllCommand = new RelayCommand<GridPanelItemViewModel>(PlayAll));
        public ICommand ShowAddToPlaylistDialogCommand => m_showAddToPlaylistDialogCommand ?? (m_showAddToPlaylistDialogCommand = new RelayCommand<GridPanelItemViewModel>(ShowAddToPlaylistDialog));
        public ICommand OpenFlyoutCommand => m_openFlyoutCommand ?? (m_openFlyoutCommand = new RelayCommand<GridPanelItemViewModel>(OpenFlyout));
        public PlayerManager PlayerManager
        {
            get;
        } = PlayerManager.Instance;

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
        public virtual void PlayAll(GridPanelItemViewModel item)
        {
        }
        public virtual void ShowAddToPlaylistDialog(GridPanelItemViewModel item)
        {
            item.IsContextOpen = true;
        }
        public virtual void OpenFlyout(GridPanelItemViewModel item)
        {
            item.IsOpen = true;
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
