using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public interface IPlaceableContext
    {
        Point OffsetPoint
        {
            get;
            set;
        }
    }
}
