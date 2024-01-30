using Microsoft.Bot.Schema;
namespace Microsoft.BotBuilderSamples
{
    // Stores User Welcome state for the conversation.
    // Stored in "Microsoft.Bot.Builder.ConversationState" and
    // backed by "Microsoft.Bot.Builder.MemoryStorage".

    public class AccountSetup
    {
        // Gets or sets whether the user has been welcomed in the conversation.
        public string FName { get; set; }
        public string LName { get; set; }
        public long Phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }

    }
}