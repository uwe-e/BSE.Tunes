using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace BSE.Tunes.StoreApp.Collections
{
    /// <summary>
    /// Represents a dynamic data collection that supports  incremental loading and provides notifications when items
    /// get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class IncrementalObservableCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private uint m_maximumItems;
        private uint m_counter;
        private Func<uint, IAsyncOperation<LoadMoreItemsResult>> m_loadMoreItems;
        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalObservableCollection"/><T> class.
        /// </summary>
        /// <param name="maximumItems">The maximum number of items.</param>
        /// <param name="loadMoreItems">The encapsulated incremental load function.</param>
        public IncrementalObservableCollection(uint maximumItems, Func<uint, IAsyncOperation<LoadMoreItemsResult>> loadMoreItems)
        {
            this.m_maximumItems = maximumItems;
            this.m_loadMoreItems = loadMoreItems;
        }
        /// <summary>
        /// Gets a sentinel value that supports incremental loading implementations.
        /// returns <b>true</b> if additional unloaded items remain in the view; otherwise, <b>false</b>.
        /// </summary>
        public bool HasMoreItems
        {
            get
            {
                if (this.m_counter == 0 || this.m_counter < this.Count)
                {
                    this.m_counter = (uint)this.Count;
                }
                else
                {
                    return false;
                }
                return this.Count < this.m_maximumItems;
            }
        }
        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <returns>The wrapped results of the load operation.</returns>
        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync()
        {
            return LoadMoreItemsAsync(1);
        }
        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <param name="count">Initializes incremental loading from the view.</param>
        /// <returns>The wrapped results of the load operation.</returns>
        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return this.m_loadMoreItems(count);
        }
    }
}
