using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
	[RoutePrefix("api/playlist")]
	[Authorize(Roles = "tunesusers")]
	public class PlaylistController : BaseApiController
	{
		[HttpPost]
		[Route("append")]
		public Playlist AppendToPlaylist([FromBody] Playlist playlist)
		{
			return this.TunesService.AppendToPlaylist(playlist);
		}
		[HttpPost]
		[Route("delete")]
		public bool DeletePlaylists([FromBody] IList<Playlist> playlists)
		{
			return this.TunesService.DeletePlaylists(playlists);
		}
		[HttpPost]
		[Route("insert")]
		public Playlist InsertPlaylist([FromBody] Playlist playlist)
		{
			return this.TunesService.InsertPlaylist(playlist);
		}
		[HttpPost]
		[Route("update")]
		public bool UpdatePlaylistEntries([FromBody] Playlist playlist)
		{
			return this.TunesService.UpdatePlaylistEntries(playlist);
		}
        [Route("{username}/numbersofplaylists")]
        public int GetNumberOfPlaylistsByUserName(string userName)
        {
            return this.TunesService.GetNumberOfPlaylistsByUserName(userName);
        }
        [Route("{username}/playlists/{pageIndex:int}/{pageSize:int}")]
        public Playlist[] GetPlaylistsByUserName(string userName, int pageIndex, int pageSize)
        {
            return TunesService.GetPlaylistsByUserName(userName, pageIndex, pageSize);
        }
        [HttpPost]
        [Route("{username}/trackids")]
        public ICollection<int> GetTrackIdsByPlaylistIds([FromBody] IList<int> playlistIds, string userName)
        {
            return TunesService.GetTrackIdsByPlaylistIds(playlistIds, userName);
        }
        [Route("{playlistId:int}/{username}/")]
		public Playlist GetPlaylistById(int playlistId, string userName)
		{
			return this.TunesService.GetPlaylistById(playlistId, userName);
		}
		[Route("{username}/playlists")]
		public Playlist[] GetPlaylistsByUserName(string userName)
		{
			return this.TunesService.GetPlaylistsByUserName(userName);
		}
		[Route("{username}/{limit:int}/playlists")]
		public Playlist[] GetPlaylistsByUserName(string userName, int limit)
		{
			return this.TunesService.GetPlaylistsByUserName(userName, limit);
		}
		[Route("GetPlaylistImageIdsById/{playlistId:int}/{username}/{limit:int}")]
		public ICollection<Guid> GetPlaylistImageIdsById(int playlistId, string userName, int limit)
		{
			return this.TunesService.GetPlaylistImageIdsById(playlistId, userName, limit);
		}
		[Route("GetPlaylistByIdWithNumberOfEntries/{playlistId:int}/{username}/")]
		public Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
		{
			return this.TunesService.GetPlaylistByIdWithNumberOfEntries(playlistId, userName);
		}
	}
}