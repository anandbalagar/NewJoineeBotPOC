using EchoBot1.Dialogs;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoBot.Dialogs.Operations
{
    public class TrainingMaterialDialog : ComponentDialog
    {
        public TrainingMaterialDialog() : base(nameof(TrainingMaterialDialog))
        {
            var waterfallSteps = new WaterfallStep[]
            {
                StartStepAsync,
                FeedbackStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {


            var receiptCard = CardTypes.GetHeroCardTraining().ToAttachment();
            var response = MessageFactory.Attachment(receiptCard, ssml: "Welcome to my Bot!");

            await stepContext.Context.SendActivityAsync(response, cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Was the above information helpful?")
            },
             cancellationToken);
        }



        private static async Task<DialogTurnResult> FeedbackStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {

                var heroCard = new HeroCard
                {
                    Text = "Thank you for using our Bot 😊",

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

                return await stepContext.NextAsync();
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), cancellationToken: cancellationToken);
            }

        }
    }
 }