using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class AboutItemViewModel : SettingsItemViewModel
    {
        public override string Icon => "Help";
        public override string Description => string.Empty;
        public override string Title => ResourceService.GetString("IDS_AboutPageHeader", "Info");
        public override void Navigate() => NavigationService.NavigateAsync(typeof(Views.AboutPage));
    }
}
