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
    
    public partial class albums
    {
        public int Artist_Id { get; set; }
        public string Artist_Name { get; set; }
        public string Artist_SortName { get; set; }
        public int Album_Id { get; set; }
        public string Album_Title { get; set; }
        public string Album_AlbumId { get; set; }
        public Nullable<int> Album_Year { get; set; }
        public int Genre_Id { get; set; }
        public string Genre_Name { get; set; }
    }
}