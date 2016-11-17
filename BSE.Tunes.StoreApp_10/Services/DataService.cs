using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public class DataService : IDataService
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        public DataService()
        {
            m_settingsService = SettingsService.Instance;
        }
        #endregion

        #region MethodsPublic

        public static IDataService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataService>() as DataService;
            }
        }
        public async Task<bool> IsHostAccessible()
        {
            bool isHostAccessible = false;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(string.Format("{0}/", m_settingsService.ServiceUrl));
                try
                {
                    HttpResponseMessage message = await httpClient.GetAsync("api/tunes/IsHostAccessible");
                    if (message.IsSuccessStatusCode)
                    {
                        isHostAccessible = message.Content.ReadAsAsync<bool>().Result;
                    }
                    else
                    {
                        throw new Exception(message.ReasonPhrase);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw exception;
                }
            }
            return isHostAccessible;
        }
        #endregion
    }
}
