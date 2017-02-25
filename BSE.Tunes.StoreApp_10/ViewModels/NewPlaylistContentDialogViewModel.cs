using BSE.Tunes.StoreApp.Mvvm;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
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
        #endregion

        #region MethodsPrivate
        private void SavePlaylist(object obj)
        {
            Cancel = true;
            //Windows.UI.Xaml.Controls.ContentDialogButtonClickEventArgs args = obj as Windows.UI.Xaml.Controls.ContentDialogButtonClickEventArgs;
            //if (args != null)
            //{
            //    args.Cancel = true;
            //}
            
        }
        #endregion
    }
}
