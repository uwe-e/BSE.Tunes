using BSE.Tunes.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;

namespace BSE.Tunes.Entities
{
    public class TunesRepository
    {
        #region FieldsPrivate
        private string m_audioDirectory;
        private string m_connectionString;
        #endregion

        #region Properties
        public string AudioDirectory => m_audioDirectory ??
            (m_audioDirectory = System.Configuration.ConfigurationManager.AppSettings["AudioDirectory"]);

        protected string ConnectionString => m_connectionString ??
            (m_connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TunesEntities"].ConnectionString);
        #endregion

        #region MethodsPublic
        public Genre[] GetGenres()
        {
            Genre[] genres = null;
            using (EntityConnection entityConnection =
                new EntityConnection(ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        List<Genre> genreCollection = null;

                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("SELECT g.Genre_Id, g.Genre_Name FROM tunesEntities.genres AS g");
                        stringBuilder.Append(" ORDER BY g.Genre_Name");

                        entityCommand.CommandText = stringBuilder.ToString();

                        using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                        {
                            // Start reading results.
                            while (dataReader.Read())
                            {
                                if (genreCollection == null)
                                {
                                    genreCollection = new List<Genre>();
                                }
                                genreCollection.Add(new Genre
                                {
                                    Id = dataReader.GetInt32("Genre_Id", false, 0),
                                    Name = dataReader.GetString("Genre_Name", false, string.Empty)
                                });
                            }
                        }
                        if (genreCollection != null)
                        {
                            genres = genreCollection.ToArray();
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return genres;
        }

        public int GetNumberOfAlbums(int? genreId, int? artistId)
        {
            int numberofAlbums = default(int);

            using (EntityConnection entityConnection =
                    new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {

                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("SELECT COUNT(a.Album_Id) FROM tunesEntities.albums AS a");

                        int genreID = genreId.GetValueOrDefault();
                        if (genreID > 0)
                        {
                            stringBuilder.Append(" WHERE a.Genre_Id = @genreID");
                            entityCommand.Parameters.Add(new EntityParameter
                            {
                                ParameterName = "genreID",
                                Value = genreID
                            });
                        }

                        int artistID = artistId.GetValueOrDefault();
                        if (artistID > 0)
                        {
                            stringBuilder.Append(" WHERE a.Artist_Id = @artistID");
                            entityCommand.Parameters.Add(new EntityParameter
                            {
                                ParameterName = "artistID",
                                Value = artistID
                            });
                        }

                        string sql = stringBuilder.ToString();
                        entityCommand.CommandText = sql;
                        object obj = entityCommand.ExecuteScalar();
                        if (obj is int)
                        {
                            numberofAlbums = (int)obj;
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return numberofAlbums;
        }
        public Album[] GetAlbums(int? genreId, int? artistId, int skip = 0, int limit = 10)
        {
            Album[] albums = null;

            using (EntityConnection entityConnection =
                new EntityConnection(ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("SELECT a.Artist_Id, a.Artist_Name, a.Artist_SortName, a.Album_Id, a.Album_Title, a.Album_AlbumId, a.Album_Year, a.Genre_Id, a.Genre_Name FROM tunesEntities.albums AS a");

                        int genreID = genreId.GetValueOrDefault();
                        if (genreID > 0)
                        {
                            stringBuilder.Append(" WHERE a.Genre_Id = @genreID");
                            entityCommand.Parameters.Add(new EntityParameter
                            {
                                ParameterName = "genreID",
                                Value = genreID
                            });
                        }

                        int artistID = artistId.GetValueOrDefault();
                        if (artistID > 0)
                        {
                            stringBuilder.Append(" WHERE a.Artist_Id = @artistID");
                            entityCommand.Parameters.Add(new EntityParameter
                            {
                                ParameterName = "artistID",
                                Value = artistID
                            });
                        }

                        stringBuilder.Append(" ORDER BY a.Artist_SortName, a.Album_Title");
                        stringBuilder.Append(" SKIP @skip LIMIT @limit ");

                        entityCommand.Parameters.Add(new EntityParameter
                        {
                            ParameterName = "skip",
                            Value = skip
                        });
                        entityCommand.Parameters.Add(new EntityParameter
                        {
                            ParameterName = "limit",
                            Value = limit
                        });

                        string sql = stringBuilder.ToString();
                        entityCommand.CommandText = sql;

                        albums = ExecuteAlbumReader(entityCommand);
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return albums;
        }

        public Album[] GetFeaturedAlbums(int limit = 10)
        {
            Album[] albums = null;
            var randomAlbumIDs = GetRandomizedAlbums(limit, GetAlbumsThatHavePlayableTracks().ToList())?.Take(limit);
            var idList = string.Join<int>(",", randomAlbumIDs);
            if (!string.IsNullOrEmpty(idList))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT a.Artist_Id, a.Artist_Name, a.Artist_SortName, a.Album_Id, a.Album_Title, a.Album_AlbumId, a.Album_Year, a.Genre_Id, a.Genre_Name FROM tunesEntities.albums AS a");
                stringBuilder.Append(" WHERE a.Album_Id IN {" + idList + "} ");
                string sql = stringBuilder.ToString();

                using (EntityConnection entityConnection = new EntityConnection(ConnectionString))
                {
                    try
                    {
                        entityConnection.Open();
                        using (EntityCommand entityCommand = entityConnection.CreateCommand())
                        {
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
            return albums;
        }

        public Album[] GetNewestAlbums(int limit = 10)
        {
            Album[] albums = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT a.Artist_Id, a.Artist_Name, a.Artist_SortName, a.Album_Id, a.Album_Title, a.Album_AlbumId, a.Album_Year, a.Genre_Id, a.Genre_Name FROM tunesEntities.albums AS a");
            stringBuilder.Append(" ORDER BY a.Album_Id DESC");
            stringBuilder.Append(" LIMIT @limit ");
            string sql = stringBuilder.ToString();

            using (EntityConnection entityConnection =
                new EntityConnection(ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        entityCommand.Parameters.Add(new EntityParameter
                        {
                            ParameterName = "limit",
                            Value = limit
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
            return albums;
        }
        public Album GetAlbumById(int albumId)
        {
            Album album = null;
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
                            ParameterName = "albumid",
                            Value = albumId
                        });

                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("SELECT a.Artist_Id, a.Artist_Name, a.Artist_SortName, a.Album_Id, a.Album_Title, a.Album_AlbumId, a.Album_Year, a.Genre_Id, a.Genre_Name FROM tunesEntities.albums AS a");
                        stringBuilder.Append(" WHERE a.Album_Id = @albumId");
                        entityCommand.CommandText = stringBuilder.ToString();

                        album = ExecuteAlbumReader(entityCommand).FirstOrDefault();
                        GetAlbumTracksByTitelId(album, entityConnection);
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return album;
        }

        public ICollection<Track> GetTopTracks(int skip = 0, int limit = 10)
        {
            Collection<Track> tracks = null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT COUNT(h.LiedID) AS number, h.LiedID, t.Lied ,t.Dauer, t.guid, a.TitelID, a.Titel1, a.Guid as AlbumId, i.Interpret");
            stringBuilder.Append(" FROM tunesEntities.history AS h");
            stringBuilder.Append(" JOIN tunesEntities.titel AS a ON h.titelid = a.TitelID");
            stringBuilder.Append(" JOIN tunesEntities.interpreten AS i ON a.interpretid = i.interpretid");
            stringBuilder.Append(" LEFT JOIN tunesEntities.lieder AS t ON h.liedid = t.liedid");
            stringBuilder.Append(" WHERE h.appid = 1");
            stringBuilder.Append(" AND ((Year(CurrentDateTime()) * 12 + Month(CurrentDateTime())) - (Year(h.zeit) * 12 + Month(h.zeit))) < 6");
            stringBuilder.Append(" GROUP BY h.LiedID, t.Lied ,t.Dauer,t.guid, a.TitelID, a.Titel1, a.Guid, i.Interpret");
            stringBuilder.Append(" ORDER BY number desc");
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
                            ParameterName = "skip",
                            Value = skip
                        });

                        entityCommand.Parameters.Add(new EntityParameter
                        {
                            ParameterName = "limit",
                            Value = limit
                        });

                        entityCommand.CommandText = sql;
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
                                    Id = dataReader.GetInt32("LiedID", false, 0),
                                    Name = dataReader.GetString("Lied", true, string.Empty),
                                    Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue),
                                    Guid = dataReader.GetGuid("guid", false, Guid.Empty),
                                    Album = new Album
                                    {
                                        Id = dataReader.GetInt32("TitelID", false, 0),
                                        Title = dataReader.GetString("Titel1", false, string.Empty),
                                        AlbumId = dataReader.GetGuid("AlbumId", false, Guid.Empty),
                                        Artist = new Artist
                                        {
                                            Name = dataReader.GetString("Interpret", false, string.Empty)
                                        }
                                    }
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


            return tracks;
        }
        public Track GetTrackById(int trackId)
        {
            Track track = null;
            string audioDirectory = AudioDirectory;
            if (string.IsNullOrEmpty(audioDirectory) == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer, t.Liedpfad, t.guid, a.TitelID, a.Titel1, a.Guid as AlbumId, i.Interpret FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" JOIN tunesEntities.titel AS a ON a.TitelID = t.TitelID");
                stringBuilder.Append(" JOIN tunesEntities.interpreten AS i ON a.InterpretID = i.InterpretID");
                stringBuilder.Append(" WHERE t.LiedId = @trackid");

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
                                ParameterName = "trackid",
                                Value = trackId
                            });

                            entityCommand.CommandText = sql;
                            // Execute the command.
                            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                            {
                                if (dataReader.Read() == true)
                                {
                                    track = new Track
                                    {
                                        Id = dataReader.GetInt32("LiedID", false, 0),
                                        TrackNumber = dataReader.GetInt32("Track", false, 0),
                                        Name = dataReader.GetString("Lied", false, string.Empty),
                                        Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue),
                                        Guid = dataReader.GetGuid("guid", false, Guid.Empty),
                                        Album = new Album
                                        {
                                            Id = dataReader.GetInt32("TitelID", false, 0),
                                            Title = dataReader.GetString("Titel1", false, string.Empty),
                                            AlbumId = dataReader.GetGuid("AlbumId", false, Guid.Empty),
                                            Artist = new Artist
                                            {
                                                Name = dataReader.GetString("Interpret", false, string.Empty)
                                            }
                                        }
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
            }
            return track;
        }
        public ICollection<int> GetTrackIdsByFilter(int? genreId)
        {
            Collection<int> tracks = null;

            using (EntityConnection entityConnection =
               new EntityConnection(this.ConnectionString))
            {
                try
                {
                    entityConnection.Open();
                    using (EntityCommand entityCommand = entityConnection.CreateCommand())
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("SELECT t.LiedID");
                        stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                        stringBuilder.Append(" INNER JOIN tunesEntities.titel AS a ON t.TitelID = a.TitelID");
                        stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");

                        int genreID = genreId.GetValueOrDefault();
                        if (genreID > 0)
                        {
                            stringBuilder.Append(" AND a.genreid = @genreID");
                            entityCommand.Parameters.Add(new EntityParameter
                            {
                                ParameterName = "genreID",
                                Value = genreID
                            });
                        }
                        entityCommand.CommandText = stringBuilder.ToString();
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
                            }
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }
            return tracks;
        }
        #endregion

        #region MethodsProtected
        protected Album[] ExecuteAlbumReader(EntityCommand entityCommand)
        {
            Album[] albums = null;
            List<Album> albumCollection = null;
            using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
            {
                // Start reading results.
                while (dataReader.Read())
                {
                    if (albumCollection == null)
                    {
                        albumCollection = new List<Album>();
                    }
                    Album album = new Album
                    {
                        Artist = new Artist
                        {
                            Id = dataReader.GetInt32("Artist_Id", false, 0),
                            Name = dataReader.GetString("Artist_Name", false, string.Empty),
                            SortName = dataReader.GetString("Artist_SortName", true, string.Empty)
                        },
                        Id = dataReader.GetInt32("Album_Id", false, 0),
                        Title = dataReader.GetString("Album_Title", false, string.Empty),
                        AlbumId = dataReader.GetGuid("Album_AlbumId", false, Guid.Empty),
                        Year = dataReader.GetInt32("Album_Year", true, 0),
                        Genre = new Genre
                        {
                            Id = dataReader.GetInt32("Genre_Id", true, 0),
                            Name = dataReader.GetString("Genre_Name", true, string.Empty)
                        }
                    };
                    albumCollection.Add(album);
                }
            }
            if (albumCollection != null)
            {
                albums = albumCollection.ToArray();
            }
            return albums;
        }

        protected void GetAlbumTracksByTitelId(Album album, EntityConnection entityConnection)
        {
            if (album != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("SELECT t.LiedID, t.Track, t.Lied ,t.Dauer");
                stringBuilder.Append(" FROM tunesEntities.lieder AS t");
                stringBuilder.Append(" WHERE t.titelid = @albumId");
                stringBuilder.Append(" AND t.Liedpfad IS NOT NULL");
                stringBuilder.Append(" ORDER BY t.Track");
                string sql = stringBuilder.ToString();

                using (EntityCommand entityCommand = entityConnection.CreateCommand())
                {
                    EntityParameter id = new EntityParameter();
                    id.ParameterName = "albumid";
                    id.Value = album.Id;
                    entityCommand.Parameters.Add(id);
                    entityCommand.CommandText = sql;

                    List<Track> tracks = null;
                    using (EntityDataReader dataReader = entityCommand.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        while (dataReader.Read())
                        {
                            if (tracks == null)
                            {
                                tracks = new List<Track>();
                            }
                            Track track = new Track
                            {
                                Id = dataReader.GetInt32("LiedID", false, 0),
                                TrackNumber = dataReader.GetInt32("Track", false, 0),
                                Name = dataReader.GetString("Lied", false, string.Empty),
                                Duration = dataReader.GetTimeSpan("Dauer", true, TimeSpan.MinValue)
                            };
                            tracks.Add(track);
                        }
                        if (tracks != null)
                        {
                            album.Tracks = tracks.ToArray();
                        }
                    }
                }
            }
        }

        protected ICollection<int> GetAlbumsThatHavePlayableTracks()
        {
            Collection<int> albums = new Collection<int>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT DISTINCT a.TitelID");
            stringBuilder.Append(" FROM tunesEntities.titel AS a");
            stringBuilder.Append(" INNER JOIN tunesEntities.lieder AS t ON a.TitelID = t.TitelID");
            stringBuilder.Append(" WHERE t.Liedpfad IS NOT NULL");
            string sql = stringBuilder.ToString();
            using (EntityConnection entityConnection =
                   new EntityConnection(ConnectionString))
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
                                albums.Add(dataReader.GetInt32("TitelID", false, 0));
                            }
                        }
                    }
                }
                finally
                {
                    entityConnection.Close();
                }
            }

            return albums;
        }
        protected ICollection<int> GetRandomizedAlbums(int limit, ICollection<int> albumIDs)
        {
            Collection<int> randomCollection = new Collection<int>();
            var albumIDCollection = new Collection<int>(albumIDs?.ToList());
            //            if (albumIDCollection.Count > limit)
            {
                //Random random = new Random(DateTime.Now.Millisecond);
                Random random = new Random(Guid.NewGuid().GetHashCode());
                while (albumIDCollection.Count > 0)
                {
                    int index = random.Next(albumIDCollection.Count);
                    int albumID = albumIDCollection[index];
                    randomCollection.Add(albumID);
                    albumIDCollection.RemoveAt(index);
                }
                albumIDs = randomCollection;
            }
            return albumIDs;
        }
        #endregion
    }
}
