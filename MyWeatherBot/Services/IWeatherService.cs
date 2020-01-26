using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace MyWeatherBot.Services
{
    public interface IWeatherService
    {
        Task<Attachment> GetWeatherForCity(string cityName);
    }
}
