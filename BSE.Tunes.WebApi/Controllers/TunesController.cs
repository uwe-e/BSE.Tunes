using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSE.Tunes.WebApi.Controllers
{
    [RoutePrefix("api/tunes")]
    [Authorize(Roles = "tunesusers")]
    public class TunesController : ApiController
    {
        #region FieldsPrivate
        private ITunesService m_tunesService;
        // Track whether Dispose has been called.
        private bool m_isDisposed;
        #endregion

        public TunesController()
        {
            this.m_tunesService = new BSE.Tunes.Entities.TunesBusinessObject();
        }

        // GET /api/tunes/IsHostAccessible
        [HttpGet]
        [AllowAnonymous]
        [Route("{action}")]
        public bool IsHostAccessible()
        {
            return this.m_tunesService.IsHostAccessible();
        }
        // GET /api/tunes/GetGenres
        [HttpGet]
        [Route("{action}")]
        public Genre[] GetGenres()
        {
            return this.m_tunesService.GetGenres();
        }

        [Route("{action}")]
        public int GetNumberOfPlayableAlbums()
        {
            return this.m_tunesService.GetNumberOfPlayableAlbums();
        }

        // POST /api/album/GetAlbums/?PageIndex=0&PageSize=20
        [HttpPost]
        [Route("{action}")]
        public Album[] GetAlbums([FromBody] Query query)
        {
            return this.m_tunesService.GetAlbums(query);
        }

        [Route("{action}/{limit}")]
        public Album[] GetNewestAlbums(int limit)
        {
            return this.m_tunesService.GetNewestAlbums(limit);
        }
        
        // GET /api/tunes/getalbumbyid/1164
        [Route("{action}/{id}")]
        public Album GetAlbumById(int id)
        {
            return this.m_tunesService.GetAlbumById(id);
        }

        [Route("{action}/{trackId}")]
        public Track GetTrackById(int trackId)
        {
            return this.m_tunesService.GetTrackById(trackId);
        }
        
        [HttpPost]
        [Route("{action}")]
        public ICollection<Track> GetTracksByFilters([FromBody]Filter filter)
        {
            return this.m_tunesService.GetTracksByFilters(filter);
        }
        [HttpPost]
        [Route("{action}")]
        public SearchResult GetSearchResults([FromBody] Query query)
        {
            return this.m_tunesService.GetSearchResults(query);
        }

        [HttpPost]
        [Route("{action}")]
        public Album[] GetAlbumSearchResults([FromBody] Query query)
        {
            return this.m_tunesService.GetAlbumSearchResults(query);
        }

        [HttpPost]
        [Route("{action}")]
        public Track[] GetTrackSearchResults([FromBody] Query query)
        {
            return this.m_tunesService.GetTrackSearchResults(query);
        }

        [HttpPost]
        [Route("{action}")]
        public void UpdateHistory([FromBody] History history)
        {
            this.m_tunesService.UpdateHistory(history);
        }

        [Route("{action}/{playlistId}/{username}/")]
        public Playlist GetPlaylistById(int playlistId, string userName)
        {
            return this.m_tunesService.GetPlaylistById(playlistId, userName);
        }

        [Route("{action}/{username}/")]
        public Playlist[] GetPlaylistsByUserName(string userName)
        {
            return this.m_tunesService.GetPlaylistsByUserName(userName);
        }

        [Route("{action}/{username}/{limit}")]
        public Playlist[] GetPlaylistsByUserName(string userName, int limit)
        {
            return this.m_tunesService.GetPlaylistsByUserName(userName, limit);
        }

        [Route("{action}/{playlistId}/{username}/")]
        public Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            return this.m_tunesService.GetPlaylistByIdWithNumberOfEntries(playlistId, userName);
        }

        // POST api/tunes/InsertPlaylist
        [HttpPost]
        [Route("{action}")]
        public Playlist InsertPlaylist([FromBody] Playlist playlist)
        {
            return this.m_tunesService.InsertPlaylist(playlist);
        }

        [HttpPost]
        [Route("{action}/")]
        public Playlist AppendToPlaylist([FromBody] Playlist playlist)
        {
            return this.m_tunesService.AppendToPlaylist(playlist);
        }

        [HttpPost]
        [Route("{action}")]
        public bool UpdatePlaylistEntries([FromBody] Playlist playlist)
        {
            return this.m_tunesService.UpdatePlaylistEntries(playlist);
        }

        [HttpPost]
        [Route("{action}")]
        public bool DeletePlaylists([FromBody] IList<Playlist> playlists)
        {
            return this.m_tunesService.DeletePlaylists(playlists);
        }

        [Route("{action}")]
        public string GetHelloWorld()
        {
            return this.m_tunesService.GetHelloWorld();
        }
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.m_isDisposed == false)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (this.m_tunesService != null)
                    {
                        this.m_tunesService = null;
                    }
                }
                this.m_isDisposed = true;
            }
            base.Dispose(disposing);
        }
    }
}