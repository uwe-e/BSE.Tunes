using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class Filter
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public FilterMode Mode { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool IsUsed { get; set; }
        [DataMember]
        public string UserName { get; set; }

    }
}
