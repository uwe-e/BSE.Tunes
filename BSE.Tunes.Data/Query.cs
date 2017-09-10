using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class Query
    {
        [DataMember]
        public string SearchPhrase
        {
            get;
            set;
        }
        [DataMember]
        public SortOrder SortByCondition
        {
            get;
            set;
        }
        [DataMember]
        public Genre GenreCondition
        {
            get;
            set;
        }
        [DataMember]
        public int PageIndex
        {
            get;
            set;
        }
        [DataMember]
        public int PageSize
        {
            get;
            set;
        }
        [DataMember]
        public dynamic Data
        {
            get;
            set;
        }
    }
}
