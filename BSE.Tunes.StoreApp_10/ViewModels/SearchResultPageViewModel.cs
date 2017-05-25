using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultPageViewModel : SearchSuggestionsViewModel
    {
        #region FieldsPrivate
        private SearchResultAlbumsUserControlViewModel m_searchResultAlbumsUserControlViewModel;
        private SearchResultTracksUserControlViewModel m_searchResultTrackssUserControlViewModel;
        private string m_headerText;
        private string m_pageHeaderText;
        #endregion

        #region Properties
        public string HeaderText
        {
            get
            {
                return m_headerText;
            }
            set
            {
                m_headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }
        public string PageHeaderText
        {
            get
            {
                return m_pageHeaderText;
            }
            set
            {
                m_pageHeaderText = value;
                RaisePropertyChanged(() => PageHeaderText);
            }
        }
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

        public SearchResultTracksUserControlViewModel TracksResult
        {
            get
            {
                return m_searchResultTrackssUserControlViewModel;
            }
            set
            {
                m_searchResultTrackssUserControlViewModel = value;
                RaisePropertyChanged(() => TracksResult);
            }
        }
        #endregion

        #region MethodsPublic
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            QueryText = parameter as string;
            HeaderText = string.Format(CultureInfo.InvariantCulture, "\"{0}\"", QueryText);
            PageHeaderText = string.Format(CultureInfo.CurrentUICulture, ResourceService.GetString("SearchResultPage_PageHeaderText"), QueryText);

            if (!string.IsNullOrEmpty(QueryText))
            {
                AlbumsResult = new SearchResultAlbumsUserControlViewModel(new Query
                {
                    SearchPhrase = QueryText,
                    PageSize = 9
                });

                TracksResult = new SearchResultTracksUserControlViewModel(new Query
                {
                    SearchPhrase = QueryText,
                    PageSize = 9
                });
            }
        }
        #endregion
    }
}
