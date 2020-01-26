using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using MyWeatherBot.Services;

namespace MyWeatherBot.Bots
{
    public class MyWeatherBot : ActivityHandler
    {
        private readonly IWeatherService _weatherService;

        public MyWeatherBot(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var cityName = turnContext.Activity.Text;
            var weatherCard = await _weatherService.GetWeatherForCity(cityName);
            IActivity replyActivity = null;

            if (weatherCard == null)
            {
                replyActivity = MessageFactory.Text( $"The city \"{cityName}\" was not found. Try again:)");
            }
            else
            {
                replyActivity = MessageFactory.Attachment(weatherCard);
            }
            await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text("Type the name of the next city!"));
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!\nPlease, type the city";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
