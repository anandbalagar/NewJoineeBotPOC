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

            // Create a Hero Card with a button for returning to the Main Menu
            var heroCard = new HeroCard
           {
                    Buttons = new List<CardAction>
            {
                new CardAction
                {
                    Type = ActionTypes.ImBack,
                    Title = "Main Menu",
                    Value = "Main Menu"
                }
            }
            };

            // Attach the Hero Card to the response
            response.Attachments.Add(heroCard.ToAttachment());

            await stepContext.Context.SendActivityAsync(response, cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        //private async Task<DialogTurnResult> ExitStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    if ((bool)stepContext.Result)
        //    {
        //        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        //    }
        //    else
            
        //    {
        //        return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), cancellationToken: cancellationToken);

        //    }

        //}
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