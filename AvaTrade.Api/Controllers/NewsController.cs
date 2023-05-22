using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using AvaTrade.Data;
using AvaTrade.Services.Newses;
using Microsoft.AspNetCore.Authorization;

namespace AvaTrade.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  // TODO: Validate request parameters and response objects like AccountController
  // TODO: Return actual ActionResult like AccountController
  public class NewsController : ControllerBase
  {
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService, AvaTradeDbContext context)
    {
      _newsService = newsService;
    }
        
    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllNews()
    {
      var news = await _newsService.GetAllNewsAsync();
      return Ok(news);
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllNewsLastDays(int days)
    {
      var news = await _newsService.GetAllNewsLastDays(days);
      return Ok(news);
    }

    [Authorize]
    [HttpGet("[action]")]
    //Get all news per instrument name include news limit(default limit = 10). What is instrument?
    // I will use keyword instead instrument
    public async Task<IActionResult> GetNewsPerKeyword(string keyword, int limit = 10)
    {
      var news = await _newsService.GetNewsPerKeyword(keyword, limit);
      return Ok(news);
    }

    [Authorize]
    [HttpGet("[action]")]
    //???
    public async Task<IActionResult> GetNewsByText(string text)
    {
      await Task.CompletedTask;
      return Ok();
    }

    //[Authorize] Allow customer to subscribe. - Can't understand this....
    //[Public] Get latest new (top latest 5 different instruments) for conversion tool. - Where is the tool?
  }
}
