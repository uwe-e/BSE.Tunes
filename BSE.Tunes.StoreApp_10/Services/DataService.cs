using BSE.Tunes.Data;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public class DataService : IDataService
    {
        #region FieldsPrivate
        private SettingsService m_settingsService;
        private IAuthenticationService m_authenticationHandler;

        #endregion

        #region Properties
        public string ServiceUrl
        {
            get
            {
                return m_settingsService.ServiceUrl;
            }
        }
        #endregion

        #region MethodsPublic
        public DataService(IAuthenticationService authenticationHandler)
        {
            m_settingsService = SettingsService.Instance;
            m_authenticationHandler = authenticationHandler;
        }
        public static IDataService Instance
        {
            get
            {
                return ServiceLocator.Current.GetInstance<IDataService>() as DataService;
            }
        }
        public async Task<bool> IsHostAccessible()
        {
            return await IsHostAccessible(m_settingsService.ServiceUrl);
        }
        public async Task<bool> IsHostAccessible(string serviceUrl)
        {
            bool isHostAccessible = false;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(string.Format("{0}/", serviceUrl));
                try
                {
                    HttpResponseMessage message = await httpClient.GetAsync("api/tunes/IsHostAccessible");
                    if (message.IsSuccessStatusCode)
                    {
                        isHostAccessible = message.Content.ReadAsAsync<bool>().Result;
                    }
                    else
                    {
                        throw new Exception(message.ReasonPhrase);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.InnerException != null)
                    {
                        throw exception.InnerException;
                    }
                    throw exception;
                }
            }
            return isHostAccessible;
        }

        public async Task<int> GetNumberOfAlbumsByGenre(int? genreId)
        {
            string strUrl = string.Format("{0}/api/v2/albums/genre/{1}/count", ServiceUrl, genreId ?? 0);
            return await GetHttpResponse<int>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Album>> GetAlbumsByGenre(int? genreId, int skip, int limit)
        {
            string strUrl = string.Format("{0}/api/v2/albums/genre/{1}/?skip={2}&limit={3}", ServiceUrl, genreId ?? 0, skip, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }
        public async Task<int> GetNumberOfAlbumsByArtist(int artistId)
        {
            string strUrl = string.Format("{0}/api/v2/artists/{1}/albums/count", ServiceUrl, artistId);
            return await GetHttpResponse<int>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<Album>> GetAlbumsByArtist(int artistId, int skip = 0, int limit = 10)
        {
            string strUrl = string.Format("{0}/api/v2/artists/{1}/albums?skip={2}&limit={3}", ServiceUrl, artistId, skip, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<int>> GetTrackIdsByAlbumIds(ICollection<int> albumIds)
        {
            string strUrl = string.Format("{0}/api/albums/trackids", ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<int>, ICollection<int>>(new Uri(strUrl), albumIds);
        }

        public async Task<ObservableCollection<Track>> GetTopTracks(int skip, int limit)
        {
            string strUrl = string.Format("{0}/api/v2/tracks/top/?skip={1}&limit={2}", ServiceUrl, skip, limit);
            return await GetHttpResponse<ObservableCollection<Track>>(new Uri(strUrl));
        }

        public async Task<Album> GetAlbumById(int albumId)
        {
            string strUrl = string.Format("{0}/api/v2/albums/{1}", ServiceUrl, albumId);
            return await GetHttpResponse<Album>(new Uri(strUrl));
        }
        public Uri GetImage(Guid imageId, bool asThumbnail = false)
        {
            string strUrl = string.Format("{0}/api/files/image/{1}", ServiceUrl, imageId.ToString());
            if (asThumbnail)
            {
                strUrl = string.Format("{0}/api/files/image/{1}/true", ServiceUrl, imageId.ToString());
            }
            return new Uri(strUrl);
        }
        public async Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/v2/albums/featured?limit={1}", ServiceUrl, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Album>> GetNewestAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/v2/albums/newest?limit={1}", ServiceUrl, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }
        public async Task<Track> GetTrackById(int trackId)
        {
            string strUrl = string.Format("{0}/api/v2/tracks/{1}", ServiceUrl, trackId);
            return await GetHttpResponse<Track>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<int>> GetTrackIdsByGenre(int? genreId = null)
        {
            string strUrl = string.Format("{0}/api/v2/tracks/genre/{1}", ServiceUrl, genreId ?? 0);
            return await GetHttpResponse<ObservableCollection<int>>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<Album>> GetAlbumSearchResults(Query query)
        {
            string strUrl = string.Format("{0}/api/search/albums", ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Album>, Query>(new Uri(strUrl), query);
        }
		public async Task<ObservableCollection<Genre>> GetGenres()
		{
			string strUrl = string.Format("{0}/api/v2/genres", ServiceUrl);
			return await GetHttpResponse<ObservableCollection<Genre>>(new Uri(strUrl));
		}
		public async Task<ObservableCollection<Track>> GetTrackSearchResults(Query query)
        {
            string strUrl = string.Format("{0}/api/search/tracks", ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Track>, Query>(new Uri(strUrl), query);
        }
        public async Task<String[]> GetSearchSuggestions(Query query)
        {
            string strUrl = string.Format("{0}/api/search/suggestions", ServiceUrl);
            return await GetHttpResponseFromPost<String[], Query>(new Uri(strUrl), query);
        }
        public async Task<bool> UpdateHistory(History history)
        {
            string strUrl = string.Format("{0}/api/tunes/UpdateHistory", ServiceUrl);
            return await GetHttpResponseFromPost<bool, History>(new Uri(strUrl), history);
        }
        public async Task<SystemInfo> GetSystemInfo()
        {
            string strUrl = string.Format("{0}/api/system", ServiceUrl);
            return await GetHttpResponse<SystemInfo>(new Uri(strUrl));
        }
        public async Task<int> GetNumberOfPlaylistsByUserName(string userName)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/numbersofplaylists", ServiceUrl, userName);
            return await GetHttpResponse<int>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<int>> GetTrackIdsByPlaylistIds(ICollection<int> playlistIds, string userName)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/trackids", ServiceUrl, userName);
            return await GetHttpResponseFromPost<ObservableCollection<int>, ICollection<int>>(new Uri(strUrl), playlistIds);
        }
        public async Task<Playlist> GetPlaylistById(int playlistId, string userName)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/{2}/", ServiceUrl, playlistId, userName);
            return await GetHttpResponse<Playlist>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/playlists", ServiceUrl, userName);
            return await GetHttpResponse<ObservableCollection<Playlist>>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int limit)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/{2}/playlists", ServiceUrl, userName, limit);
            return await GetHttpResponse<ObservableCollection<Playlist>>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int pageIndex, int pageSize)
        {
            string strUrl = string.Format("{0}/api/playlist/{1}/playlists/{2}/{3}", ServiceUrl, userName, pageIndex, pageSize);
            return await GetHttpResponse<ObservableCollection<Playlist>>(new Uri(strUrl));
        }
        public async Task<Playlist> GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            string strUrl = string.Format("{0}/api/playlist/GetPlaylistByIdWithNumberOfEntries/{1}/{2}/", ServiceUrl, playlistId, userName);
            return await GetHttpResponse<Playlist>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Guid>> GetPlaylistImageIdsById(int playlistId, string userName, int limit)
        {
            string strUrl = string.Format("{0}/api/playlist/GetPlaylistImageIdsById/{1}/{2}/{3}", ServiceUrl, playlistId, userName, limit);
            return await GetHttpResponse<ObservableCollection<Guid>>(new Uri(strUrl));
        }
        public async Task<Playlist> InsertPlaylist(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/playlist/insert", ServiceUrl);
            return await GetHttpResponseFromPost<Playlist, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<bool> UpdatePlaylistEntries(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/playlist/update", ServiceUrl);
            return await GetHttpResponseFromPost<bool, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<Playlist> AppendToPlaylist(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/playlist/append", ServiceUrl);
            return await GetHttpResponseFromPost<Playlist, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<bool> DeletePlaylists(ObservableCollection<Playlist> playlists)
        {
            string strUrl = string.Format("{0}/api/playlist/delete", ServiceUrl);
            return await GetHttpResponseFromPost<bool, ObservableCollection<Playlist>>(new Uri(strUrl), playlists);
        }
        public async Task<HttpClient> GetHttpClient(bool withRefreshToken = true)
        {
            var tokenResponse = m_authenticationHandler.TokenResponse;
            if (withRefreshToken)
            {
                tokenResponse = await m_authenticationHandler.RefreshToken();
            }
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            return client;
        }

        #endregion

        #region MethodsPrivate
        private async Task<T> GetHttpResponseFromPost<T, U>(Uri uri, U from)
        {
            T result = default(T);
            using (var client = await GetHttpClient())
            {
                try
                {
                    var responseMessage = await client.PostAsJsonAsync<U>(uri, from);
                    responseMessage.EnsureExtendedSuccessStatusCode();
                    result = responseMessage.Content.ReadAsAsync<T>().Result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        private async Task<T> GetHttpResponse<T>(Uri uri)
        {
            T result = default(T);
            using (var client = await GetHttpClient())
            {
                try
                {
                    var responseMessage = await client.GetAsync(uri);
                    responseMessage.EnsureExtendedSuccessStatusCode();
                    result = responseMessage.Content.ReadAsAsync<T>().Result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }
        #endregion
    }
}
