using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BSE.Tunes.Data
{
    [ServiceContract]
    public interface ITunesService
    {
        [OperationContract]
        bool IsHostAccessible();
		CoverImage GetImage(Guid imageId, bool asThumbnail = false);
		[OperationContract]
        string GetAudioFileNameByGuid(Guid guid);
        [OperationContract]
        ICollection<int> GetTrackIdsByAlbumIds(IList<int> albumIds);
        [OperationContract]
        Album GetAlbumById(int albumId);
        [OperationContract]
        Genre[] GetGenres();
        [OperationContract]
        Album[] GetAlbums(Query query);
        [OperationContract]
        int GetNumberOfAlbumsByArtist(int artistId);
        [OperationContract]
        Album[] GetAlbumsByArtist(Query query);
        [OperationContract]
        Album[] GetNewestAlbums(int limit);
        [OperationContract]
        Album[] GetFeaturedAlbums(int limit);
        [OperationContract]
        int GetNumberOfPlayableAlbums();
        [OperationContract]
        Track GetTrackById(int trackId);
        [OperationContract]
		ICollection<int> GetTrackIdsByFilters(Filter filter);
		[OperationContract]
        ICollection<Track> GetTracksByFilters(Filter filter);
        [OperationContract]
        ICollection<Track> GetTopTracks(int pageIndex, int pageSize);
        [OperationContract]
        String[] GetSearchSuggestions(Query query);
        [OperationContract]
        SearchResult GetSearchResults(Query query);
        [OperationContract]
        Album[] GetAlbumSearchResults(Query query);
        [OperationContract]
        Track[] GetTrackSearchResults(Query query);
        [OperationContract]
        bool UpdateHistory(History history);
        [OperationContract]
        int GetNumberOfPlaylistsByUserName(string userName);
        [OperationContract]
        ICollection<int> GetTrackIdsByPlaylistIds(IList<int> playlistIds, string userName);
        [OperationContract]
		ICollection<Guid> GetPlaylistImageIdsById(int playlistId, string userName, int limit);
		[OperationContract]
        Playlist GetPlaylistById(int playlistId, string userName);
        [OperationContract(Name = "GetPlaylistsByUserName")]
        Playlist[] GetPlaylistsByUserName(string userName);
        [OperationContract]
        Playlist[] GetPlaylistsByUserName(string userName, int pageIndex, int pageSize);
        [OperationContract(Name = "GetPlaylistsByUserNameAndLimit")]
        Playlist[] GetPlaylistsByUserName(string userName, int limit);
        [OperationContract]
        Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName);
        [OperationContract]
        Playlist InsertPlaylist(Playlist playlist);
        [OperationContract]
        Playlist AppendToPlaylist(Playlist playlist);
        [OperationContract]
        bool UpdatePlaylistEntries(Playlist playlist);
        [OperationContract]
        bool DeletePlaylists(IList<Playlist> playlists);
        [OperationContract]
        SystemInfo GetSystemInfo();
        [OperationContract]
        string GetHelloWorld();

    }
}
