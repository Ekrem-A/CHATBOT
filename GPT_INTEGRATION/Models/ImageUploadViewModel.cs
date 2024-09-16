using System.Text.Json.Serialization;

namespace GPT_INTEGRATION.Models
{
    public class ImageUploadViewModel
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
        public int max_tokens { get; set; }
    }
    public class ImageUrl
    {
        public string url { get; set; }

    }

    public class Message
    {
        public string role { get; set; }
        public List<Content> content { get; set; }
    }

    [JsonDerivedType(typeof(ContentA))]
    [JsonDerivedType(typeof(ContentB))]

    public class Content
    { }

    public class ContentA : Content
    {
        public string type { get; set; }
        public string text { get; set; }

    }
    public class ContentB : Content
    {
        public string type { get; set; }
        public ImageUrl image_url { get; set; }
    }
}
