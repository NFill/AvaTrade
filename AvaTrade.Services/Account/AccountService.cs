using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using AvaTrade.Data;
using AvaTrade.Data.Repositories;
using AvaTrade.Domain.Entities;

namespace AvaTrade.Services.Account
{
  public class AccountService : IAccountService
  {
    private readonly IRepository<UserRefreshToken> userRefTokenRepo;
    private readonly IConfiguration configuration;
    private readonly IChanges changes;
    private readonly UserManager<AppUser> userManager;

    public AccountService(IRepository<UserRefreshToken> userRefTokenRepo, 
                          IConfiguration configuration,
                          IChanges changes,
                          UserManager<AppUser> userManager)
    {
      this.userRefTokenRepo = userRefTokenRepo;
      this.configuration = configuration;
      this.changes = changes;
      this.userManager = userManager;
    }

    public async Task<CreateUserResult> RegisterUser(AuthRequest authRequest)
    {
      AppUser user = await userManager.FindByNameAsync(authRequest.UserName);
      if (user != null)
      {
        return await Task.FromResult(new CreateUserResult { Succeeded = false, Created=false, Message="User exist" });
      }

      user = new AppUser();
      user.UserName = authRequest.UserName;

      IdentityResult result = await userManager.CreateAsync(user, authRequest.Password);
      if (result.Succeeded)
      {
        return await Task.FromResult(new CreateUserResult { Succeeded = true, Created=true, Message = "User created" });
      }
      else
      {
        return await Task.FromResult(new CreateUserResult { Succeeded = false, Created=false, Message = result.Errors });
      }
    }

    public async Task<AuthResponse> GetTokenAsync(AuthRequest authRequest, string ipAddress)
    {
      AppUser user = await userManager.FindByNameAsync(authRequest.UserName);
      if (user == null)
      {
        return await Task.FromResult<AuthResponse>(null);
      }
      bool validPassword = await userManager.CheckPasswordAsync(user, authRequest.Password);
      if (!validPassword)
      {
        return await Task.FromResult<AuthResponse>(null);
      }

      string tokenString = generateToken();
      string refreshTokenString = generateRefreshToken();
      await setNewTokenDetails(ipAddress, user.Id, tokenString, refreshTokenString);
      await changes.Commit();
      return new AuthResponse { Token = tokenString, RefreshToken = refreshTokenString, IsSuccess = true };
    }

    public async Task<AuthResponse> GetRefreshTokenAsync(RefreshTokenRequest request)
    {
      var userRefreshToken = await userRefTokenRepo
        .FirstOrDefaultAsync(x => 
             x.IsInvalidated == false
          && x.Token == request.ExpiredToken
          && x.RefreshToken == request.RefreshToken
          && x.IpAddress == request.IpAddress);

      JwtSecurityToken token = getJwtToken(request.ExpiredToken);
      AuthResponse response = validateDetails(token, userRefreshToken);
      if (!response.IsSuccess)
      {
        return await Task.FromResult<AuthResponse>(null);
      }

      userRefreshToken.IsInvalidated = true;

      string tokenString = generateToken();
      string refreshTokenString = generateRefreshToken();
      await setNewTokenDetails(request.IpAddress, userRefreshToken.AppUserId, tokenString, refreshTokenString);
      await changes.Commit();
      return new AuthResponse { Token = tokenString, RefreshToken = refreshTokenString, IsSuccess = true };
    }

    public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
    {
      var isValid = await userRefTokenRepo
                          .FirstOrDefaultAsync
                          (x => x.Token == accessToken
                           && x.IpAddress == ipAddress) != null;
      return await Task.FromResult(isValid);
    }

    private string generateToken()
    {
      byte[] keyBytes = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
      SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
      {
        Expires = DateTime.Now.AddSeconds(90),
        Audience = configuration["Jwt:Audience"],
        Issuer = configuration["Jwt:Issuer"],
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
      };

      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken token = tokenHandler.CreateToken(descriptor);
      string tokenString = tokenHandler.WriteToken(token);
      return tokenString;
    }
    private string generateRefreshToken()
    {
      using (var generator = RandomNumberGenerator.Create())
      {
        byte[] bytes = new byte[64];
        generator.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
      }
    }
    private async Task setNewTokenDetails(string ipAddress, int userId, string tokenString, string refreshTokenString)
    {
      var userRefreshToken = new UserRefreshToken
      {
        CreatedDate = DateTime.Now,
        ExpirationDate = DateTime.Now.AddMinutes(5),
        IpAddress = ipAddress,
        IsInvalidated = false,
        RefreshToken = refreshTokenString,
        Token = tokenString,
        AppUserId = userId
      };
      await userRefTokenRepo.AddAsync(userRefreshToken);
    }
    private AuthResponse validateDetails(JwtSecurityToken token, UserRefreshToken userRefreshToken)
    {
      if (userRefreshToken == null)
      {
        return new AuthResponse { IsSuccess = false, Reason = "Invalid Token Details." };
      }

      if (token.ValidTo > DateTime.Now)
      {
        return new AuthResponse { IsSuccess = false, Reason = "Token not expired." };
      }

      if (!userRefreshToken.IsActive)
      {
        return new AuthResponse { IsSuccess = false, Reason = "Refresh Token Expired" };
      }

      return new AuthResponse { IsSuccess = true };
    }
    private JwtSecurityToken getJwtToken(string expiredToken)
    {
      JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
      JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(expiredToken);
      return jwtSecurityToken;
    }
  }
}
