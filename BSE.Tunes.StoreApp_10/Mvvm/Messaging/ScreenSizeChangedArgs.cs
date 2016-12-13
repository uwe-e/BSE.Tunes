using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class ScreenSizeChangedArgs : MessageBase
    {
        public bool IsFullScreen
        {
            get; set;
        }
        public ScreenSizeChangedArgs(bool isFullScreen)
        {
            IsFullScreen = isFullScreen;
        }
    }
}
