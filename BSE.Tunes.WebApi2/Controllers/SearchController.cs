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
		
		[Route("albums/search")]
		public Album[] GetAlbumSearchResults([FromUri] string query, int skip = 0, int limit = 10)
		{
			return this.TunesService.GetAlbumSearchResults(new Query
			{
				SearchPhrase = query,
				PageIndex = skip,
				PageSize = limit
			});
		}
		
		[HttpPost]
		[Route("tracks/")]
		public Track[] GetTrackSearchResults([FromBody] Query query)
		{
			return this.TunesService.GetTrackSearchResults(query);
		}
		
		[Route("tracks/search/")]
		public Track[] GetTrackSearchResults([FromUri] string query, int skip = 0, int limit = 10)
		{
			return this.TunesService.GetTrackSearchResults(new Query
			{
				SearchPhrase = query,
				PageIndex = skip,
				PageSize = limit
			});
		}
		
		[HttpPost]
        [Route("suggestions/")]
        public String[] GetSearchSuggestions([FromBody] Query query)
        {
            return TunesService.GetSearchSuggestions(query);
        }
        
		[Route("suggestions/{searchPhrase}")]
        public String[] GetSearchSuggestions(string searchPhrase)
        {
            return TunesService.GetSearchSuggestions(new Query
            {
                SearchPhrase = searchPhrase
            });
        }
    }
}