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
                return Windows.Storage.ApplicationData.Current.RoamingSettings.Values["serviceurl"] != null ?
                    (string)Windows.Storage.ApplicationData.Current.RoamingSettings.Values["serviceurl"] : "localhost";
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
