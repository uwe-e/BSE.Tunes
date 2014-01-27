using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.DataModel
{
    public class SearchCategory
    {
        public FilterMode Mode
        {
            get;
            set;
        }
        public string Query
        {
            get;
            set;
        }
    }
}
