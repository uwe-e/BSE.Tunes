using System;
using System.Threading.Tasks;

namespace BSE.Tunes.WebApi.Identity
{
    public interface IRefreshTokenStore<TRefeshToken> : IDisposable where TRefeshToken : class
    {
        Task<bool> StoreRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken> GetRefreshTokenAsync(string refreshTokenHandle);

        Task<bool> RemoveRefreshTokenAsync(string refreshTokenHandle);
    }
}
