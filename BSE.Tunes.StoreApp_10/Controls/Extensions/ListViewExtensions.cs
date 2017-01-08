using System.Collections.Specialized;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BSE.Tunes.StoreApp.Controls.Extensions
{
    /// <summary>
    /// Extension methods and attached properties for the ListViewBase class.
    /// </summary>
    public static class ListViewExtensions
    {
        #region ItemClick Command
        /// <summary>
        /// Attached Command Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
            typeof(ICommand),
            typeof(ListViewExtensions),
            new PropertyMetadata(null, OnCommandChanged));
        /// <summary>
        /// Gets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The invoked command.</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }
        /// <summary>
        /// Sets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">The command.</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }
        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
            {
                control.ItemClick += OnItemClick;
            }
        }
        /// <summary>
        /// Occurs when an item in the list view receives an interaction, and the IsItemClickEnabled property is true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = sender as ListViewBase;
            if (control != null)
            {
                var command = GetCommand(control);

                if (command != null && command.CanExecute(e.ClickedItem))
                    command.Execute(e.ClickedItem);
            }
        }
        #endregion

        #region RightTapped Command
        /// <summary>
        /// Attached Command Property
        /// </summary>
        public static readonly DependencyProperty RightTappedCommandProperty =
            DependencyProperty.RegisterAttached("RightTappedCommand",
            typeof(ICommand),
            typeof(ListViewExtensions),
            new PropertyMetadata(null, OnRightTappedCommandChanged));
        /// <summary>
        /// Gets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The invoked command.</returns>
        public static ICommand GetRightTappedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(RightTappedCommandProperty);
        }
        /// <summary>
        /// Sets the command to invoke when an item is clicked.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value">The command.</param>
        public static void SetRightTappedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(RightTappedCommandProperty, value);
        }
        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property has changed value.</param>
        /// <param name="e">Event data that is issued by any event that tracks changes to the effective value of this property.</param>
        private static void OnRightTappedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
            {
                control.RightTapped += OnRightTapped;
            }
        }
        /// <summary>
        /// Occurs when an item in the list view receives an interaction, and the IsItemClickEnabled property is true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var control = sender as ListViewBase;
            if (control != null)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (element != null && element.DataContext != null)
                {
                    var command = GetRightTappedCommand(control);
                    command?.Execute(element.DataContext);
                }
            }
        }
        #endregion

        //#region ItemToBringIntoView
        ///// <summary>
        ///// ItemToBringIntoView Attached Dependency Property
        ///// </summary>
        //public static readonly DependencyProperty ItemToBringIntoViewProperty =
        //    DependencyProperty.RegisterAttached(
        //        "ItemToBringIntoView",
        //        typeof (object),
        //        typeof (ListViewExtensions),
        //        new PropertyMetadata(null, OnItemToBringIntoViewChanged));

        ///// <summary>
        ///// Gets the ItemToBringIntoView property. This dependency property 
        ///// indicates the item that should be brought into view.
        ///// </summary>
        //public static object GetItemToBringIntoView(DependencyObject d)
        //{
        //    return (object)d.GetValue(ItemToBringIntoViewProperty);
        //}

        ///// <summary>
        ///// Sets the ItemToBringIntoView property. This dependency property 
        ///// indicates the item that should be brought into view when first set.
        ///// </summary>
        //public static void SetItemToBringIntoView(DependencyObject d, object value)
        //{
        //    d.SetValue(ItemToBringIntoViewProperty, value);
        //}

        ///// <summary>
        ///// Handles changes to the ItemToBringIntoView property.
        ///// </summary>
        ///// <param name="d">
        ///// The <see cref="DependencyObject"/> on which
        ///// the property has changed value.
        ///// </param>
        ///// <param name="e">
        ///// Event data that is issued by any event that
        ///// tracks changes to the effective value of this property.
        ///// </param>
        //private static void OnItemToBringIntoViewChanged(
        //    DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    object newItemToBringIntoView =
        //        (object)d.GetValue(ItemToBringIntoViewProperty);

        //    if (newItemToBringIntoView != null)
        //    {
        //        var listView = (ListView)d;
        //        listView.ScrollIntoView(newItemToBringIntoView);
        //    }
        //}
        //#endregion

        ///// <summary>
        ///// Scrolls a vertical ListView to the bottom.
        ///// </summary>
        ///// <param name="listView"></param>
        //public static void ScrollToBottom(this ListView listView)
        //{
        //    var scrollViewer = listView.GetFirstDescendantOfType<ScrollViewer>();
        //    scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
        //}
    }

    public class ListViewBindableSelectionHandler
    {
        private ListViewBase _listView;
        private dynamic _boundSelection;
        private readonly NotifyCollectionChangedEventHandler _handler;

        public ListViewBindableSelectionHandler(
            ListViewBase listView, dynamic boundSelection)
        {
            _handler = OnBoundSelectionChanged;
            Attach(listView, boundSelection);
        }

        private void Attach(ListViewBase listView, dynamic boundSelection)
        {
            _listView = listView;
            _listView.SelectionChanged += OnListViewSelectionChanged;
            _boundSelection = boundSelection;
            _listView.SelectedItems.Clear();

            foreach (object item in _boundSelection)
            {
                if (!_listView.SelectedItems.Contains(item))
                {
                    _listView.SelectedItems.Add(item);
                }
            }

            var eventInfo =
                _boundSelection.GetType().GetDeclaredEvent("CollectionChanged");
            eventInfo.AddEventHandler(_boundSelection, _handler);
            //_boundSelection.CollectionChanged += OnBoundSelectionChanged;
        }

        private void OnListViewSelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            foreach (dynamic item in e.RemovedItems)
            {
                if (_boundSelection.Contains(item))
                {
                    _boundSelection.Remove(item);
                }
            }

            foreach (dynamic item in e.AddedItems)
            {
                if (!_boundSelection.Contains(item))
                {
                    _boundSelection.Add(item);
                }
            }
        }

        private void OnBoundSelectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action ==
                NotifyCollectionChangedAction.Reset)
            {
                _listView.SelectedItems.Clear();

                foreach (var item in _boundSelection)
                {
                    if (!_listView.SelectedItems.Contains(item))
                    {
                        _listView.SelectedItems.Add(item);
                    }
                }

                return;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (_listView.SelectedItems.Contains(item))
                    {
                        _listView.SelectedItems.Remove(item);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (!_listView.SelectedItems.Contains(item))
                    {
                        _listView.SelectedItems.Add(item);
                    }
                }
            }
        }

        internal void Detach()
        {
            _listView.SelectionChanged -= OnListViewSelectionChanged;
            _listView = null;
            var eventInfo =
                _boundSelection.GetType().GetDeclaredEvent("CollectionChanged");
            eventInfo.RemoveEventHandler(_boundSelection, _handler);
            _boundSelection = null;
        }
    }
}