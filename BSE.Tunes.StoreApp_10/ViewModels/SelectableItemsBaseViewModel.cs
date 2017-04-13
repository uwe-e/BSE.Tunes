using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SelectableItemsBaseViewModel : FeaturedItemsBaseViewModel
    {
        #region FieldsPrivate
        private ObservableCollection<object> m_selectedItems;
        private bool m_isCommandBarVisible;
        private bool m_hasSelectedItems;
        private ICommand m_selectItemsCommand;
        private ICommand m_clearSelectionCommand;
        private RelayCommand m_playSelectedItemsCommand;
        private ICommand m_openMobileFlyoutCommand;
        #endregion

        #region Properties
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return m_selectedItems;
            }
            set
            {
                m_selectedItems = value;
                RaisePropertyChanged("SelectedItems");
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the the toolbar for the playlist related content is open.
        /// </summary>
        /// <remarks>Because it´s a twoway bound property in xaml the <see cref="HasSelectedItems"/> property can not used.</remarks>
        public virtual bool IsCommandBarVisible
        {
            get
            {
                return m_isCommandBarVisible;
            }
            set
            {
                m_isCommandBarVisible = value;
                RaisePropertyChanged("IsCommandBarVisible");
            }
        }
        /// <summary>
        /// Gets or sets an value indication whether the tracklist has at the minimum one selected item.
        /// </summary>
        public virtual bool HasSelectedItems
        {
            get
            {
                return m_hasSelectedItems;
            }
            set
            {
                m_hasSelectedItems = value;
                RaisePropertyChanged("HasSelectedItems");
            }
        }
        public ICommand SelectItemsCommand => m_selectItemsCommand ?? (m_selectItemsCommand = new RelayCommand<ListViewItemViewModel>(SelectItems));
        public ICommand ClearSelectionCommand => m_clearSelectionCommand ?? (m_clearSelectionCommand = new RelayCommand(ClearSelection));
        public RelayCommand PlaySelectedItemsCommand => m_playSelectedItemsCommand ?? (m_playSelectedItemsCommand = new RelayCommand(PlaySelectedItems, CanPlaySelectedItems));
        public ICommand OpenMobileFlyoutCommand => m_openMobileFlyoutCommand ?? (m_openMobileFlyoutCommand = new RelayCommand<GridPanelItemViewModel>(OpenMobileFlyout));
        #endregion

        #region MethodsPublic
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<object>();
                SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            return base.OnNavigatedToAsync(parameter, mode, state);
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                SelectedItems = null;
            }
            HasSelectedItems = false;
            IsCommandBarVisible = false;
            return base.OnNavigatedFromAsync(state, suspending);
        }
        public virtual void OpenMobileFlyout(GridPanelItemViewModel item)
        {
            if (!IsCommandBarVisible)
            {
                OpenFlyout(item);
            }
        }
        public virtual void SelectItems(ListViewItemViewModel viewModel)
        {
            HasSelectedItems = true;
            SelectedItems.Add(viewModel);
        }
        public virtual void ClearSelection()
        {
            SelectedItems?.Clear();
        }
        public virtual bool CanPlaySelectedItems()
        {
            return HasSelectedItems;
        }
        public virtual void PlaySelectedItems()
        {
        }
        #endregion

        #region MethodsProtected
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasSelectedItems = SelectedItems.Count > 0;
            IsCommandBarVisible = HasSelectedItems;
            PlaySelectedItemsCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
