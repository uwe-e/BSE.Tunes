using BSE.Tunes.Data.Audio;
using BSE.Tunes.StoreApp.Managers;
using BSE.Tunes.StoreApp.Mvvm;
using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlayerBarViewModel : ViewModelBase
    {
        #region FieldsPrivate
        private PlayerManager m_playerManager;
        private IDialogService m_dialogService;
        private ICommand m_playCommand;
        private RelayCommand m_previousTrackCommand;
        private ICommand m_stopCommand;
        private RelayCommand m_nextTrackCommand;
        private PlayerState m_playerState = PlayerState.Closed;
        #endregion

        #region MethodsPublic
        #region Properties
        public PlayerState PlayerState
        {
            get
            {
                return this.m_playerState;
            }
            set
            {
                this.m_playerState = value;
                RaisePropertyChanged("PlayerState");
            }
        }
        public ICommand PlayCommand
        {
            get
            {
                return this.m_playCommand ??
                    (this.m_playCommand = new RelayCommand<object>(vm => Play()));
            }
        }
        public ICommand StopCommand
        {
            get
            {
                return this.m_stopCommand ??
                    (this.m_stopCommand = new RelayCommand<object>(vm => this.m_playerManager.Stop()));
            }
        }
        public RelayCommand PreviousTrackCommand
        {
            get
            {
                return this.m_previousTrackCommand ??
                    (this.m_previousTrackCommand = new RelayCommand(this.ExecutePreviousTrack, this.CanExecutePreviousTrack));
            }
        }
        public RelayCommand NextTrackCommand
        {
            get
            {
                return this.m_nextTrackCommand ??
                    (this.m_nextTrackCommand = new RelayCommand(this.ExecuteNextTrack, this.CanExecuteNextTrack));
            }
        }
        #endregion

        public PlayerBarViewModel()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                m_playerManager = PlayerManager.Instance;
                m_dialogService = DialogService.Instance;
            }
        }
        
        #endregion

        #region MethodsPrivate
        private void Play()
        {
            try
            {
                switch (this.PlayerState)
                {
                    case Data.Audio.PlayerState.Stopped:
                    case Data.Audio.PlayerState.Closed:
                        if (this.m_playerManager.CanExecutePlay())
                        {
                            this.m_playerManager.ReplayPlayTracks();
                        }
                        break;
                    case Data.Audio.PlayerState.Playing:
                        this.m_playerManager.Pause();
                        break;
                    case Data.Audio.PlayerState.Paused:
                        this.m_playerManager.Play();
                        break;
                }
            }
            catch (Exception exception)
            {
                m_dialogService.ShowAsync(exception.Message);
            }
        }
        private bool CanExecutePreviousTrack()
        {
            //return !this.m_trackCommandExecuted && this.m_playerManager.CanExecutePreviousTrack();
            return this.m_playerManager.CanExecutePreviousTrack();
        }
        private void ExecutePreviousTrack()
        {
            //this.m_trackCommandExecuted = true;
            this.PreviousTrackCommand.RaiseCanExecuteChanged();
            this.NextTrackCommand.RaiseCanExecuteChanged();
            this.m_playerManager.ExecutePreviousTrack();
        }
        private bool CanExecuteNextTrack()
        {
            //return !this.m_trackCommandExecuted && this.m_playerManager.CanExecuteNextTrack();
            return this.m_playerManager.CanExecuteNextTrack();
        }
        private void ExecuteNextTrack()
        {
            //this.m_trackCommandExecuted = true;
            this.PreviousTrackCommand.RaiseCanExecuteChanged();
            this.NextTrackCommand.RaiseCanExecuteChanged();
            this.m_playerManager.ExecuteNextTrack();
        }
        #endregion
    }
}
