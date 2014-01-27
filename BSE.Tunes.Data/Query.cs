using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Data
{
    public class Query
    {
        public string SearchPhrase
        {
            get;
            set;
        }
        public SortOrder SortByCondition
        {
            get;
            set;
        }
        public Genre GenreCondition
        {
            get;
            set;
        }
        public int PageIndex
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }
    }
}
