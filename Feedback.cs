using Microsoft.Azure.Cosmos.Table;
using Microsoft.Bot.Schema;
namespace Microsoft.BotBuilderSamples
{
    // Stores User Welcome state for the conversation.
    // Stored in "Microsoft.Bot.Builder.ConversationState" and
    // backed by "Microsoft.Bot.Builder.MemoryStorage".

    public class Feedback : TableEntity
    {
        // Gets or sets whether the user has been welcomed in the conversation.
        public string Comment { get; set; }
        //public string rating { get; set; }

    }
}