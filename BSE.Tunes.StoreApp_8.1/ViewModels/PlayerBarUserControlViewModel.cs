using BSE.Tunes.Data;
using BSE.Tunes.Data.Collections;
using BSE.Tunes.Data.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using BSE.Tunes.Data.Audio;
using GalaSoft.MvvmLight.Command;
using System.Globalization;
using BSE.Tunes.StoreApp.Services;
using BSE.Tunes.StoreApp.Managers;
using GalaSoft.MvvmLight.Messaging;
using BSE.Tunes.StoreApp.Messaging;
using BSE.Tunes.StoreApp.Extensions;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class PlayerBarUserControlViewModel : PlayerUserControlViewModel
    {
        #region MethodsPublic
        public PlayerBarUserControlViewModel(IDataService dataService, IAccountService accountService, IDialogService dialogService, IResourceService resourceService, PlayerManager playerManager, INavigationService navigationService, ICacheableBitmapService cacheableBitmapService): base(dataService,accountService,dialogService,resourceService,playerManager,navigationService,cacheableBitmapService)
        {
        }
        #endregion
    }
}