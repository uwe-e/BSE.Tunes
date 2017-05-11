using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
	[RoutePrefix("api/search")]
	[Authorize(Roles = "tunesusers")]
	public class SearchController : BaseApiController
	{
		[HttpPost]
		[Route("")]
		public SearchResult GetSearchResults([FromBody] Query query)
		{
			return this.TunesService.GetSearchResults(query);
		}

		[HttpPost]
		[Route("albums/")]
		public Album[] GetAlbumSearchResults([FromBody] Query query)
		{
			return this.TunesService.GetAlbumSearchResults(query);
		}

		[HttpPost]
		[Route("tracks/")]
		public Track[] GetTrackSearchResults([FromBody] Query query)
		{
			return this.TunesService.GetTrackSearchResults(query);
		}
        [HttpPost]
        [Route("suggestions/")]
        public String[] GetSearchSuggestions([FromBody] Query query)
        {
            return TunesService.GetSearchSuggestions(query);
        }

    }
}