using BSE.Tunes.Data;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Mvvm.Messaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class DeletePlaylistContentDialogViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private Playlist m_playlist;
        private ICommand m_deletePlaylistCommand;
        private bool m_cancel;
        private string m_errorMessage;
        #endregion

        #region Properties

        public Playlist Playlist
        {
            get
            {
                return m_playlist;
            }
            set
            {
                m_playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }
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
        public ICommand DeletePlaylistCommand => m_deletePlaylistCommand ?? (m_deletePlaylistCommand = new RelayCommand(DeletePlaylist));
        #endregion

        #region MethodsPrivate
        private void DeletePlaylist()
        {
            Cancel = !ValidateDialog();
        }
        private bool ValidateDialog()
        {
            bool isValid = false;
            if (Playlist != null)
            {
                try
                {
                    System.Collections.ObjectModel.ObservableCollection<Playlist> playlists = new System.Collections.ObjectModel.ObservableCollection<Playlist>();
                    playlists.Add(Playlist);
                    Task<bool> task = Task.Run(async () => await DataService.DeletePlaylists(playlists));
                    try
                    {
                        isValid = task.Result;
                        Messenger.Default.Send<PlaylistChangedArgs>(new PlaylistDeletedArgs(null));
                    }
                    catch (AggregateException aggregateException)
                    {
                        //var nameExistsException = aggregateException.InnerExceptions.Where(ex => ex is PlaylistExistsException).FirstOrDefault();
                        //if (nameExistsException != null)
                        //{
                        //    ErrorMessage = string.Format(CultureInfo.CurrentCulture, ResourceService.GetString("NewPlaylistContentDialog_PlayListAlreadyExistsExceptionMessage"), PlaylistName);
                        //}
                        //else
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
