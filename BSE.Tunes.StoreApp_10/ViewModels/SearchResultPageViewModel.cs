using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultPageViewModel : SearchSuggestionsViewModel
    {
        private SearchResultAlbumsUserControlViewModel m_searchResultAlbumsUserControlViewModel;

        public SearchResultAlbumsUserControlViewModel AlbumsResult
        {
            get
            {
                return m_searchResultAlbumsUserControlViewModel;
            }
            set
            {
                m_searchResultAlbumsUserControlViewModel = value;
                RaisePropertyChanged(() => AlbumsResult);
            }
        }

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            QueryText = parameter as string;
            if (!string.IsNullOrEmpty(QueryText))
            {
                AlbumsResult = new SearchResultAlbumsUserControlViewModel(new Query
                {
                    SearchPhrase = QueryText,
                    PageSize = 9
                });
            }
        }
    }
}
