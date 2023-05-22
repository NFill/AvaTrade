using System.ComponentModel.DataAnnotations;

namespace AvaTrade.Services.Account
{
  public class AuthRequest
  {
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
  }
}
