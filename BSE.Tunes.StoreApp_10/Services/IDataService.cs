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
        Task<int> GetNumberOfAlbumsByGenre(int? genreId);
        Task<ObservableCollection<Album>> GetAlbumsByGenre(int? genreId, int skip, int limit);
        Task<int> GetNumberOfAlbumsByArtist(int artistId);
        Task<ObservableCollection<Album>> GetAlbumsByArtist(int artistId, int skip, int limit);
        Task<ObservableCollection<int>> GetTrackIdsByAlbumIds(ICollection<int> albumIds);
		Task<ObservableCollection<Track>> GetTopTracks(int skip, int limit);
		Task<Album> GetAlbumById(int albumId);
		Uri GetImage(Guid imageId, bool asThumbnail = false);
		Task<HttpClient> GetHttpClient(bool withRefreshToken = true);
		Task<ObservableCollection<Album>> GetNewestAlbums(int limit);
		Task<ObservableCollection<Album>> GetFeaturedAlbums(int limit);
		Task<ObservableCollection<Genre>> GetGenres();
		Task<ObservableCollection<Track>> GetTrackSearchResults(Query query);
		Task<Track> GetTrackById(int trackId);
        Task<ObservableCollection<int>> GetTrackIdsByGenre(int? genreId = null);
		Task<String[]> GetSearchSuggestions(Query query);
		Task<ObservableCollection<Album>> GetAlbumSearchResults(Query query);
		Task<bool> UpdateHistory(History history);
		Task<SystemInfo> GetSystemInfo();
		Task<int> GetNumberOfPlaylistsByUserName(string userName);
		Task<ObservableCollection<int>> GetTrackIdsByPlaylistIds(ICollection<int> playlistIds, string userName);
		Task<Playlist> GetPlaylistById(int playlistId, string userName);
		Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName);
		Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int limit);
		Task<ObservableCollection<Playlist>> GetPlaylistsByUserName(string userName, int pageIndex, int pageSize);
		Task<Playlist> GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName);
		Task<ObservableCollection<Guid>> GetPlaylistImageIdsById(int playlistId, string userName, int limit);
		Task<Playlist> InsertPlaylist(Playlist playlist);
		Task<Playlist> AppendToPlaylist(Playlist playlist);
		Task<bool> UpdatePlaylistEntries(Playlist playlist);
		Task<bool> DeletePlaylists(ObservableCollection<Playlist> playlists);
	}
}
