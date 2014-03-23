using BSE.Tunes.Data;
using BSE.Tunes.Data.Exceptions;
using BSE.Tunes.StoreApp.Event;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class NewPlaylistUserControlViewModel : ViewModelBase
    {
        #region Events
        public event EventHandler<PlaylistChangedEventArgs> PlaylistInserted;
        #endregion

        #region FieldsPrivate
        private bool m_isOpen;
        private IDataService m_dataService;
        private IAccountService m_accountService;
        private IResourceService m_resourceService;
        private string m_playlistName;
        private ICommand m_saveNewPlaylistCommand;
        private string m_errorMessage;
        #endregion

        #region Properties
        public bool IsOpen
        {
            get
            {
                return this.m_isOpen;
            }
            set
            {
                this.m_isOpen = value;
                RaisePropertyChanged("IsOpen");
            }
        }
        public string PlaylistName
        {
            get
            {
                return this.m_playlistName;
            }
            set
            {
                this.m_playlistName = value;
                RaisePropertyChanged(PlaylistName);
            }
        }
        public ICommand SaveNewPlaylistCommand
        {
            get
            {
                return this.m_saveNewPlaylistCommand ??
                    (this.m_saveNewPlaylistCommand = new RelayCommand(this.SaveNewPlaylist));
            }
        }
        public string WatermarkText
        {
            get
            {
                return this.m_resourceService.GetString("IDS_NewPlaylistDialog_WatermarkText", string.Empty);
            }
        }
        public string ErrorMessage
        {
            get
            {
                return this.m_errorMessage;
            }
            set
            {
                this.m_errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        #endregion

        #region MethodsPublic
        public NewPlaylistUserControlViewModel(IDataService dataService, IAccountService accountService, IResourceService resourceService)
        {
            this.m_dataService = dataService;
            this.m_accountService = accountService;
            this.m_resourceService = resourceService;
            this.IsOpen = false;
        }
        #endregion

        #region MethodsPrivate
        private void SaveNewPlaylist()
        {
            if (!string.IsNullOrEmpty(this.PlaylistName))
            {
                this.ErrorMessage = string.Empty;
                TunesUser user = this.m_accountService.User;
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    Playlist playlist = new Playlist
                    {
                        Name = this.PlaylistName,
                        UserName = user.UserName,
                        Guid = Guid.NewGuid()
                    };
                    try
                    {
                        var task = Task.Run(async () => await this.m_dataService.InsertPlaylist(playlist));
                        task.Wait();
                        this.ErrorMessage = string.Empty;
                        this.PlaylistName = string.Empty;
                        if (this.PlaylistInserted != null)
                        {
                            this.PlaylistInserted(this, new PlaylistChangedEventArgs(task.Result));
                        }
                        this.IsOpen = false;
                    }
                    catch (AggregateException aggregateException)
                    {
                        foreach (var innerException in aggregateException.Flatten().InnerExceptions)
                        {
                            PlaylistExistsException playlistExistsException = innerException as PlaylistExistsException;
                            if (playlistExistsException != null)
                            {
                                this.ErrorMessage = string.Format(CultureInfo.CurrentCulture, this.m_resourceService.GetString("IDS_NewPlaylistDialog_PlayListAlreadyExistsExceptionMessage"), this.PlaylistName);
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
