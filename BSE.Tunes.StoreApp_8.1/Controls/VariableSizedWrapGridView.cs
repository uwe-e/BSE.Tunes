using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BSE.Tunes.StoreApp.Extensions;
using BSE.Tunes.StoreApp.Attributes;

namespace BSE.Tunes.StoreApp.Controls
{
    public class VariableSizedWrapGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            if (item != null)
            {
                int iColumnSpan = 1;
                GridItemColumnSpan gridItemColumnSpan = item.GetCustomAttribute<GridItemColumnSpan>();
                if (gridItemColumnSpan != null)
                {
                    iColumnSpan = gridItemColumnSpan.ColumnSpan;
                }
                int iRowSpan = 1;
                GridItemRowSpan gridItemRowSpan = item.GetCustomAttribute<GridItemRowSpan>();
                if (gridItemRowSpan != null)
                {
                    iRowSpan = gridItemRowSpan.RowSpan;
                }

                element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, iColumnSpan);
                element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.RowSpanProperty, iRowSpan);
            }

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
