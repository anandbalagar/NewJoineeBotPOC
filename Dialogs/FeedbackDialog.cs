using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using NewJoineeBOT.Models;
using NewJoineeBOT.Utility;
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
        UserRepository userRepository;

        public FeedbackDialog(UserRepository _userRepository) : base(nameof(FeedbackDialog))
        {
            userRepository = _userRepository;

            var waterfallSteps = new WaterfallStep[]
            {
                StartStepAsync,
                SecondStepAsync,
                ThirdStepAsync,
                FourthStepAsync,
                //DisplayFeedbackStepAsync
                //CommentStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }


        //private async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    //stepContext.Values["rate"] = ((FoundChoice)stepContext.Result).Value;

        //    var receiptCard = CardTypes.GetAdaptiveCardFeedback();
        //    var response = MessageFactory.Attachment(receiptCard, ssml: "Welcome to my Bot!");

        //    await stepContext.Context.SendActivityAsync(response, cancellationToken);
        //    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        //    //return null;
        //}

        private async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Please enter your rating."),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3" }),
               }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Ratings"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("You can also add some comment") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ThirdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Comments"] = (string)stepContext.Result;

            Feedback feedback = new Feedback();
            feedback.Rating = (string)stepContext.Values["Ratings"];
            feedback.Comment = (string)stepContext.Values["Comments"]; 
            
            bool status = userRepository.InsertFeedback(feedback);

            if (status)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Feedback Inserted"), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Feedback Not Inserted or Employee already exists"), cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to insert more Employee details?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FourthStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}
