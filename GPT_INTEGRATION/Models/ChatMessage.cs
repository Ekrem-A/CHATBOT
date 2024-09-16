namespace GPT_INTEGRATION.Models
{

    public static class Utility
    {
        public static readonly List<ChatMessage> chatMessages = new List<ChatMessage>();
    }
    public class ChatMessage
    {
        public required string Content { get; set; }
        public bool IsUser { get; set; } // True if message from user, false if from assistant
        //public required string Document { get; set; }

    }

}
