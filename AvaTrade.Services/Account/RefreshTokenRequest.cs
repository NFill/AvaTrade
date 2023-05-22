using System.ComponentModel.DataAnnotations;

namespace AvaTrade.Services.Account
{
  public class RefreshTokenRequest
  {
    //TODO use several classes without redundant properties
    [Required]
    public string ExpiredToken { get; set; }
    [Required]
    public string RefreshToken { get; set; }
    public string IpAddress { get; set; }
    public int AppUserId { get; set; }
  }
}
