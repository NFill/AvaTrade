using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvaTrade.Domain.Entities
{
  public class UserRefreshToken : BaseEntity
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    public string IpAddress { get; set; }
    public bool IsInvalidated { get; set; }

    public int AppUserId { get; set; }
    public virtual AppUser User
    {
      get; set;
    }

    [NotMapped]
    public bool IsActive
    {
      get
      {
        return ExpirationDate > DateTime.Now;
      }
    }

  }
}

