using Newtonsoft.Json;

namespace AvaTrade.Domain.Entities
{
  public class Publisher : BaseEntity
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("homepage_url")]
    public string HomepageUrl { get; set; }

    [JsonProperty("logo_url")]
    public string LogoUrl { get; set; }

    [JsonProperty("favicon_url")]
    public string FaviconUrl { get; set; }
  }
}
