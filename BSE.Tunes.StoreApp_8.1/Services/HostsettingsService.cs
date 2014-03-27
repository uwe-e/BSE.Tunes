using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public class HostsettingsService : IHostsettingsService
    {
        #region Properties
        public string ServiceUrl
        {
            get
            {
				return (string)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["serviceurl"];
            }
        }
        #endregion

        #region MethodsPublic
        public void SetServiceUrl(string serviceUrl)
        {
            if (!string.IsNullOrEmpty(serviceUrl))
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["serviceurl"] = serviceUrl;
            }
        }
        #endregion
    }
}
