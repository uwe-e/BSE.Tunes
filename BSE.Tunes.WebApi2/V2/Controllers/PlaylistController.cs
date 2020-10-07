using BSE.Tunes.Data;
using BSE.Tunes.WebApi.Controllers;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BSE.Tunes.WebApi.V2.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/playlists")]
    [Authorize(Roles = "tunesusers")]
    public class PlaylistController : BaseApiController
    {
        [HttpPut]
        [Route("playlist/append")]
        public Playlist AppendToPlaylist([FromBody] Playlist playlist)
        {
            return TunesService.AppendToPlaylist(playlist);
        }

        [Route("{username}")]
        public Playlist[] GetPlaylistsByUserName(string userName, [FromUri] int skip = 0, int limit = 10)
        {
            return TunesService.GetPlaylistsByUserName(userName, skip, limit);
        }

        [Route("{username}/{playlistId:int}")]
        public Playlist GetPlaylistById(int playlistId, string userName)
        {
            return this.TunesService.GetPlaylistById(playlistId, userName);
        }

        [Route("{username}/{playlistId:int}/imageids")]
        public ICollection<Guid> GetPlaylistImageIdsById(int playlistId, string userName, int limit)
        {
            return this.TunesService.GetPlaylistImageIdsById(playlistId, userName, limit);
        }

        [Route("{username}/{playlistId:int}/$count")]
        public Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            return this.TunesService.GetPlaylistByIdWithNumberOfEntries(playlistId, userName);
        }
    }
}
