using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using MyWeatherBot.Properties;
using System.Web;
using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Schema;
using AdaptiveCards.Templating;
using MyWeatherBot.Resources.DataTemplates;
using Newtonsoft.Json;
using System.IO;

namespace MyWeatherBot.Services.Impl
{
    public class WeatherServiceImpl : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherSettings _weatherSettings;
        private readonly string WeatherCardPath = Path.Combine(".", "Resources", "WeatherCard.json");
            //"..\\..\\Resources\\WeatherCard.json";

        public WeatherServiceImpl(HttpClient httpClient, IOptions<WeatherSettings> opWeatherSettings)
        {
            _httpClient = httpClient;
            _weatherSettings = opWeatherSettings.Value;
        }
        public async Task<Attachment> GetWeatherForCity(string cityName)
        {
            var weatherUri = buildGetWeatherUri(cityName);
            using var request = new HttpRequestMessage(HttpMethod.Get, weatherUri);
            using var response = await _httpClient.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                var jsonStr = await response.Content.ReadAsStringAsync();
                dynamic jsonObj = JObject.Parse(jsonStr);
                var conditionsDescription = new List<string>();
                foreach(dynamic cond in jsonObj.weather)
                {
                    conditionsDescription.Add(cond.main.ToObject<string>());
                }

                var data = new WeatherCardDataTemplate()
                {
                    CityName = jsonObj.name.ToObject<string>(),
                    Date = DateTime.UtcNow.AddSeconds(jsonObj.timezone.ToObject<double>()).ToString("dddd, dd MMMM yyyy"),
                    Humidity = jsonObj.main.humidity.ToObject<string>(),
                    WindSpeed = jsonObj.wind.speed.ToObject<string>(),
                    Cloudiness = jsonObj.clouds.all.ToObject<string>(),
                    Description = string.Join(',', conditionsDescription.ToArray())
                };

                var dataJson = JsonConvert.SerializeObject(data);

                var templateJson = File.ReadAllText(WeatherCardPath);

                return buildWeatherCard(templateJson, dataJson);
            }
        }

        private Attachment buildWeatherCard(string templateJson, string dataJson)
        {
            var transformer = new AdaptiveTransformer();
            var cardJson = transformer.Transform(templateJson, dataJson);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }

        private Uri buildGetWeatherUri(string cityName)
        {
            var encodedCityName = HttpUtility.UrlEncode(cityName);
            return new Uri(
                $"{_weatherSettings.WeatherEndpoint}?q={encodedCityName}&units={_weatherSettings.Units}&APPID={_weatherSettings.APPID}"
            );
        }
    }
}
