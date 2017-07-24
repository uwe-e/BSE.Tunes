using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace BSE.Tunes.StoreApp.Controls
{
    public class ListView : Windows.UI.Xaml.Controls.ListView
    {
        public static readonly DependencyProperty AlternatingRowProperty =
            DependencyProperty.Register("AlternatingRow", typeof(Brush), typeof(ListView),
                new PropertyMetadata(null, AlternatingRowChanged));

        public Brush AlternatingRow
        {
            get
            {
                return (Brush)GetValue(AlternatingRowProperty);
            }
            set
            {
                SetValue(AlternatingRowProperty, value);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (AlternatingRow != null)
            {
                foreach (var itm in Items)
                {
                    var index = Items.IndexOf(itm);
                    ListViewItem item = this.ContainerFromIndex(index) as ListViewItem;
                    SetAlternatingBackground(item, index);
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            int index = IndexFromContainer(element);
            SetAlternatingBackground(element, index);
        }

        private void SetAlternatingBackground(DependencyObject element, int index)
        {
            if (AlternatingRow != null)
            {
                ListViewItem lvi = element as ListViewItem;
                if (index % 2 == 0)
                {
                    lvi.Background = AlternatingRow;
                }
                else
                {
                    lvi.Background = Background;
                }
            }
        }

        private static void AlternatingRowChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ListView control = dependencyObject as ListView;
            if (control != null)
            {
                control.AlternatingRow = dependencyPropertyChangedEventArgs.NewValue as Brush;
            }
        }
    }
}
