using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AvaTrade.Domain.Entities
{
  public class News : BaseEntity
  {
    [JsonProperty("id")]
    public string ApiNewsId { get; set; }

    public int PublisherId { get; set; }

    [JsonProperty("publisher")]

    public Publisher Publisher { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }

    [JsonProperty("published_utc")]
    public DateTime PublishedUtc { get; set; }

    [JsonProperty("article_url")]
    public string ArticleUrl { get; set; }

    [NotMapped]
    [JsonProperty("tickers")]
    public string[] TickersJson { get; set; }

    public string Tickers { get; set; }

    [JsonProperty("amp_url")]
    public string AmpUrl { get; set; }

    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [NotMapped]
    [JsonProperty("keywords")]
    public string[] KeywordsJson { get; set; }

    public string Keywords { get; set; }
  }
}
