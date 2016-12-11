using BSE.Tunes.StoreApp.Models;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Mvvm.Messaging
{
    public class PlayerStateChangedArgs : MessageBase
    {
        public PlayerState PlayerState
        {
            get; set;
        }
        public PlayerStateChangedArgs(PlayerState playerState)
        {
            PlayerState = playerState;
        }
    }
}
