using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.DataModel;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Views;
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
    public class AlbumsGroupUserControlViewModel : GroupUserControlViewModel
    {
        #region FieldsPrivate
        private readonly IDataService m_dataService;
        private INavigationService m_navigationService;
        private ICommand m_selectCommand;
        #endregion

        #region Properties
        public ICommand SelectCommand
        {
            get
            {
                return this.m_selectCommand ??
                    (this.m_selectCommand = new RelayCommand<DataItemViewModel>(this.SelectItem));
            }
        }
        #endregion

        #region MethodsPublic
        public AlbumsGroupUserControlViewModel(IDataService dataService, INavigationService navigationService)
        {
            this.m_dataService = dataService;
            this.m_navigationService = navigationService;
            this.DataGroup = new DataGroupViewModel();
            this.LoadData();
        }
        public override void OnSelectGroupHeader()
        {
            this.m_navigationService.Navigate(typeof(AlbumsPage));
        }
        #endregion

        #region MethodsPrivate
        private void SelectItem(DataItemViewModel dataItem)
        {
            if (dataItem != null)
            {
                Album album = dataItem.Data as Album;
                if (album != null)
                {
                    this.m_navigationService.Navigate(typeof(AlbumDetailPage), album.Id);
                }
            }
        }
        private async void LoadData()
        {
            this.IsBusy = true;
            var newestAlbums = await this.m_dataService.GetNewestAlbums(9);
            if (newestAlbums != null)
            {
                int iIndex = 0;
                foreach (var album in newestAlbums)
                {
                    if (album != null)
                    {
                        DataItemViewModel dataItem = new DataItemViewModel();
                        if (iIndex == 0)
                        {
                            dataItem = new BigDataItemViewModel();
                        }
                        iIndex++;
                        dataItem.Title = album.Title;
                        dataItem.Subtitle = album.Artist.Name;
                        dataItem.Image = album.Cover;
                        dataItem.Data = album;
                        this.DataGroup.Items.Add(dataItem);
                    }
                }
                this.IsBusy = false;
            }
        }
        #endregion
    }
}
