//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BSE.Tunes.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrackEntity
    {
        public int LiedID { get; set; }
        public int TitelID { get; set; }
        public Nullable<int> Track { get; set; }
        public string Lied { get; set; }
        public Nullable<System.DateTime> Dauer { get; set; }
        public string Liedpfad { get; set; }
        public string guid { get; set; }
        public System.DateTime Timestamp { get; set; }
    }
}
