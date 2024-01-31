using AdaptiveCards;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.BotBuilderSamples
{
    public class CardTypes : ActivityHandler
    {

        public Attachment CreateReceiptCard(AccountSetup userInfo)
        {

            var receiptCard = new ReceiptCard
            {
                Title = "Your account has been created. Here are your details",

                Items = new List<ReceiptItem>
                {
                    new ReceiptItem(" "),
                    new ReceiptItem(" "),

                     new ReceiptItem(
                         $"Name : {userInfo.FName} {userInfo.LName}",
                         image: new CardImage(url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTJxksz_0WVfcdYFqUAdkX1Gvgqb8y7r4rHRg&usqp=CAU")
                         ),
                    new ReceiptItem(
                        $"Phone : {userInfo.Phone}",
                         image: new CardImage(url: "data:image/jpeg;base64,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")
                        ),
                    new ReceiptItem(
                         $"Address : {userInfo.Address}",
                         image: new CardImage(url: "https://th.bing.com/th/id/OIP.qJjSqS0EQJw2hkM7tbLglQHaGX?w=234&h=201&c=7&r=0&o=5&pid=1.7")
                         ),
                      new ReceiptItem(
                         $"Gender : {userInfo.Gender}",
                         image: new CardImage(url: "data:image/jpeg;base64,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")
                         ),
                       new ReceiptItem(
                         $"Email : {userInfo.FName}.{userInfo.LName}@fnf.com",
                         image: new CardImage(url: "https://th.bing.com/th/id/OIP.uk4lQ_Ow6e14xXpctp41QgHaHa?w=190&h=190&c=7&r=0&o=5&pid=1.7")
                         ),
                        new ReceiptItem(
                         $"Password : {userInfo.Password}",
                         image: new CardImage(url: "https://th.bing.com/th/id/OIP.-8x8vO7I1bTYV-wEcI_3qQHaHa?w=181&h=181&c=7&r=0&o=5&pid=1.7")
                         ),
                        

                        new ReceiptItem(

                         $"All the Best for you new journey!"
                         )


                    }
            };


            return new Attachment()
            {
                ContentType = ReceiptCard.ContentType,
                Content = receiptCard

            };
        }

        public static HeroCard GetHeroCardTraining()
        {
            var heroCard = new HeroCard
            {
                Title = "Welcome to FNFI's Online Training Platform!",
                Subtitle = "Empowering Your Skills",
                Text = "Explore our diverse courses covering security fundamentals, technical concepts, ethical training, and organizational cooperation. Gain essential cybersecurity skills, stay updated on the latest technologies, and learn ethical decision-making. Our programs focus on collaborative behaviors and effective communication within teams. Benefit from expert instructors, flexible learning, and receive recognized certifications upon completion. Elevate your expertise with FNFI - where empowerment meets education!",
                Images = new List<CardImage> { new CardImage("https://th.bing.com/th/id/OIP.EVbqPESF1xt33ND64WLlEwHaEf?w=271&h=180&c=7&r=0&o=5&pid=1.7") },
                Buttons = new List<CardAction>
        {
            new CardAction(ActionTypes.OpenUrl, "Annual SDL Training", value: "https://fnf.vega.securitycompass.com/"),
            new CardAction(ActionTypes.OpenUrl, "Mandatory Corporate Compliance Training link", value: "https://saiglobal-fnf.csod.com"),
            new CardAction(ActionTypes.ImBack, "Back to Main Menu", value: "Main Menu"),
        }
            };

            return heroCard;
        }

        public static Attachment CreateAdaptiveCardAttachment(string cardFileName)
        {

            var cardPath = Path.Combine("cards", cardFileName);
            var cardJson = File.ReadAllText(cardPath);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson, new JsonSerializerSettings { MaxDepth = null }),
            };

        }

      


        public static Attachment GetAdaptiveCardFeedback()
        {
            var adaptiveCard = new AdaptiveCard(new AdaptiveSchemaVersion(1, 3))
            {
                Body = new List<AdaptiveElement>
        {
            new AdaptiveTextBlock
            {
                Text = "Welcome to feedback!",
                Size = AdaptiveTextSize.Large,
                Weight = AdaptiveTextWeight.Bolder
            },
            new AdaptiveTextBlock
            {
                Text = "Provide feedback"
            },
            new AdaptiveChoiceSetInput
            {
                Id = "rating",
                Style = AdaptiveChoiceInputStyle.Expanded,
                Choices = new List<AdaptiveChoice>
                {
                    new AdaptiveChoice { Title = "⭐", Value = "1" },
                    new AdaptiveChoice { Title = "⭐⭐", Value = "2" },
                    new AdaptiveChoice { Title = "⭐⭐⭐", Value = "3" },
                    new AdaptiveChoice { Title = "⭐⭐⭐⭐", Value = "4" },
                    new AdaptiveChoice { Title = "⭐⭐⭐⭐⭐", Value = "5" },
                },
                Placeholder = "Select a rating",
            },
            new AdaptiveTextInput
            {
                Id = "comments",
                Placeholder = "Type your comments here..."
            }
        },
                Actions = new List<AdaptiveAction>
        {
            new AdaptiveSubmitAction
            {
                Title = "Submit",
                Data = new
                {
                    Type = "submit"
                }
            },
            new AdaptiveSubmitAction
            {
                Title = "Exit",
                Data = new
                {
                    Type = "exit"
                }
            }
        }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCard.ToJson())
            };
        }



    }
}