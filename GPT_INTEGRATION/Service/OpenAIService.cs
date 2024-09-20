using Azure.AI.OpenAI.Assistants;
using Azure;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Diagnostics;


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


        //public async Task<OpenAIFile> UploadFileToAssistantAsync(string filePath)
        //{
        //    // Dosyayı yüklemek için dosya içeriklerini oku
        //    byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        //    var content = new ByteArrayContent(fileBytes);
        //    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

        //    // Dosyayı API'ye yükle
        //    var uploadResponse = await _httpClient.PostAsync("https://api.openai.com/v1/files", content);
        //    uploadResponse.EnsureSuccessStatusCode();

        //    var result = await uploadResponse.Content.ReadAsStringAsync();
        //    var openAIFile = JsonConvert.DeserializeObject<Response<OpenAIFile>>(result).Value;

        //    return openAIFile;
        //}

        //public async Task<string> CreateAssistantWithFileAsync(OpenAIFile uploadedFile, string message)
        //{

        //    var client = new AssistantsClient("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");
        //    // Asistan oluşturma isteği
        //    var assistantPayload = new AssistantCreationOptions("gpt4")
        //    {
        //        Name = "SDK Test Assistant - Retrieval",
        //        Instructions = "You are a helpful assistant that can help fetch data from files you know about.",
        //        Tools = { new CodeInterpreterToolDefinition() }

        //    };
        //    string pdfFilePath = "Anayasa.pdf";

        //    var fileUploadResponse = await client.UploadFileAsync(pdfFilePath, OpenAIFilePurpose.Assistants);


        //    assistantPayload.FileIds.Add(fileUploadResponse.Value.Id);

        //    assistantPayload.Instructions += $" The file with id {fileUploadResponse.Value.Id} " + $"has a original file name of{Path.GetFileName(args[0])} and is" + $" a {Path.GetExtension(args[0]).Replace(".", string.Empty)}file. ";

        //    var assistant = await client.CreateAssistantAsync(assistantPayload);
        //    var thread = await client.CreateThreadAsync();

        //    while (!string.IsNullOrWhiteSpace(message))

        //    {
        //        string? lastMessageId = null;

        //        await client.CreateMessageAsync(thread.Value.Id, MessageRole.User, message);
        //        var run = await client.CreateRunAsync(thread.Value.Id, new CreateRunOptions(assistant.Value.Id));


        //        var messageResponse = await client.GetMessageAsync(thread.Value.Id,
        //            order: ListSortOrder.Ascending,
        //            after: lastMessageId
        //            );
        //    }
        //    var jsonPayload = JsonConvert.SerializeObject(assistantPayload);
        //    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        //    // API'ye isteği gönder
        //    var response = await _httpClient.PostAsync("https://api.openai.com/v1/assistants", content);
        //    response.EnsureSuccessStatusCode();

        //    var result = await response.Content.ReadAsStringAsync();
        //    return result; // Dilerseniz burada assistant ID'yi döndürebilirsiniz.
        //}
    }
}







