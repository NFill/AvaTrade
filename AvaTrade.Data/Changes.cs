using System.Threading.Tasks;

namespace AvaTrade.Data
{
  public class Changes : IChanges
  {
    private readonly AvaTradeDbContext context;

    public Changes(AvaTradeDbContext context)
    {
      this.context = context;
    }

    public async Task<int> Commit()
    {
      return await context.SaveChangesAsync();
    }
  }
}
