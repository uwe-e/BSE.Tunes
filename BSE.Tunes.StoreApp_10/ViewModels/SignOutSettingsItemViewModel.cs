using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class SignOutSettingsItemViewModel : SettingsItemViewModel
    {
        public override string Icon => "Contact";
        public override string Description => ResourceService.GetString("IDS_SignInSettingsDescription", "Your account at BSEtunes.");
        public override string Title => ResourceService.GetString("IDS_SignInSettingsHeader","Account Settings");
        public override void Navigate() => NavigationService.NavigateAsync(typeof(Views.SignOutSettingsPage));
    }
}
