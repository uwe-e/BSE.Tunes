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
        [OperationContract]
        string GetAudioFileNameByGuid(Guid guid);
        [OperationContract]
        Album GetAlbumById(int albumId);
        [OperationContract]
        Genre[] GetGenres();
        [OperationContract]
        Album[] GetAlbums(Query query);
        [OperationContract]
        Album[] GetNewestAlbums(int limit);
        [OperationContract]
        int GetNumberOfPlayableAlbums();
        [OperationContract]
        Track GetTrackById(int trackId);
        [OperationContract]
        ICollection<Track> GetTracksByFilters(Filter filter);
        [OperationContract]
        SearchResult GetSearchResults(Query query);
        [OperationContract]
        [Obsolete("use method GetSearchResults(query) instead!")]
        SearchResult GetSearchResults(string queryString, int pageIndex, int pageSize);
        [OperationContract]
        Album[] GetAlbumSearchResults(Query query);
        [OperationContract]
        [Obsolete("use method GetAlbumSearchResults(query) instead!")]
        Album[] GetAlbumSearchResults(string queryString, int pageIndex, int pageSize);
        [OperationContract]
        Track[] GetTrackSearchResults(Query query);
        [OperationContract]
        [Obsolete("use method GetTrackSearchResults(query) instead!")]
        Track[] GetTrackSearchResults(string queryString, int pageIndex, int pageSize);
        [OperationContract]
        void UpdateHistory(History history);
        [OperationContract]
        Playlist GetPlaylistById(int playlistId, string userName);
        [OperationContract(Name = "GetPlaylistsByUserName")]
        Playlist[] GetPlaylistsByUserName(string userName);
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
        string GetHelloWorld();

    }
}
