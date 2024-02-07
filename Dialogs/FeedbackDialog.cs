using Microsoft.Azure.Cosmos.Table;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using NewJoineeBOT.Utility;
using Newtonsoft.Json;
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
        private object AdaptiveCardPrompt;


        public FeedbackDialog() : base(nameof(FeedbackDialog))
        {

            var waterfallSteps = new WaterfallStep[]
            {
                StartStepAsync,
                SecondStepAsync,
               
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }




        private async Task<DialogTurnResult> StartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("We are sorry to hear that the provided information was not helpful. Can you please add some suggestions so that we can improve upon ourselves") }, cancellationToken);

        }


        private async Task<DialogTurnResult> SecondStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["comment"] = (string)stepContext.Result;

            var userFeedback = new Feedback
            {
                PartitionKey = "UserDetails",
                RowKey = Guid.NewGuid().ToString(),
                Comment = (string)stepContext.Values["comment"],
            };



            // Retrieve your Azure Storage account connection string
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=storagedemofeedback;AccountKey=5Kt3xEhLQinX0/pzPm6fukovqZRmwNVxEeLiUnhAZsAYVyq8BpxeZ8k7lk+tHD3DM7J8dhUfpgj8+AStg4SC9w==;EndpointSuffix=core.windows.net";

            // Create a CloudTableClient object
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Get a reference to your Azure table
            var table = tableClient.GetTableReference("Feedback");

            // Create the table if it doesn't exist
            await table.CreateIfNotExistsAsync();

            // Create an operation to insert the user profile into the table
            var insertOperation = Microsoft.Azure.Cosmos.Table.TableOperation.InsertOrReplace(userFeedback);

            // Execute the insert operation
            await table.ExecuteAsync(insertOperation);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Thank you! Feedback submitted successfully"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);


        }

    }
}
