using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AvaTrade.Services.Account;

namespace AvaTrade.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AccountController : ControllerBase
  {
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
      _accountService = accountService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] AuthRequest authRequest)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new AuthResponse { IsSuccess = false, Reason = "UserName and Password error" });
      }

      CreateUserResult requestResult = await _accountService.RegisterUser(authRequest);
      if (requestResult.Succeeded && requestResult.Created)
      {
        return StatusCode(StatusCodes.Status201Created, requestResult.Message);
      }
      if (requestResult.Succeeded && !requestResult.Created)
      {
        return StatusCode(StatusCodes.Status403Forbidden, requestResult.Message );
      }
      return StatusCode(StatusCodes.Status206PartialContent, requestResult.Message);
    }


    [HttpPost("[action]")]
    public async Task<IActionResult> AuthToken([FromBody] AuthRequest authRequest)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new AuthResponse { IsSuccess = false, Reason = "UserName and Password error" });
      }

      AuthResponse authResponse = await _accountService.GetTokenAsync(authRequest, HttpContext.Connection.RemoteIpAddress.ToString());

      if (authResponse == null)
      {
        return Unauthorized();
      }
      return Ok(authResponse);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(new AuthResponse { IsSuccess = false, Reason = "Tokens must be provided" });
      }
     
      string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
      request.IpAddress = ipAddress;
      AuthResponse authResponse = await _accountService.GetRefreshTokenAsync(request);
      return Ok(authResponse);
    }
  }
}