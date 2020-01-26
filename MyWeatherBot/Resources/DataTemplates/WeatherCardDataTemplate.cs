using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWeatherBot.Resources.DataTemplates
{
    public class WeatherCardDataTemplate
    {
        public string CityName { get; set; }
        public string Date { get; set; }
        public string Humidity { get; set; }
        public string WindSpeed { get; set; }
        public string Cloudiness { get; set; }
        public string Description { get; set; }
    }
}
