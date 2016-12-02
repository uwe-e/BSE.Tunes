using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SystemSettingsItemViewModel : SettingsItemViewModel
    {
        public override string Icon => "Setting";
        public override string Description => "The address of the webserver that contains the services";
        public override string Title => "Appearance";
        public override void Navigate() => NavigationService.NavigateAsync(typeof(Views.SettingsPage),0);
    }
}
