using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BSE.Tunes.StoreApp.Views
{
    public sealed partial class NewPlaylistUserControl : UserControl
    {
        public NewPlaylistUserControl()
        {
            this.InitializeComponent();
            this.PopupNewPlaylist.Opened += OnPopupOpened;
            this.PopupNewPlaylist.Closed += OnPopupClosed;
        }

        private void OnPopupClosed(object sender, object e)
        {
            Popup popup = sender as Popup;
            if (popup != null)
            {
                popup.Opacity = 0;
            }
        }

        private void OnPopupOpened(object sender, object e)
        {
            Popup popup = sender as Popup;
            if (popup != null)
            {
                if (popup.Parent != null)
                {
                    popup.Opacity = 1;
                    this.TxtPlaylistName.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                }
                else
                {
                    popup.Opacity = 0;
                }
            }
        }
    }
}
