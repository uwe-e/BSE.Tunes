using BSE.Tunes.Data;
using BSE.Tunes.WebApi.Controllers;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSE.Tunes.WebApi.V2.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/genres")]
    [Authorize(Roles = "tunesusers")]
    public class GenresController : BaseApiController
    {
        [Route("")]
        public Genre[] GetGenres()
        {
            return TunesService.GetGenres();
        }
    }
}
