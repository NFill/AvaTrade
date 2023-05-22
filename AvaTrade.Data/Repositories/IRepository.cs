using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AvaTrade.Domain.Entities;

namespace AvaTrade.Data.Repositories
{
  public interface IRepository<T> where T : BaseEntity
  {
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);
  }
}
