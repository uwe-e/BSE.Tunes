﻿using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Entities
{
    public class TunesBusinessObject : TunesRepository, ITunesService
    {
        #region MethodsPublic
        public TunesBusinessObject()
        {
        }

        public bool IsHostAccessible()
        {
            bool isAccessible = false;
            using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
            {
                using (ObjectContext objectContext = tunesEntity.ObjectContext())
                {
                    isAccessible = objectContext.DatabaseExists();
                }
            }
            return isAccessible;
        }

        [Obsolete("Use the common method GetNumberOfAlbums instead")]
        public int GetNumberOfPlayableAlbums()
        {
            int iNumberofAlbums = -1;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(DISTINCT a.TitelID) FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");

            string sql = stringBuilder.ToString();
            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            iNumberofAlbums = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return iNumberofAlbums;
        }
        [Obsolete("Use the common method GetNumberOfAlbums instead")]
        public int GetNumberOfPlayableAlbums(Query query)
        {
            int iNumberofAlbums = -1;
            using (EntityConnection entityConnection =
                                new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        StringBuilder select = new StringBuilder();
                        StringBuilder join = new StringBuilder();
                        StringBuilder clause = new StringBuilder();

                        select.Append("SELECT COUNT(DISTINCT a.TitelID) FROM tunesEntities.titel AS a");
                        join.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
                        clause.Append(" WHERE t.Liedpfad IS NOT NULL");

                        if (query.Data != null ?? query.Data.Id > 0)
                        {
                            join.Append(" LEFT JOIN tunesEntities.genre AS g ON a.genreid = g.genreid");
                            clause.Append(" AND a.genreid = @genreid");

                            EntityParameter genreId = new EntityParameter
                            {
                                ParameterName = "genreid",
                                Value = (int)query.Data.Id
                            };
                            entityCommand.Parameters.Add(genreId);
                        }

                        string sql = select
                            .Append(join)
                            .Append(clause).ToString();

                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            iNumberofAlbums = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return iNumberofAlbums;
        }
        public Album[] GetAlbums(Query query)
        {
            return null;
        }

        [Obsolete("Use the common method GetNumberOfAlbums instead")]
        public int GetNumberOfAlbumsByArtist(int artistId)
        {
            int iNumberofAlbums = -1;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(DISTINCT a.TitelID) FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE i.InterpretID = @artistId");
            stringBuilder.Append(" AND t.Liedpfad IS NOT NULL");

            string sql = stringBuilder.ToString();
            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        EntityParameter id = new EntityParameter
                        {
                            ParameterName = "artistId",
                            Value = artistId
                        };
                        entityCommand.Parameters.Add(id);

                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            iNumberofAlbums = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return iNumberofAlbums;
        }
        public Album[] GetAlbumsByArtist(Query query)
        {
            Album[] albums = null;
            if (query != null && query.Data != null)
            {
                int artistId = (int)query.Data.Id;
                if (artistId > 0)
                {
                    query.PageSize = query.PageSize == 0 ? 1 : query.PageSize;

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("SELECT i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.Guid as AlbumId, a.ErschDatum, g.genreid, g.genre1 FROM tunesEntities.titel AS a");
                    stringBuilder.Append(" INNER JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
                    stringBuilder.Append(" LEFT JOIN tunesEntities.genre AS g ON a.genreid = g.genreid");
                    stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
                    stringBuilder.Append(" WHERE i.InterpretID = @artistId");
                    stringBuilder.Append(" AND t.Liedpfad IS NOT NULL");
                    stringBuilder.Append(" GROUP BY i.InterpretID, i.Interpret, i.Interpret_Lang ,a.TitelID, a.Titel1, a.Guid, a.ErschDatum, g.genreid, g.genre1");
                    stringBuilder.Append(" ORDER BY a.ErschDatum DESC");
                    stringBuilder.Append(" SKIP @skip LIMIT @limit ");

                    string sql = stringBuilder.ToString();
                    using (EntityConnection entityConnection =
                        new EntityConnection(this.ConnectionString))
                    {
                        try
                        {
                            entityConnection.Open();
                            using (EntityCommand entityCommand = entityConnection.CreateCommand())
                            {
                                entityCommand.Parameters.Add(new EntityParameter
                                {
                                    ParameterName = "artistId",
                                    Value = artistId
                                });

                                entityCommand.Parameters.Add(new EntityParameter
                                {
                                    ParameterName = "skip",
                                    Value = query.PageIndex
                                });

                                entityCommand.Parameters.Add(new EntityParameter
                                {
                                    ParameterName = "limit",
                                    Value = query.PageSize
                                });

                                entityCommand.CommandText = sql;

                                albums = ExecuteAlbumReader(entityCommand);
                            }
                        }
                        finally
                        {
                            entityConnection.Close();
                        }
                    }
                }
            }
            return albums;
        }

        public ICollection<int> GetTrackIdsByAlbumIds(IList<int> albumIds)
        {
            Collection<int> trackIds = null;
            if (albumIds != null)
            {
                string list = string.Join(",", albumIds);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
                stringBuilder.Append(" AND t.titelid IN {" + list + "}");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                   new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (trackIds == null)
                                    {
                                        trackIds = new Collection<int>();
                                    }
                                    trackIds.Add(dataReader.GetInt32("LiedID", false, 0));
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return trackIds;
        }
        public CoverImage GetImage(Guid imageId, bool asThumbnail = false)
        {
            CoverImage image = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT a.{0}, a.PictureFormat, a.ErstellDatum, a.MutationDatum");
            stringBuilder.Append(" FROM tunesEntities.titel AS a");
            stringBuilder.Append(" WHERE a.guid = @imageId");

            string field = asThumbnail ? "thumbnail" : "cover";
            string sql = string.Format(CultureInfo.InvariantCulture, stringBuilder.ToString(), field);

            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        EntityParameter id = new EntityParameter
                        {
                            ParameterName = "imageId",
                            Value = imageId.ToString()
                        };
                        entityCommand.Parameters.Add(id);
                        entityCommand.CommandText = sql;
                        using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            if (dataReader.Read())
                            {
                                image = new CoverImage
                                {
                                    Cover = dataReader.GetBytes(field, true, null),
                                    Extension = dataReader.GetString("PictureFormat", true, String.Empty),
                                    ModifiedSince = dataReader.GetDateTime("MutationDatum", true, DateTime.MinValue)
                                };
                            }
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return image;
        }
        public string GetAudioFileNameByGuid(Guid guid)
        {
            string fileName = null;
            string audioDirectory = this.AudioDirectory;
            if (string.IsNullOrEmpty(audioDirectory) == false && guid != null && guid != Guid.Empty)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.Liedpfad FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" WHERE t.guid = @guid");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                        new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter guidParam = new EntityParameter
                            {
                                ParameterName = "guid",
                                Value = guid.ToString()
                            };
                            entityCommand.Parameters.Add(guidParam);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    fileName = GetTrackFilePath(dataReader, audioDirectory);
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return fileName;
        }
        public ICollection<int> GetTrackIdsByFilters(Filter filter)
        {
            Collection<int> tracks = null;
            if (filter != null)
            {
                //filter.Value = "17,25,5,14";
                var names = new int[] { 17, 25, 5, 14 };

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID");
                stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" INNER JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
                //stringBuilder.Append(" AND a.genreid IN (17,25,5,14)");

                switch (filter.Mode)
                {
                    case FilterMode.Genre:
                        stringBuilder.Append(" AND a.genreid IN {" + filter.Value + "}");
                        //stringBuilder.Append(" AND a.genreid IN (@filterValue)");
                        break;
                    case FilterMode.Year:
                        break;
                    default:
                        break;
                }

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                   new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            //EntityParameter filterValue = new EntityParameter();
                            //filterValue.ParameterName = "filterValue";
                            //filterValue.Value = filter.Value;
                            //entityCommand.Parameters.Add(filterValue);
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (tracks == null)
                                    {
                                        tracks = new Collection<int>();
                                    }
                                    tracks.Add(dataReader.GetInt32("LiedID", false, 0));
                                    //Track track = new Track
                                    //{
                                    //	Id = dataReader.GetInt32("LiedID", false, 0)
                                    //};
                                    //tracks.Add(track);
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }

            }
            return tracks;
        }
        public ICollection<Track> GetTracksByFilters(Filter filter)
        {
            Collection<Track> tracks = null;
            if (filter != null)
            {
                //filter.Value = "17,25,5,14";
                var names = new int[] { 17, 25, 5, 14 };

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer");
                stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" INNER JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
                //stringBuilder.Append(" AND a.genreid IN (17,25,5,14)");

                switch (filter.Mode)
                {
                    case FilterMode.Genre:
                        stringBuilder.Append(" AND a.genreid IN {" + filter.Value + "}");
                        //stringBuilder.Append(" AND a.genreid IN (@filterValue)");
                        break;
                    case FilterMode.Year:
                        break;
                    default:
                        break;
                }

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                   new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            //EntityParameter filterValue = new EntityParameter();
                            //filterValue.ParameterName = "filterValue";
                            //filterValue.Value = filter.Value;
                            //entityCommand.Parameters.Add(filterValue);
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (tracks == null)
                                    {
                                        tracks = new Collection<Track>();
                                    }
                                    Track track = new Track
                                    {
                                        Id = dataReader.GetInt32("LiedID", false, 0)
                                    };
                                    tracks.Add(track);
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }

            }
            return tracks;
        }
        public String[] GetSearchSuggestions(Query query)
        {
            string[] suggestions = null;
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false && query.SearchPhrase.Length > 2)
            {
                var searchPhrase = query.SearchPhrase;
                if (!string.IsNullOrEmpty(searchPhrase) && searchPhrase.Length >= 3)
                {
                    using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                    {
                        if (tunesEntity != null)
                        {
                            using (ObjectContext objectContext = tunesEntity.ObjectContext())
                            {
                                var result = objectContext.ExecuteFunction<String>("GetSearchSuggestions", new ObjectParameter[] { new ObjectParameter("searchPhrase", searchPhrase) });
                                if (result != null)
                                {
                                    suggestions = result.ToArray<String>();
                                }
                            }
                        }
                    }
                }
            }
            return suggestions;
        }
        public BSE.Tunes.Data.SearchResult GetSearchResults(Query query)
        {
            BSE.Tunes.Data.SearchResult searchResult = new Data.SearchResult();
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                searchResult.Albums = this.GetAlbumSearchResults(query);
                searchResult.Tracks = this.GetTrackSearchResults(query);
            }
            return searchResult;
        }
        public Album[] GetAlbumSearchResults(Query query)
        {
            Album[] albums = null;
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        List<Album> albumCollection = null;
                        using (ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var result = objectContext.ExecuteFunction<SearchResult>("GetAlbumSearch", new ObjectParameter[] {
                                new ObjectParameter("searchPhrase", query.SearchPhrase),
                                new ObjectParameter("pageSize", query.PageSize),
                                new ObjectParameter("pageIndex", query.PageIndex)
                            });
                            if (result != null)
                            {
                                if (albumCollection == null)
                                {
                                    albumCollection = new List<Album>();
                                }
                                foreach (var album in result)
                                {
                                    if (album != null)
                                    {
                                        albumCollection.Add(new Album
                                        {
                                            Id = album.AlbumId,
                                            Title = album.AlbumName,
                                            AlbumId = string.IsNullOrEmpty(album.Guid) ? Guid.Empty : new Guid(album.Guid),
                                            Artist = new Artist
                                            {
                                                Id = album.ArtistId,
                                                Name = album.ArtistName
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (albumCollection != null)
                        {
                            albums = albumCollection.ToArray();
                        }
                    }
                }
            }
            return albums;
        }
        public Track[] GetTrackSearchResults(Query query)
        {
            Track[] tracks = null;
            if (query != null && string.IsNullOrEmpty(query.SearchPhrase) == false)
            {
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        List<Track> trackCollection = null;
                        using (ObjectContext objectContext = tunesEntity.ObjectContext())
                        {
                            var results = objectContext.ExecuteFunction<SearchResult>("GetTrackSearch", new ObjectParameter[] {
                                new ObjectParameter("searchPhrase", query.SearchPhrase),
                                new ObjectParameter("pageSize", query.PageSize),
                                new ObjectParameter("pageIndex", query.PageIndex)
                                });
                            if (results != null)
                            {
                                if (trackCollection == null)
                                {
                                    trackCollection = new List<Track>();
                                }
                                foreach (var result in results)
                                {
                                    if (result != null)
                                    {
                                        trackCollection.Add(new Track
                                        {
                                            Id = result.TrackId,
                                            Name = result.Track,
                                            Duration = result.Duration,
                                            Album = new Album
                                            {
                                                Id = result.AlbumId,
                                                Title = result.AlbumName,
                                                AlbumId = string.IsNullOrEmpty(result.Guid) ? Guid.Empty : new Guid(result.Guid),
                                                Artist = new Artist
                                                {
                                                    Id = result.ArtistId,
                                                    Name = result.ArtistName
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                        }
                        if (trackCollection != null)
                        {
                            tracks = trackCollection.ToArray();
                        }
                    }
                }
            }
            return tracks;
        }
        public bool UpdateHistory(History history)
        {
            bool hasUpdated = false;
            if (history != null)
            {
                HistoryEntity entity = new HistoryEntity
                {
                    AppID = history.AppId,
                    TitelID = history.AlbumId,
                    LiedID = history.TrackId,
                    Zeit = history.PlayedAt,
                    Benutzer = history.UserName,
                    Interpret = string.Empty,
                    Titel = string.Empty,
                    Lied = string.Empty
                };

                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    tunesEntity.history.Add(entity);
                    hasUpdated = tunesEntity.SaveChanges() > 0;
                }
            }
            return hasUpdated;
        }
        public int GetNumberOfPlaylistsByUserName(string userName)
        {
            int numberofPlaylists = -1;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(p.ListId) FROM tunesEntities.playlist AS p");
            stringBuilder.Append(" WHERE p.User = @userName");

            string sql = stringBuilder.ToString();
            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {

                        EntityParameter user = new EntityParameter
                        {
                            ParameterName = "userName",
                            Value = userName
                        };
                        entityCommand.Parameters.Add(user);

                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            numberofPlaylists = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return numberofPlaylists;
        }
        public ICollection<int> GetTrackIdsByPlaylistIds(IList<int> playlistIds, string userName)
        {
            Collection<int> trackIds = null;
            if (playlistIds != null)
            {
                string list = string.Join(",", playlistIds);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" JOIN tunesEntities.playlistentries AS ple ON t.LiedID = ple.LiedId");
                stringBuilder.Append(" JOIN tunesEntities.playlist AS pl ON ple.PlaylistId = pl.ListId");
                stringBuilder.Append(" WHERE pl.User = @userName");
                stringBuilder.Append(" AND t.Liedpfad IS NOT NULL");
                stringBuilder.Append(" AND pl.ListId IN {" + list + "}");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                   new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (trackIds == null)
                                    {
                                        trackIds = new Collection<int>();
                                    }
                                    trackIds.Add(dataReader.GetInt32("LiedID", false, 0));
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return trackIds;
        }
        public ICollection<Guid> GetPlaylistImageIdsById(int playlistId, string userName, int limit)
        {
            Collection<Guid> imageIds = null;
            if (string.IsNullOrEmpty(userName) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.Append("SELECT a.Guid, COUNT(a.Guid) AS Number FROM tunesEntities.playlist AS p");
                stringBuilder.Append("SELECT a.Guid FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" LEFT JOIN tunesEntities.lieder AS t ON pe.LiedId = t.LiedID");
                stringBuilder.Append(" LEFT JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" WHERE p.ListId = @playlistId");
                stringBuilder.Append(" AND p.User = @userName");
                stringBuilder.Append(" ORDER BY pe.sortorder");
                stringBuilder.Append(" LIMIT @limit ");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                   new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter idParam = new EntityParameter
                            {
                                ParameterName = "playlistId",
                                Value = playlistId
                            };
                            entityCommand.Parameters.Add(idParam);

                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            EntityParameter limitParam = new EntityParameter
                            {
                                ParameterName = "limit",
                                Value = limit
                            };
                            entityCommand.Parameters.Add(limitParam);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                while (dataReader.Read())
                                {
                                    if (imageIds == null)
                                    {
                                        imageIds = new Collection<Guid>();
                                    }
                                    imageIds.Add(dataReader.GetGuid("Guid", true, Guid.Empty));
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return imageIds;
        }
        public Playlist GetPlaylistById(int playlistId, string userName)
        {
            Playlist playlist = null;
            if (string.IsNullOrEmpty(userName) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.User, p.Guid, pe.EntryId, pe.sortorder, pe.Guid as EntryGuid, t.LiedID, t.Lied, t.Dauer, a.Guid as AlbumId, a.TitelID, a.Titel1 as Title, a.ErschDatum, i.Interpret");
                stringBuilder.Append(" FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" LEFT JOIN tunesEntities.lieder AS t ON pe.LiedId = t.LiedID");
                stringBuilder.Append(" LEFT JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                stringBuilder.Append(" LEFT JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
                stringBuilder.Append(" WHERE p.ListId = @playlistId");
                stringBuilder.Append(" AND p.User = @userName");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                        new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter trackIdParam = new EntityParameter
                            {
                                ParameterName = "playlistId",
                                Value = playlistId
                            };
                            entityCommand.Parameters.Add(trackIdParam);

                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (playlist == null)
                                    {
                                        playlist = new Playlist
                                        {
                                            Id = dataReader.GetInt32("ListId", false, 0),
                                            Name = dataReader.GetString("ListName", false, string.Empty),
                                            UserName = dataReader.GetString("User", false, string.Empty),
                                            Guid = dataReader.GetGuid("Guid", false, Guid.Empty),
                                        };
                                    }
                                    int entryId = dataReader.GetInt32("EntryId", true, 0);
                                    if (entryId > 0)
                                    {
                                        //An "Invalid attempt to read from column ordinal" error occurs when you use DataReader in Visual C#
                                        //When you use DataReader to read a row, if you try to access columns in that row, you receive an error message that resembles the following:
                                        //
                                        //System.InvalidOperationException: Invalid attempt to read from column ordinal '0'. With CommandBehavior.SequentialAccess, you may only read from column ordinal '2' or greater. 

                                        var sortOrder = dataReader.GetInt32("sortorder", true, 0);
                                        var guid = dataReader.GetGuid("EntryGuid", true, Guid.Empty);
                                        var trackId = dataReader.GetInt32("LiedID", true, 0);
                                        var name = dataReader.GetString("Lied", true, string.Empty);
                                        var duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue);
                                        var albumId = dataReader.GetGuid("AlbumId", false, Guid.Empty);
                                        var titleId = dataReader.GetInt32("TitelID", true, 0);
                                        var title = dataReader.GetString("Title", true, string.Empty);
                                        var year = dataReader.GetInt32("ErschDatum", true, 0);
                                        var artist = dataReader.GetString("Interpret", true, string.Empty);

                                        PlaylistEntry entry = new PlaylistEntry
                                        {
                                            Id = entryId,
                                            PlaylistId = playlist.Id,
                                            SortOrder = sortOrder,
                                            Guid = guid,
                                            TrackId = trackId,
                                            Name = name,
                                            Duration = duration,
                                            AlbumId = albumId,
                                            Artist = artist,
                                            Track = new Track
                                            {
                                                Id = trackId,
                                                Name = name,
                                                Duration = duration,
                                                Album = new Album
                                                {
                                                    AlbumId = albumId,
                                                    Id = titleId,
                                                    Title = title,
                                                    Year = year,
                                                    Artist = new Artist
                                                    {
                                                        Name = artist
                                                    }
                                                }
                                            }
                                        };
                                        playlist.Entries.Add(entry);
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlist;
        }
        public Playlist[] GetPlaylistsByUserName(string userName)
        {
            return GetPlaylistsByUserName(userName, 0);
        }
        public Playlist[] GetPlaylistsByUserName(string userName, int pageIndex, int pageSize)
        {
            Playlist[] playlists = null;
            if (!string.IsNullOrEmpty(userName))
            {
                pageSize = pageSize == 0 ? 1 : pageSize;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.User, p.guid, COUNT(pe.PlaylistId) as Number ");
                stringBuilder.Append(" FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" WHERE p.User = @userName");
                stringBuilder.Append(" GROUP BY p.listid, p.ListName, p.User, p.guid");
                stringBuilder.Append(" ORDER BY p.ListName");
                stringBuilder.Append(" SKIP @skip LIMIT @limit ");

                string sql = stringBuilder.ToString();

                using (EntityConnection entityConnection =
                      new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            EntityParameter skip = new EntityParameter
                            {
                                ParameterName = "skip",
                                Value = pageIndex
                            };
                            entityCommand.Parameters.Add(skip);

                            EntityParameter limit = new EntityParameter
                            {
                                ParameterName = "limit",
                                Value = pageSize
                            };
                            entityCommand.Parameters.Add(limit);

                            List<Playlist> playlistCollection = null;
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (playlistCollection == null)
                                    {
                                        playlistCollection = new List<Playlist>();
                                    }

                                    Playlist playlist = new Playlist
                                    {
                                        Id = dataReader.GetInt32("ListId", false, 0),
                                        Name = dataReader.GetString("ListName", false, string.Empty),
                                        UserName = dataReader.GetString("User", false, string.Empty),
                                        Guid = dataReader.GetGuid("guid", true, Guid.Empty),
                                        NumberEntries = dataReader.GetInt32("Number",false,0)
                                    };
                                    playlistCollection.Add(playlist);
                                }
                            }
                            if (playlistCollection != null)
                            {
                                playlists = playlistCollection.ToArray();

                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlists;
        }
        public Playlist[] GetPlaylistsByUserName(string userName, int limit)
        {
            Playlist[] playlists = null;
            if (!string.IsNullOrEmpty(userName))
            {
                bool hasLimit = false;
                if (limit > 0)
                {
                    hasLimit = true;
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.User, p.guid FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" WHERE p.User = @userName");
                stringBuilder.Append(" ORDER BY p.ListName");
                if (hasLimit)
                {
                    stringBuilder.Append(" LIMIT @limit ");
                }
                string sql = stringBuilder.ToString();

                using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            EntityParameter limitParam = new EntityParameter
                            {
                                ParameterName = "limit",
                                Value = limit
                            };
                            entityCommand.Parameters.Add(limitParam);

                            List<Playlist> playlistCollection = null;
                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                // Start reading results.
                                while (dataReader.Read())
                                {
                                    if (playlistCollection == null)
                                    {
                                        playlistCollection = new List<Playlist>();
                                    }

                                    Playlist playlist = new Playlist
                                    {
                                        Id = dataReader.GetInt32("ListId", false, 0),
                                        Name = dataReader.GetString("ListName", false, string.Empty),
                                        UserName = dataReader.GetString("User", false, string.Empty),
                                        Guid = dataReader.GetGuid("guid", true, Guid.Empty)
                                    };
                                    playlistCollection.Add(playlist);
                                }
                            }
                            if (playlistCollection != null)
                            {
                                playlists = playlistCollection.ToArray();

                            }
                        }
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlists;
        }
        public Playlist GetPlaylistByIdWithNumberOfEntries(int playlistId, string userName)
        {
            Playlist playlist = null;
            if (string.IsNullOrEmpty(userName) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT p.ListId, p.ListName, p.guid, COUNT(pe.PlaylistId) as Number ");
                stringBuilder.Append(" FROM tunesEntities.playlist AS p");
                stringBuilder.Append(" LEFT JOIN tunesEntities.playlistentries AS pe ON p.ListId = pe.PlaylistId");
                stringBuilder.Append(" WHERE p.ListId = @playlistId");
                stringBuilder.Append(" AND p.User = @userName");
                stringBuilder.Append(" GROUP BY p.listid, p.ListName, p.guid");

                string sql = stringBuilder.ToString();
                using (EntityConnection entityConnection =
                        new EntityConnection(this.ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
                            EntityParameter id = new EntityParameter
                            {
                                ParameterName = "playlistId",
                                Value = playlistId
                            };
                            entityCommand.Parameters.Add(id);

                            EntityParameter user = new EntityParameter
                            {
                                ParameterName = "userName",
                                Value = userName
                            };
                            entityCommand.Parameters.Add(user);

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    playlist = new Playlist
                                    {
                                        Id = dataReader.GetInt32("ListId", false, 0),
                                        Name = dataReader.GetString("ListName", false, string.Empty),
                                        Guid = dataReader.GetGuid("guid", false, Guid.Empty),
                                        NumberEntries = dataReader.GetInt32("Number", false, 0)
                                    };
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        entityConnection.Close();
                    }
                }
            }
            return playlist;
        }
        public Playlist InsertPlaylist(Playlist playlist)
        {
            if (playlist != null)
            {
                PlaylistEntity entity = new PlaylistEntity
                {
                    ListId = playlist.Id,
                    ListName = playlist.Name,
                    User = playlist.UserName,
                    guid = playlist.Guid.ToString(),
                    Timestamp = DateTime.Now
                };
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    PlaylistEntity playlistEntity = tunesEntity.playlist.FirstOrDefault(pl => pl.ListId == playlist.Id);
                    if (playlistEntity == null)
                    {
                        // Unique index on playlist table removed
                        // because of, we allow now multiple playlists with the same name. 
                        //if (tunesEntity.playlist.FirstOrDefault(
                        //    pl => string.Compare(pl.ListName, entity.ListName) == 0 && string.Compare(pl.User, entity.User) == 0) != null)
                        //{
                        //    string playlistExistsExceptionMessage = string.Format(CultureInfo.InvariantCulture, SRResources.PlaylistExistsException, playlist.Name);
                        //    throw new PlaylistExistsException(playlistExistsExceptionMessage);
                        //}
                        tunesEntity.playlist.Add(entity);
                    }
                    tunesEntity.SaveChanges();
                    playlist = new Playlist
                    {
                        Id = entity.ListId,
                        Name = entity.ListName,
                        UserName = entity.User,
                        Guid = new Guid(entity.guid)
                    };
                }
            }
            return playlist;
        }
        public Playlist AppendToPlaylist(Playlist playlist)
        {
            if (playlist != null && playlist.Entries != null)
            {
                List<PlaylistEntry> entries = new List<PlaylistEntry>(playlist.Entries);
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        //Get the highest sort number and increase it
                        int sortOrder = (tunesEntity.playlistentries
                            .Where(pe => pe.PlaylistId == playlist.Id)
                            .OrderByDescending(pe => pe.sortorder).FirstOrDefault()?.sortorder ?? 0) + 1;

                        entries.ForEach((playlistEntry) =>
                            {
                                if (playlistEntry != null)
                                {
                                    tunesEntity.playlistentries.Add(new PlaylistEntryEntity
                                    {
                                        Guid = playlistEntry.Guid,
                                        LiedId = playlistEntry.TrackId,
                                        PlaylistId = playlist.Id,
                                        sortorder = sortOrder,
                                        Timestamp = DateTime.Now
                                    });
                                }
                            });
                        tunesEntity.SaveChanges();
                    }
                }
            }
            return playlist = this.GetPlaylistById(playlist.Id, playlist.UserName);
        }

        public Playlist UpdatePlaylist(Playlist playlist)
        {
            if (playlist != null && playlist.Entries != null)
            {
                List<PlaylistEntry> entries = new List<PlaylistEntry>(playlist.Entries);
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        var entr = tunesEntity.playlistentries.Where((e) => e.PlaylistId == playlist.Id).ToList();
                        entr.ForEach((e) =>
                        {
                            if (e != null)
                            {
                                tunesEntity.playlistentries.Remove(e);
                            }
                        });
                        foreach (var e in playlist.Entries)
                        {
                            if (e != null)
                            {
                                PlaylistEntryEntity entity = new PlaylistEntryEntity
                                {
                                    Guid = e.Guid,
                                    LiedId = e.TrackId,
                                    PlaylistId = playlist.Id,
                                    Timestamp = DateTime.Now,
                                    sortorder = e.SortOrder
                                };
                                tunesEntity.playlistentries.Add(entity);
                            }
                        }
                        tunesEntity.SaveChanges();
                    }
                }
            }
            return this.GetPlaylistById(playlist.Id, playlist.UserName);
        }

        public bool UpdatePlaylistEntries(Playlist playlist)
        {
            bool hasUpdated = false;
            if (playlist != null && playlist.Entries != null)
            {
                List<PlaylistEntry> entries = new List<PlaylistEntry>(playlist.Entries);
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    if (tunesEntity != null)
                    {
                        var entr = tunesEntity.playlistentries.Where((e) => e.PlaylistId == playlist.Id).ToList();
                        entr.ForEach((e) =>
                        {
                            if (e != null)
                            {
                                tunesEntity.playlistentries.Remove(e);
                            }
                        });
                        foreach (var e in playlist.Entries)
                        {
                            if (e != null)
                            {
                                PlaylistEntryEntity entity = new PlaylistEntryEntity
                                {
                                    Guid = e.Guid,
                                    LiedId = e.TrackId,
                                    PlaylistId = playlist.Id,
                                    Timestamp = DateTime.Now,
                                    sortorder = e.SortOrder
                                };
                                tunesEntity.playlistentries.Add(entity);
                            }
                        }
                        tunesEntity.SaveChanges();
                        hasUpdated = true;
                    }
                }
            }
            return hasUpdated;
        }

        public void DeletePlaylist(int playlistId)
        {
            using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
            {
                var entryEntities = tunesEntity.playlistentries.Where(entry => entry.PlaylistId == playlistId);
                if (entryEntities != null)
                {
                    foreach (var entry in entryEntities)
                    {
                        if (entry != null)
                        {
                            tunesEntity.playlistentries.Remove(entry);
                        }
                    }
                }
                var playlistEntity = tunesEntity.playlist.Where(list => list.ListId == playlistId).FirstOrDefault();
                if (playlistEntity != null)
                {
                    tunesEntity.playlist.Remove(playlistEntity);
                }
                tunesEntity.SaveChanges();
            }
        }

        public bool DeletePlaylists(IList<Playlist> playlists)
        {
            bool hasDeleted = false;
            if (playlists != null)
            {
                using (TunesEntities tunesEntity = new TunesEntities(this.ConnectionString))
                {
                    foreach (var playlist in playlists)
                    {
                        if (playlist != null)
                        {
                            var entryEntities = tunesEntity.playlistentries.Where(entry => entry.PlaylistId == playlist.Id);
                            if (entryEntities != null)
                            {
                                foreach (var entry in entryEntities)
                                {
                                    if (entry != null)
                                    {
                                        tunesEntity.playlistentries.Remove(entry);
                                    }
                                }
                            }
                            var playlistEntity = tunesEntity.playlist.Where(list => list.ListId == playlist.Id).FirstOrDefault();
                            if (playlistEntity != null)
                            {
                                tunesEntity.playlist.Remove(playlistEntity);
                            }
                        }
                    }
                    tunesEntity.SaveChanges();
                    hasDeleted = true;
                }
            }
            return hasDeleted;
        }
        public SystemInfo GetSystemInfo()
        {
            SystemInfo sysInfo = new SystemInfo();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(0) AS count");
            stringBuilder.Append(" FROM tunesEntities.lieder AS t");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");

            string sql = stringBuilder.ToString();
            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            sysInfo.NumberTracks = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return sysInfo;
        }
        public string GetHelloWorld()
        {
            return "Hello World";
        }
        #endregion

        #region MethodsPrivate
        private string GetTrackFilePath(IDataReader dataReader, string strAudioDirectory)
        {
            string strFilePath = null;
            if (dataReader != null)
            {
                string strPath = dataReader.GetString("Liedpfad", true, null);
                if (string.IsNullOrEmpty(strPath) == false)
                {
                    strFilePath = System.IO.Path.Combine(strAudioDirectory, strPath);
                }
            }
            return strFilePath;
        }

        #endregion
    }
}
