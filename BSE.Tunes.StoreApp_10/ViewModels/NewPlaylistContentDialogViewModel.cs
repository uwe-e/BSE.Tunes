using BSE.Tunes.Data;
using BSE.Tunes.Data.Exceptions;
using BSE.Tunes.StoreApp.Models;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class NewPlaylistContentDialogViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private ICommand m_savePlaylistCommand;
        private bool m_cancel;
        private string m_playlistName;
        private string m_errorMessage;
        #endregion

        #region Properties
        public ICommand SavePlaylistCommand => m_savePlaylistCommand ?? (m_savePlaylistCommand = new RelayCommand<object>(SavePlaylist));

        public bool Cancel
        {
            get
            {
                return m_cancel;
            }
            set
            {
                m_cancel = value;
                RaisePropertyChanged("Cancel");
            }
        }
        public string PlaylistName
        {
            get
            {
                return m_playlistName;
            }
            set
            {
                m_playlistName = value;
                RaisePropertyChanged("PlaylistName");
            }
        }
        public string ErrorMessage
        {
            get
            {
                return m_errorMessage;
            }
            set
            {
                m_errorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }
        public InsertMode InsertMode
        {
            get;
            set;
        } = InsertMode.All;
        #endregion

        #region MethodsPrivate
        private void SavePlaylist(object obj)
        {
            Cancel = !ValidateDialog();
        }
        private bool ValidateDialog()
        {
            bool isValid = false;
            User user = SettingsService.Instance.User;
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
                    Task<Playlist> task = Task.Run(async () => await DataService.InsertPlaylist(playlist));
                    try
                    {
                        playlist = task.Result;
                        Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistCreatedArgs(playlist, InsertMode));
                        isValid = true;

                    }
                    catch (AggregateException aggregateException)
                    {
                        var nameExistsException = aggregateException.InnerExceptions.Where(ex => ex is PlaylistExistsException).FirstOrDefault();
                        if (nameExistsException != null)
                        {
                            ErrorMessage = string.Format(CultureInfo.CurrentCulture, ResourceService.GetString("NewPlaylistContentDialog_PlayListAlreadyExistsExceptionMessage"), PlaylistName);
                        }
                        else
                        {
                            throw aggregateException;
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.ErrorMessage = exception.Message;
                }
            }
            return isValid;
        }
        #endregion
    }
}
