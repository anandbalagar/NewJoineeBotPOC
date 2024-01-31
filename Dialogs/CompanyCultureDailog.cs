using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;

namespace ToDoBot.Dialogs.Operations
{
    public class CompanyCultureDialog : ComponentDialog
    {
        public CompanyCultureDialog() : base(nameof(CompanyCultureDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                WelcomeStepAsync,
                ShowImageGalleryStepAsync,
                ShowVideoCardStepAsync,
                EndDialogStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           // await stepContext.Context.SendActivityAsync(MessageFactory.Text("Welcome to our Company Culture! We're excited to have you on board."), cancellationToken);
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ShowImageGalleryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var welcomeCard = CardTypes.CreateAdaptiveCardAttachment("ImageGalleryCard.json");
            var welcomeResponse = MessageFactory.Attachment(welcomeCard, ssml: "Welcome to the Bot!");
            await stepContext.Context.SendActivityAsync(welcomeResponse, cancellationToken);


            return await stepContext.NextAsync();
        }

        //private Attachment CreateAdaptiveCardAttachment(string cardFileName)
        //{
        //    var cardPath = Path.Combine("cards", cardFileName);
        //    var cardJson = File.ReadAllText(cardPath);

        //    return new Attachment()
        //    {
        //        ContentType = "application/vnd.microsoft.card.adaptive",
        //        Content = JsonConvert.DeserializeObject(cardJson, new JsonSerializerSettings { MaxDepth = null }),
        //    };
        //}

        private async Task<DialogTurnResult> ShowVideoCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var videoCard = new VideoCard
            {
                Title = "Company Culture Video",
                Subtitle = "Why our company is a great place to work",
                Text = "Our company values diversity, collaboration, and innovation. Watch this video to learn more about our culture.",
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://youtu.be/5oO2DsKBF98?si=FYwD5axPSLqc-MKz",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Learn More",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://fnfindia.co.in/life_at_fnf.htm",
                    },
                },
            };

            var videoAttachment = new Attachment
            {
                ContentType = VideoCard.ContentType,
                Content = videoCard,
            };

            var prompt = MessageFactory.Attachment(videoAttachment);
            await stepContext.Context.SendActivityAsync(prompt, cancellationToken);

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            var heroCard = new HeroCard
            {
                Text = "We hope you are excited about joining our fantastic company culture! If you have any questions, feel free to ask..",

                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Main Menu",
                        Type = ActionTypes.ImBack,
                        Value = "Main Menu",
                    },
                },
            };

            // Attach the Hero Card to the response
            var attachment = heroCard.ToAttachment();
            var message = MessageFactory.Attachment(attachment);

            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            return await stepContext.EndDialogAsync();

        }
    }
}
