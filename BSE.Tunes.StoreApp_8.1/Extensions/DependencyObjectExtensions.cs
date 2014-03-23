using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BSE.Tunes.StoreApp.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static DependencyObject FindParentPage(this DependencyObject reference)
        {
            DependencyObject dependencyObject = null;
            if (reference != null)
            {
                var parent = VisualTreeHelper.GetParent(reference);
                if (parent != null)
                {
                    Page page = parent as Page;
                    if (page != null)
                    {
                        dependencyObject = page;
                    }
                    else
                    {
                        dependencyObject = parent.FindParentPage();
                    }
                }
            }
            return dependencyObject;
        }
    }
}
