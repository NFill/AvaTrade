using System.Threading.Tasks;

namespace AvaTrade.Data
{
  public interface IChanges
  {
    Task<int> Commit();
  }
}
