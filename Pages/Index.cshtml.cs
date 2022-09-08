using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using RestSharp;
using Newtonsoft.Json.Linq;
using Utils;
using Microsoft.Extensions.Caching.Memory;


namespace aspnet_api_request.Pages;

public class IndexModel : PageModel
{
    private MemoryCache _cache;

    private readonly ILogger<IndexModel> _logger;

    private readonly String apiUrlTemplate = "https://api.openweathermap.org/data/2.5/weather?q=Stuttgart,de&units=metric&APPID={API_KEY}";

    public string Condition { get; set; }
    public string Temperature { get; set; }


    public IndexModel(ILogger<IndexModel> logger, IMemoryCacheService cache)
    {
        Condition = "...";
        Temperature = "...";
        _logger = logger;
        _cache = cache.Cache;
    }

    public async Task OnGet() // async Task, not async void!
    {
        if (!_cache.TryGetValue("Stuttgart", out JObject jo))
        {
            _logger.LogInformation("no cache, calling api");
            var config =
                new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .Build();

            Uri apiUrl = new Uri(apiUrlTemplate.Replace("{API_KEY}", config["API_KEY"]));

            RestClient client = new RestClient(apiUrl);
            var request = new RestRequest();

            var response = await client.GetAsync(request);

            if (response.IsSuccessful & response.Content is not null)
            {
                jo = JObject.Parse(response.Content!.ToString());
                _logger.LogInformation("call");
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSize(1).SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set("Stuttgart", jo, cacheEntryOptions);
                client.Dispose();
            }
            else
            {
                _logger.LogError("{0} ({1})", (int)response.StatusCode, response.ErrorMessage);
            }
        }
        else
        {
            _logger.LogInformation("cached values");
        }

        if (jo["weather"] is not null & jo["weather"]![0] is not null & jo["weather"]![0]!["description"] is not null)
        {
            Condition = jo["weather"]![0]!["description"]!.ToString() ?? "";
        }

        if (jo["main"] is not null & jo["main"]!["temp"] is not null)
        {
            Temperature = Math.Round((float)jo["main"]!["temp"]!, 1).ToString();
        }
    }
}

