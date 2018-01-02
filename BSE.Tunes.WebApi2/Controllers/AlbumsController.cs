﻿using BSE.Tunes.Data;
using Microsoft.Web.Http;
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
        [AllowAnonymous]
		public Album GetAlbumById(int id)
		{
			return this.TunesService.GetAlbumById(id);
		}
		[Route("{limit:int}/newest")]
        public Album[] GetNewestAlbums(int limit)
		{
			return this.TunesService.GetNewestAlbums(limit);
		}
        [Route("{limit:int}/featured")]
        public Album[] GetFeaturedAlbums(int limit)
        {
            return this.TunesService.GetFeaturedAlbums(limit);
        }
		[Route("number")]
		public int GetNumberOfPlayableAlbums()
		{
			return this.TunesService.GetNumberOfPlayableAlbums();
		}
		[HttpPost]
		[Route("numberof")]
		public int GetNumberOfPlayableAlbums([FromBody] Query query)
		{
            return this.TunesService.GetNumberOfPlayableAlbums(query.Data);
        }

        [HttpPost]
        [Route("trackids")]
        public ICollection<int> GetTrackIdsByAlbumIds([FromBody] IList<int> albumIds)
        {
            return TunesService.GetTrackIdsByAlbumIds(albumIds);
        }
    }
}