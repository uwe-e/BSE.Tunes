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
    
    public partial class HistoryEntity
    {
        public int PlayID { get; set; }
        public int AppID { get; set; }
        public int TitelID { get; set; }
        public int LiedID { get; set; }
        public System.DateTime Zeit { get; set; }
        public string Interpret { get; set; }
        public string Titel { get; set; }
        public string Lied { get; set; }
        public string Benutzer { get; set; }
    }
}
