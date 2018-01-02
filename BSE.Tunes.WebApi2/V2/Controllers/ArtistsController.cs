using BSE.Tunes.Data;
using BSE.Tunes.WebApi.Controllers;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSE.Tunes.WebApi.V2.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/artists")]
    [Authorize(Roles = "tunesusers")]
    public class ArtistsController : BaseApiController
    {
        //http://bse.tunes.webapi/api/v2/artists/435/albums
        [Route("{artistId:int}/albums")]
        public Album[] GetAlbums(int artistId, [FromUri] int skip = 0, int limit = 10)
        {
            return TunesService.GetAlbums(null, artistId, skip, limit);
        }
        //http://bse.tunes.webapi/api/v2/artists/435/albums/count
        [Route("{artistId:int}/albums/count")]
        public int GetNumberOfAlbumsByArtist(int artistId)
        {
            return this.TunesService.GetNumberOfAlbums(null,artistId);
        }
    }
}