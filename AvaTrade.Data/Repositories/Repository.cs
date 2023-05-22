using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using AvaTrade.Domain.Entities;

namespace AvaTrade.Data.Repositories
{
  public class Repository<T> : IRepository<T> where T : BaseEntity
  {

    protected readonly AvaTradeDbContext context;
    protected readonly DbSet<T> dbSet;
    public Repository(AvaTradeDbContext context)
    {
      this.context = context;
      dbSet = context.Set<T>();
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
    {
      T data = await dbSet.FirstOrDefaultAsync(where);
      return data;
    }
    public async Task AddAsync(T entity)
    {
      await dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
       await dbSet.AddRangeAsync(entities);
    }
  }
}
