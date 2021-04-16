using System;

namespace BSE.Tunes.WebApi.Identity
{
    public interface IRefreshToken
    {
        string UserName { get; set; }
        string SubjectId { get; set; }
        DateTime CreationTime { get; set; }
        int LifeTime { get; set;  }
    }
}
