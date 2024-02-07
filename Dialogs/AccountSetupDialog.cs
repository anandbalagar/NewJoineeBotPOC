using EchoBot1.Dialogs;
using Microsoft.Azure.Cosmos.Table;
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
    public class AccountSetupDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<AccountSetup> _userProfileAccessor;

        public AccountSetupDialog(UserState userState) : base(nameof(AccountSetupDialog))
        {
            _userProfileAccessor = userState.CreateProperty<AccountSetup>("AccountSetup");


            var waterfallSteps = new WaterfallStep[]
            {
                FNameStepAsync,
                LNameStepAsync,
                PhoneStepAsync,
                AddressStepAsync,
                GenderStepAsync,
                PasswordStepAsync,
                SummaryStepAsync,
                FeedbackStepAsync,
               // ExitStepAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<long>(nameof(NumberPrompt<long>),PhonePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt), PasswordPromptValidatorAsync));


            InitialDialogId = nameof(WaterfallDialog);


        }
        private static async Task<DialogTurnResult> FNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Please enter your first name") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> LNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["fname"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Enter your last name") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["lname"] = (string)stepContext.Result;

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Enter your phone number."),
                RetryPrompt = MessageFactory.Text("Please enter a valid input (For example : 7338366902)"),
            };

            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), promptOptions, cancellationToken);

        }

        private static async Task<DialogTurnResult> AddressStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["phone"] = (long)stepContext.Result;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Enter your address") }, cancellationToken);

        }

        private static async Task<DialogTurnResult> GenderStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["address"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
               new PromptOptions
               {
                   Prompt = MessageFactory.Text("Select your Gender"),
                   Choices = ChoiceFactory.ToChoices(new List<string> { "Male", "Female"}),
               }, cancellationToken);

        }

        private static async Task<DialogTurnResult> PasswordStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var foundChoice = (FoundChoice)stepContext.Result;
            stepContext.Values["gender"] = foundChoice.Value as string;

            // Assuming you have a custom password validator method called PasswordValidatorAsync
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Enter your password."),
                RetryPrompt = MessageFactory.Text("Please enter atleast 8 characters."),
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private  async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["password"] = (string)stepContext.Result;

            var userDetails = new AccountSetup
            {
                PartitionKey = "UserDetails",
                RowKey = Guid.NewGuid().ToString(),
                FName = (string)stepContext.Values["fname"],
                LName = (string)stepContext.Values["lname"],
                Phone = (long)stepContext.Values["phone"],
                Address = (string)stepContext.Values["address"],
                Gender = (string)stepContext.Values["gender"],
                Password = (string)stepContext.Values["password"],

            };



            // Retrieve your Azure Storage account connection string
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=storagedemofeedback;AccountKey=5Kt3xEhLQinX0/pzPm6fukovqZRmwNVxEeLiUnhAZsAYVyq8BpxeZ8k7lk+tHD3DM7J8dhUfpgj8+AStg4SC9w==;EndpointSuffix=core.windows.net";

            // Create a CloudTableClient object
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            // Get a reference to your Azure table
            var table = tableClient.GetTableReference("JoineeDetails");

            // Create the table if it doesn't exist
            await table.CreateIfNotExistsAsync();

          

            // Create an operation to insert the user profile into the table
            var insertOperation = Microsoft.Azure.Cosmos.Table.TableOperation.InsertOrReplace(userDetails);

            // Execute the insert operation
            await table.ExecuteAsync(insertOperation);

            var userInfo = await _userProfileAccessor.GetAsync(stepContext.Context, () => new AccountSetup(), cancellationToken);

            userInfo.FName = (string)stepContext.Values["fname"];
            userInfo.LName = (string)stepContext.Values["lname"];
            userInfo.Phone = (long)stepContext.Values["phone"];
            userInfo.Address = (string)stepContext.Values["address"];
            userInfo.Gender = (string)stepContext.Values["gender"];
            userInfo.Password = (string)stepContext.Values["password"];



            //Receipt card method from WelcomeDialogBot
            var cardInstance = new CardTypes();

            var receiptCard = cardInstance.CreateReceiptCard(userInfo);
            var response = MessageFactory.Attachment(receiptCard, ssml: "Welcome to my Bot!");


            await stepContext.Context.SendActivityAsync(response, cancellationToken);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions 
            {
                Prompt = MessageFactory.Text("Was the above information helpful?") 
            },
            cancellationToken);

            // return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
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

            private static Task<bool> PhonePromptValidatorAsync(PromptValidatorContext<long> promptContext, CancellationToken cancellationToken)
        {
            string phoneNumberString = promptContext.Recognized.Value.ToString();
            return Task.FromResult(promptContext.Recognized.Succeeded && phoneNumberString.Length == 10);
        }

        private static Task<bool> PasswordPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            // Implement your custom password validation logic here
            // For example, you can check the length, complexity, or other requirements

            // For demonstration purposes, let's assume the password should be at least 8 characters long
            return Task.FromResult(promptContext.Recognized.Succeeded && promptContext.Recognized.Value.Length >= 8);
        }


    }
}