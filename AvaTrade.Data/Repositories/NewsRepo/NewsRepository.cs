using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using AvaTrade.Domain.Entities;

namespace AvaTrade.Data.Repositories.NewsRepo
{
  public class NewsRepository : Repository<News>, INewsRepository
  {
    public NewsRepository(AvaTradeDbContext context) : base(context)
    {
    }
    // TODO: Create projectin an DTO objects
    public async Task<IEnumerable<News>> GetAllNewsAsync()
    {
      List<News> news = await context.News
                              .Include(n => n.Publisher)
                              .AsNoTracking()
                              .ToListAsync();
      return news;
    }

    public async Task<IEnumerable<News>> GetAllNewsLastDays(int days)
    {
      List<News> news = await context.News
                              .Where(n => n.PublishedUtc >= DateTime.Today.AddDays(-days))
                              .Include(n => n.Publisher)
                              .AsNoTracking()
                              .ToListAsync();
      return news;
    }

    public async Task<IEnumerable<News>> GetNewsPerKeyword(string keyword, int limit)
    {
      List<News> news = await context.News.Where(n => n.Keywords.ToLower().Contains(keyword.ToLower()))
                              .Include(n => n.Publisher)
                              .Take(limit)
                              .AsNoTracking()
                              .ToListAsync();
      return news;
    }
  }
}
