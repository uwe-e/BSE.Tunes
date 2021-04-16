using System;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Identity
{
    public class RefreshTokenManager<TRefreshToken> : IDisposable
        where TRefreshToken : class
    {
        private readonly IRefreshTokenStore<TRefreshToken> _store;

        public RefreshTokenManager(IRefreshTokenStore<TRefreshToken> store)
        {
            _store = store;
        }

        public async Task<bool> StoreRefreshToken(RefreshToken token)
        {
            return await _store.StoreRefreshTokenAsync(token);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle)
        {
            return await _store.GetRefreshTokenAsync(refreshTokenHandle);
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshTokenHandle)
        {
            return await _store.RemoveRefreshTokenAsync(refreshTokenHandle);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}