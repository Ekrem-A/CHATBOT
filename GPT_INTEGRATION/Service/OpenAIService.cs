using Newtonsoft.Json;
using System.Text;

namespace GPT_INTEGRATION.Service
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private readonly string _apiKey;

        public OpenAIService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
        public async Task<string> SendMessageToChatGPTAsync(string message)
        {
            var data = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = "you cant answer the mathematical problems!"},
                    new { role = "user", content = message }
                },
                max_tokens = 1000
            };

            var response = await _httpClient.PostAsync(ApiUrl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public string ConvertImageToBase64(string imagePath)
        {
            byte[] imageBytes = File.ReadAllBytes(imagePath);
            return Convert.ToBase64String(imageBytes);
        }

        public async Task<string> SendImageToGPTAsync(string imagePath, string question)
        {
            // Resmi Base64 formatına çevir
            string base64Image = ConvertImageToBase64(imagePath);

            // İstek verisini hazırla
            var payload = new
            {
                model = "gpt-4o-mini",
                messages = new[]
             {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = question },
                        new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                    }
                }
            },
                max_tokens = 300
            };

            // JSON verisini stringe dönüştür
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);

            // API'ye POST isteği gönder
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}







