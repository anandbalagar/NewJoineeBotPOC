using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using AdaptiveCards;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToDoBot.Dialogs.Operations;
using System;
using NewJoineeBOT.Utility;

namespace EchoBot1.Dialogs
{


    public class MainDialog : ComponentDialog
    {
        UserRepository userRepository;

        private readonly IStatePropertyAccessor<User> _userProfileAccessor;

        public MainDialog(UserState userState, UserRepository _userRepository)
            : base(nameof(MainDialog))
        {
            userRepository = _userRepository;
            _userProfileAccessor = userState.CreateProperty<User>("User");

            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {

                IntroStepAsync,
                ActStepAsync,
               // FinalStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new AccountSetupDialog(userState));
            AddDialog(new TrainingMaterialDialog());
            AddDialog(new FeedbackDialog(userRepository));
            AddDialog(new ItSupportDialog());
            AddDialog(new CompanyCultureDialog());

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            List<string> operationList = new List<string> { "Account Setup", "Training Material", "IT Support", "Company Culture", "Provide Feedback" };

            // Create adaptive card with welcome message, image, and choices
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {

                    new AdaptiveImage
                    {
                        Url = new Uri("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ73kV7rL_IvXTBF8vDOXggF5g2zF25Y_ADUw&usqp=CAU"),
                        Size = AdaptiveImageSize.Auto,
                        HorizontalAlignment= AdaptiveHorizontalAlignment.Center,

                    },

                    new AdaptiveTextBlock
                    {
                        Text = "Welcome to FNF India! 😊 ",
                        Size = AdaptiveTextSize.Large,
                        Weight = AdaptiveTextWeight.Bolder,
                        Wrap= true,
                    },

                    new AdaptiveTextBlock
                    {
                        Text = "Let’s make your onboarding process seamless and enjoyable.I’m here to assist you with the necessary onboarding steps.",
                        Size = AdaptiveTextSize.Normal,
                        Weight = AdaptiveTextWeight.Lighter,
                        Wrap= true,

                    },

                    new AdaptiveTextBlock
                    {
                        Text = "Whether it’s setting up your account, providing training materials, I’ve got you covered.",
                        Size = AdaptiveTextSize.Normal,
                        Weight = AdaptiveTextWeight.Lighter,
                        Wrap= true,

                    },

                    new AdaptiveTextBlock
                    {
                        Text = "What option would you like to choose?",
                        Size = AdaptiveTextSize.Medium,
                        Weight = AdaptiveTextWeight.Lighter,
                        Wrap= true,
                    }
                },
                // Use LINQ to turn the choices into submit actions
                Actions = operationList.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };

            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(operationList),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            }, cancellationToken);


        }


        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                stepContext.Values["Operation"] = ((FoundChoice)stepContext.Result).Value;
                string operation = (string)stepContext.Values["Operation"];
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You selected " + operation), cancellationToken);

                if ("Account Setup".Equals(operation))
                {
                    return await stepContext.BeginDialogAsync(nameof(AccountSetupDialog), new AccountSetup(), cancellationToken);
                }
                else if ("Training Material".Equals(operation))
                {
                    return await stepContext.BeginDialogAsync(nameof(TrainingMaterialDialog), new TrainingMaterial(), cancellationToken);
                }
                else if ("IT Support".Equals(operation))
                {
                    return await stepContext.BeginDialogAsync(nameof(ItSupportDialog), new ItSupport(), cancellationToken);
                }
                else if ("Company Culture".Equals(operation))
                {
                    return await stepContext.BeginDialogAsync(nameof(CompanyCultureDialog), new CompanyCulture(), cancellationToken);
                }
            else if ("Provide Feedback".Equals(operation))
                {
                    return await stepContext.BeginDialogAsync(nameof(FeedbackDialog), null, cancellationToken);
                }
               else
                {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("User Input not matched."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }

        }

        
    }
    }
    