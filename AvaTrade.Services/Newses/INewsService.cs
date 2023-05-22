using System.Collections.Generic;
using System.Threading.Tasks;

using AvaTrade.Domain.Entities;

namespace AvaTrade.Services.Newses
{
  public interface INewsService
  {
    Task<IEnumerable<News>> GetAllNewsAsync();
    Task<IEnumerable<News>> GetAllNewsLastDays(int days);
    Task<IEnumerable<News>> GetNewsPerKeyword(string keyword, int limit);
  }
}