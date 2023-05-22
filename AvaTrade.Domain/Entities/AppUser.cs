using Microsoft.AspNetCore.Identity;

namespace AvaTrade.Domain.Entities
{
  public class AppUser : IdentityUser<int>
  {
    public string IpAddress { get; set; }
  }
}
