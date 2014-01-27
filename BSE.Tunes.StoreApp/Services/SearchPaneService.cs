using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;

namespace BSE.Tunes.StoreApp.Services
{
    public class SearchPaneService : ISearchPaneService
    {
        /// <summary>
        /// Shows the Search Pane.
        /// </summary>
        public void Show()
        {
            SearchPane.GetForCurrentView().Show();
        }
    }
}
