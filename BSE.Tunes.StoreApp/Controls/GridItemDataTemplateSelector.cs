using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BSE.Tunes.StoreApp.Extensions;

namespace BSE.Tunes.StoreApp.Controls
{
    public class GridItemDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            BSE.Tunes.StoreApp.Attributes.DataTemplateName dataTemplateName = item.GetCustomAttribute<BSE.Tunes.StoreApp.Attributes.DataTemplateName>();
            if (dataTemplateName != null)
            {
                string strDataTemplate = dataTemplateName.TemplateName;
                if (string.IsNullOrEmpty(strDataTemplate) == false)
                {
                    object resourceObject = null;
                    Page page = container.FindParentPage() as Page;
                    if (page != null && page.Resources != null)
                    {
                        if (page.Resources.ContainsKey(strDataTemplate))
                        {
                            resourceObject = page.Resources[strDataTemplate];
                        }
                    }
                    if (resourceObject == null)
                    {
                        resourceObject = Application.Current.Resources[strDataTemplate];
                    }
                    if (resourceObject != null)
                    {
                        return resourceObject as DataTemplate;
                    }
                }
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
