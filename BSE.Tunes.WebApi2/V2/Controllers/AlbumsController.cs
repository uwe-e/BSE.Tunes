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
    //Microsoft REST API Guidelines
    //https://github.com/Microsoft/api-guidelines/blob/vNext/Guidelines.md
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/albums")]
    [Authorize(Roles = "tunesusers")]
    public class AlbumsController : BaseApiController
    {
        //http://bse.tunes.webapi/api/v2/albums/1132
        [Route("{albumId:int}")]
        public Album GetAlbumById(int albumId)
        {
            return this.TunesService.GetAlbumById(albumId);
        }
        
        //http://bse.tunes.webapi/api/v2/albums
        [Route("")]
        public Album[] GetAlbums([FromUri] int skip = 0, int limit= 10)
        {
            return TunesService.GetAlbums(null, null, skip, limit);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/count
        [Route("count")]
        public int GetNumberOfAlbums()
        {
            return TunesService.GetNumberOfAlbums(null, null);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/1132/tracks
        [Route("{albumId:int}/tracks")]
        public Track[] GetTracksByAlbumId(int albumId)
        {
            return TunesService.GetTracksByAlbumId(albumId);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/genre/1/?skip=0&limit=10
        [Route("genre/{genreId:int}")]
        public Album[] GetAlbums(int genreId, [FromUri] int skip = 0, int limit = 10)
        {
            return TunesService.GetAlbums(genreId, null, skip, limit);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/genre/1/count
        [Route("genre/{genreId:int}/count")]
        public int GetNumberOfAlbums(int genreId)
        {
            return this.TunesService.GetNumberOfAlbums(genreId, null);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/featured/?limit=10
        [Route("featured")]
        public Album[] GetFeaturesAlbums([FromUri] int limit = 10)
        {
            return this.TunesService.GetFeaturedAlbums(limit);
        }
        
        //http://bse.tunes.webapi/api/v2/albums/newest/?limit=10
        [Route("newest")]
        public Album[] GetNewestAlbums([FromUri] int limit = 10)
        {
            return this.TunesService.GetNewestAlbums(limit);
        }
    }
}
