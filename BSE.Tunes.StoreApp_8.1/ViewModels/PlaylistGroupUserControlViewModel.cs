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
    public class PlaylistGroupUserControlViewModel : GroupUserControlViewModel
    {
        #region FieldsPrivate
        private IResourceService m_resourceService;
        private INavigationService m_navigationService;
        private IDataService m_dataService;
        private IAccountService m_accountService;
        private ICommand m_selectCommand;
        private ObservableCollection<PlaylistViewModel> m_playlists;
        #endregion

        #region Properties
        public ObservableCollection<PlaylistViewModel> Playlists
        {
            get
            {
                return this.m_playlists ??
                    (this.m_playlists = new ObservableCollection<PlaylistViewModel>());
            }
        }
        public ICommand SelectCommand
        {
            get
            {
                return this.m_selectCommand ??
                    (this.m_selectCommand = new RelayCommand<PlaylistViewModel>(this.SelectItem));
            }
        }
        #endregion

        #region MethodsPublic
        public PlaylistGroupUserControlViewModel(IDataService dataService, IAccountService accountService, INavigationService navigationService, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_accountService = accountService;
            this.m_navigationService = navigationService;
            this.m_resourceService = resourceService;
            this.LoadData();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<BSE.Tunes.StoreApp.Messaging.PlaylistChangeMessage>(this, message =>
            {
                this.LoadData();
            });
        }
        public override void OnSelectGroupHeader()
        {
            this.m_navigationService.Navigate(typeof(PlaylistPage));
        }
        #endregion

        #region MethodsPrivate
        private void LoadData()
        {
            TunesUser user = this.m_accountService.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                this.IsBusy = true;
                this.Playlists.Clear();
                try
                {
                    var playlists = Task.Run(async () => await this.m_dataService.GetPlaylistsByUserName(user.UserName, 4));
                    playlists.Wait();
                    foreach (var playlist in playlists.Result)
                    {
                        if (playlist != null)
                        {
                            this.Playlists.Add(new PlaylistViewModel(this.m_dataService, this.m_accountService, this.m_resourceService, playlist.Id));
                        }
                    }
                }
                catch (Exception exception)
                {
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }
        private void SelectItem(PlaylistViewModel playlistViewModel)
        {
            if (playlistViewModel != null && playlistViewModel.Playlist != null)
            {
                this.m_navigationService.Navigate(typeof(PlaylistDetailPage), playlistViewModel.Playlist.Id);
            }
        }
        #endregion
    }
}
