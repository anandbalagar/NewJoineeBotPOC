using Microsoft.Bot.Schema;
namespace Microsoft.BotBuilderSamples
{
    // Stores User Welcome state for the conversation.
    // Stored in "Microsoft.Bot.Builder.ConversationState" and
    // backed by "Microsoft.Bot.Builder.MemoryStorage".

    public class User
    {
        // Gets or sets whether the user has been welcomed in the conversation.
        //public string Name { get; set; }
        public string Name { get; set; } = "Anand";

    }
}