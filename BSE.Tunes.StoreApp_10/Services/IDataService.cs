using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Gets the url that contains the web api service.
        /// </summary>
        string ServiceUrl
        {
            get;
        }
        Task<bool> IsHostAccessible();
        Task<bool> IsHostAccessible(string serviceUrl);
        Task<ObservableCollection<Album>> GetAlbums(Query query);
        Task<Album> GetAlbumById(int albumId);
        Task<int> GetNumberOfPlayableAlbums();
        Uri GetImage(Guid imageId, bool asThumbnail = false);
        Task<HttpClient> GetHttpClient(bool withRefreshToken = true);
        Task<ObservableCollection<Album>> GetNewestAlbums(int limit);
        Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit);
        Task<Track> GetTrackById(int trackId);
        Task<ObservableCollection<int>> GetTrackIdsByFilters(Filter filter);
        Task<bool> UpdateHistory(History history);
        Task<SystemInfo> GetSystemInfo();
        Task<Playlist> GetPlaylistById(int playlistId, string userName);
        Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName);
        Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int limit);
        Task<Playlist> GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName);
        Task<ObservableCollection<Guid>> GetPlaylistImageIdsById(int playlistId, string userName, int limit);
        Task<Playlist> InsertPlaylist(Playlist playlist);
        Task<Playlist> AppendToPlaylist(Playlist playlist);
    }
}
