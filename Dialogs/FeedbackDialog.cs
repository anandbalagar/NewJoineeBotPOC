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
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Kindly provide your feedback by rating your experience on a scale of 1 to 5"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2", "3","4","5" }),
               }, cancellationToken);
        }

        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Ratings"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please include any comments or suggestions to help us further improve.") }, cancellationToken);
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
                var heroCard = new HeroCard
                {
                    Text = "Thank you 😊. Your feedback has been submitted",

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

            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Feedback not submitted"), cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);

        }

    }
}
