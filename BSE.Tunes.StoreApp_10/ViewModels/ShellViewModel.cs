using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private string m_searchText;
        private bool m_hasSuggestionChosen;
        private ObservableCollection<string> m_searchSuggestions;
        private ICommand m_suggestionChosenCommand;
        private ICommand m_querySubmittedCommand;
        #endregion
        
        #region Properties
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                m_searchText = value;
                RaisePropertyChanged(()=> SearchText);
            }
        }
        public ObservableCollection<String> SearchSuggestions => m_searchSuggestions ?? (m_searchSuggestions = new ObservableCollection<string>());
        public ICommand QuerySubmittedCommand => m_querySubmittedCommand ?? (m_querySubmittedCommand = new RelayCommand<string>(async (suggestion) =>
        {
            m_hasSuggestionChosen = false;
            await NavigationService.NavigateAsync(typeof(Views.SearchResultPage), suggestion);
        }));

        public ICommand SuggestionChosenCommand => m_suggestionChosenCommand ?? (m_suggestionChosenCommand = new RelayCommand(() =>
        {
            m_hasSuggestionChosen = true;
        }));
        
        #endregion

        #region MethodsPublic
        public async void LoadSuggestions()
        {
            if (!string.IsNullOrEmpty(SearchText) && SearchText.Length > 3)
            {
                if (!m_hasSuggestionChosen)
                {
                    SearchSuggestions.Clear();

                    var suggestions = await DataService.GetSearchSuggestions(new Query
                    {
                        SearchPhrase = SearchText
                    });
                    foreach (var suggestion in suggestions)
                    {
                        if (!string.IsNullOrEmpty(suggestion))
                        {
                            SearchSuggestions.Add(suggestion);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
