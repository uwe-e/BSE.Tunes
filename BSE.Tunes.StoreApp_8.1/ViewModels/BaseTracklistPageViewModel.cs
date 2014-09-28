using BSE.Tunes.StoreApp.Interfaces;
using BSE.Tunes.StoreApp.Services;
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
    public class BaseTracklistPageViewModel : BasePlaylistableViewModel, INavigationAware
    {
        #region FieldsPrivate
        private bool m_hasSelectedItems;
        private bool m_isToolbarOpen;
        private INavigationService m_navigationService;
        private ObservableCollection<object> m_selectedItems;
        private RelayCommand m_playSelectedItemsCommand;
        private RelayCommand m_clearSelectionCommand;
        private ICommand m_goBackCommand;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets an value indication whether the tracklist has at the minimum one selected item.
        /// </summary>
        public virtual bool HasSelectedItems
        {
            get
            {
                return this.m_hasSelectedItems;
            }
            set
            {
                this.m_hasSelectedItems = value;
                try
                {
                    RaisePropertyChanged("HasSelectedItems");
                }
                catch { }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the the toolbar for the playlist related content is open.
        /// </summary>
        /// <remarks>Because it´s a twoway bound property in xaml the <see cref="HasSelectedItems"/> property can not used.</remarks>
        public virtual bool IsToolbarOpen
        {
            get
            {
                return this.m_isToolbarOpen;
            }
            set
            {
                this.m_isToolbarOpen = value;
                RaisePropertyChanged("IsToolbarOpen");
            }
        }
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return this.m_selectedItems;
            }
            set
            {
                this.m_selectedItems = value;
                RaisePropertyChanged("SelectedItems");
            }
        }
        public ICommand GoBackCommand
        {
            get
            {
                return this.m_goBackCommand ??
                    (this.m_goBackCommand = new RelayCommand(this.m_navigationService.GoBack));
            }
        }
        public RelayCommand PlaySelectedItemsCommand
        {
            get
            {
                return this.m_playSelectedItemsCommand ??
                    (this.m_playSelectedItemsCommand= new RelayCommand(this.PlaySelectedItems, this.CanPlaySelectedItems));
            }
        }
        public RelayCommand ClearSelectionCommand
        {
            get
            {
                return this.m_clearSelectionCommand ??
                    (this.m_clearSelectionCommand = new RelayCommand(ClearSelection, CanClearSelection));
            }
        }
        #endregion

        #region MethodsPublic
        public BaseTracklistPageViewModel(IDataService dataService, IAccountService accountService, IDialogService dialogService, IResourceService resourceService, ICacheableBitmapService cacheableBitmapService, INavigationService navigationService)
            : base(dataService, accountService, dialogService, resourceService, cacheableBitmapService)
        {
            this.m_navigationService = navigationService;
        }

        public virtual void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode)
        {
            if (navigationParameter is int)
            {
                this.SelectedItems = new ObservableCollection<object>();
                this.SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
        }
        
        public virtual void OnNavigatedFrom(bool suspending)
        {
            if (this.SelectedItems != null)
            {
                this.SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                this.SelectedItems = null;
            }
            this.HasSelectedItems = false;
            this.IsToolbarOpen = false;
        }
        #endregion

        #region MethodsProtected
        protected virtual void PlaySelectedItems()
        {
        }
        protected virtual void ClearSelection()
        {
            if (this.SelectedItems != null)
            {
                try
                {
                    this.SelectedItems.Clear();
                }
                catch (Exception exception)
                {
                    this.DialogService.ShowDialog(exception.Message);
                }
            }
        }
        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HasSelectedItems = this.SelectedItems.Count > 0;
            this.IsToolbarOpen = this.HasSelectedItems;
            this.PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region MethodsPrivate
        private bool CanPlaySelectedItems()
        {
            return this.SelectedItems != null && this.SelectedItems.Count > 0;
        }
        private bool CanClearSelection()
        {
            return this.SelectedItems != null && this.SelectedItems.Count > 0;
        }
        
        #endregion
    }
}
