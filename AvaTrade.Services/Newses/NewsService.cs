using System.Collections.Generic;
using System.Threading.Tasks;

using AvaTrade.Domain.Entities;
using AvaTrade.Data.Repositories.NewsRepo;

namespace AvaTrade.Services.Newses
{
  public class NewsService : INewsService
  {
    private readonly INewsRepository _newsRepo;

    public NewsService(INewsRepository newsRepo)
    {
      _newsRepo = newsRepo;
    }

    public async Task<IEnumerable<News>> GetAllNewsAsync()
    {
      var all = await _newsRepo.GetAllNewsAsync();
      return all;
    }

    public async Task<IEnumerable<News>> GetAllNewsLastDays(int days)
    {
      var newsForlastDays = await _newsRepo.GetAllNewsLastDays(days);
      return newsForlastDays;
    }

    public async Task<IEnumerable<News>> GetNewsPerKeyword(string keyword, int limit=10)
    {
      var newsByKeyword = await _newsRepo.GetNewsPerKeyword(keyword,  limit);
      return newsByKeyword;
    }
  }
}