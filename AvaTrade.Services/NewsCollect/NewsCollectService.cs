using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using AvaTrade.Data;
using AvaTrade.Domain.Entities;

namespace AvaTrade.Services.NewsCollect
{
  public class NewsCollectService : BackgroundService
  {
    //TODO Move magic strings
    private readonly ILogger<NewsCollectService> _logger;
    private IServiceScopeFactory _serviceScopeFactory;

    public NewsCollectService(IServiceScopeFactory serviceScopeFactory, ILogger<NewsCollectService> logger)
    {
      _serviceScopeFactory = serviceScopeFactory;
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      //TODO: Check if news already added
      using (HttpClient client = new HttpClient())
      {
        while (!stoppingToken.IsCancellationRequested)
        {
          _logger.LogInformation("BackgroundService running at: {time}", DateTime.Now);

          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "qkRZejt0ljUI7GBEKyphy6ZkvyCs_9f2");
          string url = "https://api.polygon.io/v2/reference/news?order=desc&sort=published_utc";
          using (HttpResponseMessage response = await client.GetAsync(url))
          {
            if (response.IsSuccessStatusCode)
              try
              {
                string JsonContent = await response.Content.ReadAsStringAsync();
                int startIndex = JsonContent.IndexOf("[");
                int endIndex = JsonContent.LastIndexOf("]");
                int lenght = endIndex - startIndex + 1;
                JsonContent = JsonContent.Substring(startIndex, lenght);
                List<News> news = JsonConvert.DeserializeObject<List<News>>(JsonContent);
                setData(news);
                using (AvaTradeDbContext _context = _serviceScopeFactory
                                                    .CreateScope()
                                                    .ServiceProvider
                                                    .GetRequiredService<AvaTradeDbContext>())
                {
                  await _context.News.AddRangeAsync(news);
                  await _context.SaveChangesAsync();
                }
              }
              catch (Exception ex)
              {
                _logger.LogInformation("BackgroundService error: {error}", ex.Message);
              }
          }
          await Task.Delay((int)TimeSpan.FromMinutes(60).TotalMilliseconds, stoppingToken);
        }
        
      }
    }


    private void setData(List<News> news)
    {
      for (int i = 0; i < news.Count; i++)
      {
        if (news[i].KeywordsJson != null)
        {
          news[i].Keywords = string.Join(",", news[i].KeywordsJson);
        }
        if (news[i].TickersJson != null)
        {
          news[i].Tickers = string.Join(",", news[i].TickersJson);
        }
      }
    }
  }
}