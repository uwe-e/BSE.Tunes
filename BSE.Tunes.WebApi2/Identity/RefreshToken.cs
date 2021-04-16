using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE.Tunes.WebApi.Identity
{
    public class RefreshToken : IRefreshToken
    {
        public string Id { get; set; }
        public string UserName { get ; set; }
        public string SubjectId { get; set; }
        public DateTime CreationTime { get; set; }
        public int LifeTime { get; set; }
        public string SerializedTicket { get; set; }
    }
}