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
        private ObservableCollection<string> m_searchSuggestions;
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


        public ICommand QuerySubmittedCommand => m_querySubmittedCommand ?? (m_querySubmittedCommand = new RelayCommand<string>(async (text) =>
        {
            await Task.Delay(500);
        }));

        public ICommand SuggestionChosenCommand => m_suggestionChosenCommand ?? (m_suggestionChosenCommand = new RelayCommand<string>(SelectSuggestion));
        private ICommand m_suggestionChosenCommand;
        private bool m_suggestionChosen;
        private ICommand m_querySubmittedCommand;

        public ObservableCollection<String> SearchSuggestions => m_searchSuggestions ?? (m_searchSuggestions = new ObservableCollection<string>());
        #endregion

        #region MethodsPublic
        public async void LoadSuggestions()
        {
            if (!string.IsNullOrEmpty(SearchText) && SearchText.Length > 3)
            {
                if (!m_suggestionChosen)
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
        public void QuerySubmitted()
        {

        }
        public void SuggestionChosen()
        {
            m_suggestionChosen = true;
        }
        public void SelectSuggestion(object obj)
        {

        }
        #endregion


    }
}
