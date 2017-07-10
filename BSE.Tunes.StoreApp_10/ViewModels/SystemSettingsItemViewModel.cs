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
        public override string Description => ResourceService.GetString("IDS_SystemSettingsDescription", "Colors");
        public override string Title => ResourceService.GetString("IDS_SystemSettingsHeader", "Personalization");
        public override void Navigate() => NavigationService.NavigateAsync(typeof(Views.SettingsPage),0);
    }
}
