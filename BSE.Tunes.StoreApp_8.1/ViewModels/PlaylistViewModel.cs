using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlaylistViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private IResourceService m_resourceService;
        private IDataService m_dataService;
        private IAccountService m_accountService;
        private Playlist m_playlist;
        private int m_playlistId;
        private string m_numbersOfEntries;
        #endregion

        #region Properties
        public int PlaylistId
        {
            get { return this.m_playlistId; }
            set
            {
                this.m_playlistId = value;
                RaisePropertyChanged("PlaylistId");
            }
        }
        public string NumbersOfEntries
        {
            get
            {
                return this.m_numbersOfEntries;
            }
            set
            {
                this.m_numbersOfEntries = value;
                RaisePropertyChanged("NumbersOfEntries");
            }
        }
        public Playlist Playlist
        {
            get
            {
                return this.m_playlist;
            }
            set
            {
                this.m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
        #endregion

        #region MethodsPublic
        public PlaylistViewModel(IDataService dataService, IAccountService accountService, IResourceService resourceService, int playlistId)
        {
            this.m_dataService = dataService;
            this.m_accountService = accountService;
            this.m_resourceService = resourceService;
            this.PlaylistId = playlistId;
            this.LoadData();
            Messenger.Default.Register<PlaylistEntryChangeMessage>(this, message =>
                {
                    Playlist changedPlaylist = message.Playlist;
                    if (changedPlaylist != null && changedPlaylist.Equals(this.Playlist))
                    {
                        this.LoadData();
                    }
                });
        }
        #endregion

        #region MethodsPrivate
        private void LoadData()
        {
            TunesUser user = this.m_accountService.User;
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                try
                {
                    var task = Task.Run(async () => await this.m_dataService.GetPlaylistByIdWithNumberOfEntries(this.PlaylistId, user.UserName));
                    task.Wait();

                    this.Playlist = task.Result;
                    this.FormatNumberOfEntriesString();
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void FormatNumberOfEntriesString()
        {
            int numberOfEntries = 0;
            if (this.Playlist != null)
            {
                numberOfEntries = this.Playlist.NumberEntries;
            }
            this.NumbersOfEntries = string.Format(CultureInfo.CurrentUICulture, "{0} {1}", numberOfEntries, this.m_resourceService.GetString("IDS_PlaylistItem_PartNumberOfEntries", "Songs"));
        }
        #endregion
    }
}
