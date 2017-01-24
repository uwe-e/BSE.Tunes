using BSE.Tunes.StoreApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace BSE.Tunes.StoreApp.Mvvm
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, Template10.Services.NavigationService.INavigable
    {
        #region Properties
        [JsonIgnore]
        public Template10.Services.NavigationService.INavigationService NavigationService
        {
            get; set;
        }

        [JsonIgnore]
        public Template10.Common.IDispatcherWrapper Dispatcher
        {
            get; set;
        }

        [JsonIgnore]
        public Template10.Common.IStateItems SessionState
        {
            get; set;
        }
        [JsonIgnore]
        public IResourceService ResourceService
        {
            get
            {
                return BSE.Tunes.StoreApp.Services.ResourceService.Instance;
            }
        }
        [JsonIgnore]
        public IDataService DataService
        {
            get
            {
                return BSE.Tunes.StoreApp.Services.DataService.Instance;
            }
        }
        #endregion

        #region MethodsPublic
        public ViewModelBase()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                NavigationService = NavigationService ?? Template10.Common.WindowWrapper.Current().NavigationServices.FirstOrDefault();
            }
        }
        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnNavigatingFromAsync(Template10.Services.NavigationService.NavigatingEventArgs args)
        {
            return Task.CompletedTask;
        }
        #endregion


    }
}
