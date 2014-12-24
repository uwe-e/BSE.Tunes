using BSE.Tunes.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices.WindowsRuntime;

namespace BSE.Tunes.StoreApp.Services
{
    public class DataService : IDataService
    {
        #region FieldsPrivate
        private IHostsettingsService m_hostsettingsService;
        private IAccountService m_accountService;
        #endregion

        #region Properties
        public string ServiceUrl
        {
            get { return this.m_hostsettingsService.ServiceUrl; }
        }
        public string UserName
        {
            get
            {
                return this.m_accountService.User != null ? this.m_accountService.User.UserName : null;
            }
        }
        #endregion

        #region MethodsPublic
        public DataService(IHostsettingsService hostSettingsService, IAccountService accountService)
        {
            this.m_hostsettingsService = hostSettingsService;
            this.m_accountService = accountService;
        }

        public async Task<bool> IsHostAccessible()
        {
            bool isHostAccessible = false;
            //This is the only request without any security information.
            using (var httpClient =  new HttpClient())
            {
                httpClient.BaseAddress = new Uri(string.Format("{0}/", this.ServiceUrl));
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

        public async Task<ObservableCollection<Genre>> GetGenres()
        {
            string strUrl = string.Format("{0}/api/tunes/GetGenres", this.ServiceUrl);
            return await GetHttpResponse<ObservableCollection<Genre>>(new Uri(strUrl));
        }

        public async Task<ObservableCollection<Album>> GetAlbums(Query query)
        {
            string strUrl = string.Format("{0}/api/tunes/GetAlbums", this.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Album>, Query>(new Uri(strUrl), query);
        }
        public async Task<ObservableCollection<Album>> GetNewestAlbums(int limit)
        {
            string strUrl = string.Format("{0}/api/tunes/GetNewestAlbums/{1}", this.ServiceUrl, limit);
            return await GetHttpResponse<ObservableCollection<Album>>(new Uri(strUrl));
        }
        public async Task<int> GetNumberOfPlayableAlbums()
        {
            string strUrl = string.Format("{0}/api/tunes/GetNumberOfPlayableAlbums", this.ServiceUrl);
            return await GetHttpResponse<int>(new Uri(strUrl));
        }

        public async Task<Album> GetAlbumById(int albumId)
        {
            string strUrl = string.Format("{0}/api/tunes/GetAlbumById/{1}", this.ServiceUrl, albumId);
            return await GetHttpResponse<Album>(new Uri(strUrl));
        }

        public async Task<Track> GetTrackById(int trackId)
        {
            string strUrl = string.Format("{0}/api/tunes/GetTrackById/{1}", this.ServiceUrl, trackId);
            return await GetHttpResponse<Track>(new Uri(strUrl));
        }
		public async Task<ObservableCollection<int>> GetTrackIdsByFilters(Filter filter)
		{
			string strUrl = string.Format("{0}/api/tunes/GetTrackIdsByFilters", this.ServiceUrl);
			return await GetHttpResponseFromPost<ObservableCollection<int>, Filter>(new Uri(strUrl), filter);
		}
        public async Task<ObservableCollection<Track>> GetTracksByFilters(Filter filter)
        {
            string strUrl = string.Format("{0}/api/tunes/GetTracksByFilters", this.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Track>, Filter>(new Uri(strUrl), filter);
        }
        public async Task<SearchResult> GetSearchResults(Query query)
        {
            string strUrl = string.Format("{0}/api/tunes/GetSearchResults", this.ServiceUrl);
            return await GetHttpResponseFromPost<SearchResult, Query>(new Uri(strUrl), query);
        }
        public async Task<ObservableCollection<Album>> GetAlbumSearchResults(Query query)
        {
            string strUrl = string.Format("{0}/api/tunes/GetAlbumSearchResults", this.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Album>, Query>(new Uri(strUrl), query);
        }
        public async Task<ObservableCollection<Track>> GetTrackSearchResults(Query query)
        {
            string strUrl = string.Format("{0}/api/tunes/GetTrackSearchResults", this.ServiceUrl);
            return await GetHttpResponseFromPost<ObservableCollection<Track>, Query>(new Uri(strUrl), query);
        }
        public async Task<Playlist> InsertPlaylist(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/tunes/InsertPlaylist", this.ServiceUrl);
            return await GetHttpResponseFromPost<Playlist, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<bool> UpdateHistory(History history)
        {
            string strUrl = string.Format("{0}/api/tunes/UpdateHistory", this.ServiceUrl);
            return await GetHttpResponseFromPost<bool, History>(new Uri(strUrl), history);
        }
		public async Task<ObservableCollection<Guid>> GetPlaylistImageIdsById(int playlistId, string userName, int limit)
		{
			string strUrl = string.Format("{0}/api/tunes/GetPlaylistImageIdsById/{1}/{2}/{3}", this.ServiceUrl, playlistId, this.UserName, limit);
			return await this.GetHttpResponse<ObservableCollection<Guid>>(new Uri(strUrl));
		}
		public async Task<Playlist> GetPlaylistById(int playlistId, string userName)
        {
            string strUrl = string.Format("{0}/api/tunes/GetPlaylistById/{1}/{2}/", this.ServiceUrl, playlistId, this.UserName);
            return await this.GetHttpResponse<Playlist>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName)
        {
            string strUrl = string.Format("{0}/api/tunes/GetPlaylistsByUserName/{1}/", this.ServiceUrl, this.UserName);
            return await this.GetHttpResponse<ObservableCollection<Playlist>>(new Uri(strUrl));
        }
        public async Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int limit)
        {
            string strUrl = string.Format("{0}/api/tunes/GetPlaylistsByUserName/{1}/{2}", this.ServiceUrl, this.UserName ,limit);
            return await this.GetHttpResponse<ObservableCollection<Playlist>>(new Uri(strUrl));
        }
        public async Task<Playlist> GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            string strUrl = string.Format("{0}/api/tunes/GetPlaylistByIdWithNumberOfEntries/{1}/{2}/", this.ServiceUrl, playlistId, this.UserName);
            return await this.GetHttpResponse<Playlist>(new Uri(strUrl));
        }
        public async Task<Playlist> AppendToPlaylist(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/tunes/AppendToPlaylist/", this.ServiceUrl);
            return await GetHttpResponseFromPost<Playlist, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<bool> UpdatePlaylistEntries(Playlist playlist)
        {
            string strUrl = string.Format("{0}/api/tunes/UpdatePlaylistEntries", this.ServiceUrl);
            return await GetHttpResponseFromPost<bool, Playlist>(new Uri(strUrl), playlist);
        }
        public async Task<bool> DeletePlaylists(ObservableCollection<Playlist> playlists)
        {
            string strUrl = string.Format("{0}/api/tunes/DeletePlaylists", this.ServiceUrl);
            return await GetHttpResponseFromPost<bool, ObservableCollection<Playlist>>(new Uri(strUrl), playlists);
        }
		public Uri GetImage(Guid imageId, bool asThumbnail = false )
		{
			string strUrl = string.Format("{0}/api/files/getimage/{1}", this.ServiceUrl, imageId.ToString());
			if (asThumbnail)
			{
				strUrl = string.Format("{0}/api/files/getimage/{1}/true", this.ServiceUrl, imageId.ToString());
			}
			return new Uri(strUrl);
		}

        public async Task<HttpClient> GetHttpClient(bool withRefreshToken = true)
        {
            var tokenResponse = this.m_accountService.TokenResponse;
            if (withRefreshToken)
            {
                tokenResponse = await this.m_accountService.RefreshToken();
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
            using (var client = await this.GetHttpClient())
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
            using (var client = await this.GetHttpClient())
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
