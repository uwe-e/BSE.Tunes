using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SearchResultTracksUserControlViewModel : FeaturedItemsBaseViewModel
    {
        private Query m_query;

        public Query Query
        {
            get
            {
                return m_query;
            }
            set
            {
                m_query = value;
                RaisePropertyChanged(() => Query);
            }
        }

        public SearchResultTracksUserControlViewModel()
        {
        }
        public SearchResultTracksUserControlViewModel(Query query) : base()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Query = query;
                LoadData();
            }
        }
        public async override void LoadData()
        {
            if (Query != null)
            {
                var tracks = await DataService.GetTrackSearchResults(Query);
                if (tracks != null)
                {
                    foreach (var track in tracks)
                    {
                        if (track != null)
                        {
                            Items.Add(new GridPanelItemViewModel
                            {
                                Data = track
                            });
                        }
                    }
                }
            }
        }
    }
}
