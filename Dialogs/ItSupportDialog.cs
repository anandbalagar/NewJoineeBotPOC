using EchoBot1.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoBot.Dialogs.Operations
{
    public class ItSupportDialog : ComponentDialog
    {
        public ItSupportDialog() : base(nameof(ItSupportDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                WelcomeStepAsync,
                ProvideOptionsStepAsync,
                ProcessOptionStepAsync,
                FeedbackStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> WelcomeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Welcome to IT Support!"), cancellationToken);
            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> ProvideOptionsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var videoCard = new VideoCard
            {
                Title = "How to Raise a Support Ticket",
                Subtitle = "Watch this video to know how to raise a support ticket",
                Text = "To raise a support ticket, please watch the above video. After watching the video, you can access the ticket portal to raise your ticket from the given below link.",
                Image = new ThumbnailUrl
                {
                    Url = "https://chatbot897.blob.core.windows.net/chatbotimage/it support.PNG",
                },
                Media = new List<MediaUrl>
                {
                    new MediaUrl()
                    {
                        Url = "https://chatbot897.blob.core.windows.net/chatbotimage/Call with Chavan, and 1 other-20240131_162212-Meeting Recording.mp4",
                    },
                },
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Title = "Access Ticket Portal",
                        Type = ActionTypes.OpenUrl,
                        Value = "https://fnf.service-now.com/sp",
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


        private async Task<DialogTurnResult> ProcessOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var heroCard = new HeroCard
            {
                Text = "For further assistance, please contact IT support via email at it_helpdesk@fnf.com.",

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

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for working with our Bot. Would you like to provide feedback on your experience?") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> FeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {

                return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), null, cancellationToken: cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(MainDialog), cancellationToken: cancellationToken);
            }
        }




    }
}
