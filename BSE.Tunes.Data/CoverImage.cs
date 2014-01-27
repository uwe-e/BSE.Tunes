using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BSE.Tunes.Data
{
    [DataContract]
    public class CoverImage
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public byte[] Thumbnail
        {
            get;
            set;
        }
        [DataMember]
        public byte[] Cover
        {
            get;
            set;
        }
    }
}
