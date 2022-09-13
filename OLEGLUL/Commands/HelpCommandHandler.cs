using OLEGLUL.Models;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;

namespace OLEGLUL.Commands
{
    /// <summary>
    /// The <see cref="HelpCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
    /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
    /// </summary>
    public class HelpCommandHandler : ITeamsCommandHandler
    {
        private readonly ILogger<HelpCommandHandler> _logger;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "HelpCommandCard.json");

        public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            // Used to trigger the command handler if the command text contains 'Help'
            new RegExpTrigger("Help")
        };

        public HelpCommandHandler(ILogger<HelpCommandHandler> logger)
        {
            _logger = logger;
        }

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation($"Bot received message: {message.Text}");

            // Read adaptive card template
            var cardTemplate = await File.ReadAllTextAsync(_adaptiveCardFilePath, cancellationToken);

            // Render adaptive card content
            var cardContent = new AdaptiveCardTemplate(cardTemplate).Expand
            (
                new HelpModel
                {
                    Title = "Type: Url for Url scan \n Type: Ping for Ping Scan \n Type: File for File scan",
                    Body = "If you want more information click the documentation below to learn more about Bots and the Teams Toolkit.",
                }
            );

            // Build attachment
            var activity = MessageFactory.Attachment
            (
                new Attachment
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(cardContent),
                }
            );

            // send response
            return new ActivityCommandResponse(activity);
        }
    }
}
