using System.Threading.Tasks;

namespace AvaTrade.Services.Account
{
  public interface IAccountService
  {
    Task<CreateUserResult> RegisterUser(AuthRequest authRequest);
    Task<AuthResponse> GetTokenAsync(AuthRequest authRequest, string ipAddress);
    Task<AuthResponse> GetRefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> IsTokenValid(string accessToken, string ipAddress);
  }
}