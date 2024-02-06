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
                //SecondStepAsync,
                ThirdStepAsync,
               // FourthStepAsync,
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
            var feedbackCard = CardTypes.CreateAdaptiveCardAttachment("Feedback.json");
            var message = MessageFactory.Attachment(feedbackCard, ssml: "Welcome to my Bot!");
            await stepContext.Context.SendActivityAsync(message, cancellationToken);

            //// You can create a Hero Card with a text input and a button
            //var heroCard = new HeroCard
            //{
            //    Title = "Feedback",
            //    Text = "We are sorry that the provided information wasn't helpful. We value your input. Please add some comments",
            //    Buttons = new List<CardAction>
            //    {
            //        new CardAction
            //        {
            //            Type = ActionTypes.ImBack,
            //            Title = "Submit Feedback",
            //            Value = "Submit Feedback",
            //        },
            //    },

            //    // Input.Text for user to enter their comment
            //    Tap = new CardAction
            //    {
            //        Type = ActionTypes.ImBack,
            //        Text = "Enter your comment...",
            //    },
            //};

            //var response = MessageFactory.Attachment(heroCard.ToAttachment());
            //await stepContext.Context.SendActivityAsync(response, cancellationToken);
            return Dialog.EndOfTurn;
        }

        //private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    //stepContext.Values["Ratings"] = ((FoundChoice)stepContext.Result).Value;

        //    //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("You can also add comments or suggestions so that we can improve upon ourselves") }, cancellationToken);
        //}

        private async Task<DialogTurnResult> ThirdStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userComment = ((JObject)stepContext.Result)["userComment"].ToString();

            Feedback feedback = new Feedback();
            feedback.Comment = userComment;

            bool status = userRepository.InsertFeedback(feedback);

            if (status)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank You! Feedback submitted successfully"), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Feedback not submitted"), cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);

        }

        //private async Task<DialogTurnResult> FourthStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if ((bool)stepContext.Result)
        //    {
        //        return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
        //    }
        //    else
        //    {
        //        return await stepContext.EndDialogAsync(null, cancellationToken);
        //    }
        //}
    }
}
