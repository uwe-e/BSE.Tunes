using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class ScreenChangedArgs : MessageBase
    {
        public bool IsFullScreen
        {
            get; set;
        }
        public ScreenChangedArgs(bool isFullScreen)
        {
            IsFullScreen = isFullScreen;
        }
    }
}
