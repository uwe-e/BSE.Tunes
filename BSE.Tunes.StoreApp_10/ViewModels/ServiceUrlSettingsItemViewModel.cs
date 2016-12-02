using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ServiceUrlSettingsItemViewModel : SettingsItemViewModel
    {
        public override string Icon => "World";
        public override string Description => ResourceService.GetString("IDS_ServiceUrlSettingsDescription", "The address of the webserver that contains the services");
        public override string Title => ResourceService.GetString("IDS_ServiceUrlSettingsHeader", "Service Address");
        public override void Navigate() => NavigationService.NavigateAsync(typeof(Views.ServiceUrlSettingsPage));
    }
}
