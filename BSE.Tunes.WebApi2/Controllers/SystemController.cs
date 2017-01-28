using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
    [RoutePrefix("api/system")]
    [Authorize(Roles = "tunesusers")]
    public class SystemController : BaseApiController
    {
        [Route("")]
        public SystemInfo Get()
        {
            return this.TunesService.GetSystemInfo();
        }
    }
}
