using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoBot.Dialogs.Operations
{
    public class FeedbackDialog : ComponentDialog
    {
        public FeedbackDialog() : base(nameof(FeedbackDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                StartStepAsync,
                //DisplayFeedbackStepAsync
                //CommentStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        //private async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your first name") }, cancellationToken);

        //    //var cardInstance = new CardTypes();
        //    //var menuCard = cardInstance.CreateAdaptiveCardAttachment("feedbackadaptive.json");
        //    //var response = MessageFactory.Attachment(menuCard, ssml: "Welcome to my menu items!");
        //    //return null;

        //    return await stepContext.PromptAsync(nameof(ChoicePrompt),
        //     new PromptOptions
        //     {
        //         Prompt = MessageFactory.Text("Rate us a number  out of 5"),
        //         Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3", "4", "5" }),
        //     }, cancellationToken);
        //}

        private async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //stepContext.Values["rate"] = ((FoundChoice)stepContext.Result).Value;

            var receiptCard = CardTypes.GetAdaptiveCardFeedback();
            var response = MessageFactory.Attachment(receiptCard, ssml: "Welcome to my Bot!");

            await stepContext.Context.SendActivityAsync(response, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            //return null;
        }

        //private async Task<DialogTurnResult> DisplayFeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    // Retrieve the user's selected rating and comments
        //    var result = (JObject)stepContext.Result;
        //    var rating = result["rating"]?.ToString();
        //    var comments = result["comments"]?.ToString();

        //    // Display the user's feedback
        //    var responseMessage = $"Thank you for providing feedback!\n\nRating: {rating}\nComments: {(string.IsNullOrWhiteSpace(comments) ? "N/A" : comments)}";

        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(responseMessage), cancellationToken);

        //    // You can perform any additional actions or logic here if needed

        //    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        //}

    }
}
