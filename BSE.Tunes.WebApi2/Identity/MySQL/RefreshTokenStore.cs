using AspNet.Identity.MySQL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Identity.MySQL
{
    public class RefreshTokenStore : IRefreshTokenStore<RefreshToken>
    {
        private readonly MySQLDatabase _database;
        private readonly RefreshTokenTable _refreshTokenTable;

        public RefreshTokenStore()
        {
        }

        public RefreshTokenStore(MySQLDatabase database)
        {
            _database = database;
            _refreshTokenTable = new RefreshTokenTable(database);
        }

        public void Dispose()
        {
            if (_database != null)
            {
                _database.Dispose();
            }
        }

        public Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            return Task.FromResult(_refreshTokenTable.GetTokenById(refreshTokenHandle));
        }

        public Task<bool> RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            return Task.FromResult(_refreshTokenTable.Delete(refreshTokenHandle) > 0);
        }

        public Task<bool> StoreRefreshTokenAsync(RefreshToken refreshToken)
        {

            try
            {
                return Task.FromResult(_refreshTokenTable.Insert(refreshToken)>0);

            }
            catch(Exception ex)
            {

            }
            //finally
            {
                return Task.FromResult(false);
            }
        }
    }
}