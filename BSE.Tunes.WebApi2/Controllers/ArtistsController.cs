using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
    [RoutePrefix("api/artists")]
    [Authorize(Roles = "tunesusers")]
    public class ArtistsController : BaseApiController
    {
        [HttpPost]
        [Route("albums")]
        public Album[] GetAlbums([FromBody] Query query)
        {
            return TunesService.GetAlbumsByArtist(query);
        }
        [Route("albums/{artistId:int}/numberofalbums")]
        public int GetNumberOfAlbumsByArtist(int artistId)
        {
            return this.TunesService.GetNumberOfAlbumsByArtist(artistId);
        }
    }
}
