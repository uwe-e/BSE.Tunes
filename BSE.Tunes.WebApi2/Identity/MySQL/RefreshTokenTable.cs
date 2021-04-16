using AspNet.Identity.MySQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSE.Tunes.WebApi.Identity.MySQL
{
    public class RefreshTokenTable
    {
        private readonly MySQLDatabase _database;

        public RefreshTokenTable(MySQLDatabase database)
        {
            _database = database;
        }
        public int Delete(string refreshTokenHandle)
        {
            string commandText = "Delete from RefreshTokens where Id = @tokenId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@tokenId", refreshTokenHandle);

            return _database.Execute(commandText, parameters);
        }

        public RefreshToken GetTokenById(string refreshTokenHandle)
        {
            RefreshToken token = null;

            string commandText = "Select * from RefreshTokens where Id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", refreshTokenHandle } };

            var rows = _database.Query(commandText, parameters);
            if (rows != null && rows.Count == 1)
            {
                var row = rows[0];
                token = new RefreshToken
                {
                    Id = row["Id"],
                    UserName = row["UserName"],
                    SubjectId = row["SubjectId"],
                    CreationTime = DateTime.Parse(row["CreationTime"]),
                    LifeTime = int.Parse(row["LifeTime"]),
                    SerializedTicket = row["SerializedTicket"]
                };
            }

            return token;
        }

        public int Insert(RefreshToken token) {
            
            string commandText = "Insert into RefreshTokens (Id, UserName, SubjectId, CreationTime, LifeTime, SerializedTicket) values ( @id, @name, @subjectId, @creationTime, @lifeTime, @ticket)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", token.Id);
            parameters.Add("@name", token.UserName);
            parameters.Add("@subjectId", token.SubjectId);
            parameters.Add("@creationTime", token.CreationTime);
            parameters.Add("@lifeTime", token.LifeTime);
            parameters.Add("@ticket", token.SerializedTicket);

            return _database.Execute(commandText, parameters);
        }
    }
}