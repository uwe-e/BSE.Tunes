using CommonServiceLocator;
using Windows.ApplicationModel.Resources;

namespace BSE.Tunes.StoreApp.Services
{
    public class ResourceService : IResourceService
    {
        #region FieldsPrivate
        private ResourceLoader m_resourceLoader;
        #endregion

        #region MethodsPublic
        public static IResourceService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IResourceService>() as ResourceService;
            }
        }
        public ResourceService()
        {
            this.m_resourceLoader = new ResourceLoader();
        }
        public string GetString(string key)
        {
            return this.m_resourceLoader.GetString(key);
        }
        public string GetString(string key, string defaultValue)
        {
            string strValue = this.GetString(key);
            if (string.IsNullOrEmpty(strValue) == true)
            {
                strValue = defaultValue;
            }
            return strValue;
        }
        #endregion
    }
}
