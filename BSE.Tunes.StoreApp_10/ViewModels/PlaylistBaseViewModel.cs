using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Collections.Specialized;
using Template10.Services.NavigationService;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistBaseViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private bool m_hasSelectedItems;
        private bool m_isToolbarOpen;
        private ObservableCollection<object> m_selectedItems;
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
                RaisePropertyChanged("HasSelectedItems");
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

        #endregion

        #region MethodsPublic
        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (this.SelectedItems == null)
            {
                this.SelectedItems = new ObservableCollection<object>();
                this.SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
            }
            return Task.CompletedTask;
        }
        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            if (this.SelectedItems != null)
            {
                this.SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                this.SelectedItems = null;
            }
            this.HasSelectedItems = false;
            this.IsToolbarOpen = false;
            return base.OnNavigatedFromAsync(state, suspending);
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
            this.HasSelectedItems = this.SelectedItems.Count > 0;
            this.IsToolbarOpen = this.HasSelectedItems;
            //this.PlaySelectedItemsCommand.RaiseCanExecuteChanged();
            //this.ClearSelectionCommand.RaiseCanExecuteChanged();
        }
        #endregion

    }
}
