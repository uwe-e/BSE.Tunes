using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
	[RoutePrefix("api/albums")]
	[Authorize(Roles = "tunesusers")]
	public class AlbumsController : BaseApiController
	{
		[HttpPost]
		[Route("")]
		public Album[] GetAlbums([FromBody] Query query)
		{
			return this.TunesService.GetAlbums(query);
		}
		[Route("{id:int}")]
		public Album GetAlbumById(int id)
		{
			return this.TunesService.GetAlbumById(id);
		}
		[Route("{limit:int}/newest")]
		public Album[] GetNewestAlbums(int limit)
		{
			return this.TunesService.GetNewestAlbums(limit);
		}
		[Route("number")]
		public int GetNumberOfPlayableAlbums()
		{
			return this.TunesService.GetNumberOfPlayableAlbums();
		}
	}
}