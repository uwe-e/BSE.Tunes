using BSE.Tunes.Data;
using BSE.Tunes.WebApi.Controllers;
using Microsoft.Web.Http;
using System.Collections.Generic;
using System.Web.Http;

namespace BSE.Tunes.WebApi.V2.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/tracks")]
    [Authorize(Roles = "tunesusers")]
    public class TracksController : BaseApiController
    {
        [Route("{trackId:int}")]
        public Track GetTrackById(int trackId)
        {
            return TunesService.GetTrackById(trackId);
        }
        [Route("genre/{genreId:int}")]
        public ICollection<int> GetTrackIdsByGenre(int genreId)
        {
            return TunesService.GetTrackIdsByFilter(genreId);
        }
        [Route("top")]
        public ICollection<Track> GetTopTracks([FromUri] int skip = 0, int limit = 10)
        {
            return TunesService.GetTopTracks(skip, limit);
        }

    }
}