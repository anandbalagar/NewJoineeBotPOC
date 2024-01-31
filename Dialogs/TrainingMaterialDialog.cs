using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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
                //ExitStepAsync,
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
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}