using GPT_INTEGRATION.Models;
using GPT_INTEGRATION.Services;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;


namespace GPT_INTEGRATION.Controllers
{
    public class ImageController(OpenAIService openAIService) : Controller
    {

        private readonly OpenAIService _openAIService = openAIService;
        public string documentContent = ExtractTextFromPdf("Anayasa.pdf");


        public static string ExtractTextFromPdf(string path)
        {
            using (var pdfReader = new PdfReader(path))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                var text = new StringBuilder();
                var strategy = new SimpleTextExtractionStrategy();

                for (int page = 1; page <= pdfDocument.GetNumberOfPages(); ++page)
                {
                    var pdfPage = pdfDocument.GetPage(page);
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfPage, strategy);
                    text.Append(pageText);
                }

                return text.ToString();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image, string question)
        {
            string apiResponse;

            if (image != null && image.Length > 0)
            {
                // Resmi geçici bir klasöre kaydet
                var tempDirectory = Path.GetTempPath();
                var filePath = Path.Combine(tempDirectory, image.FileName);

                string prompt = $"{documentContent}\n\n{question}";

                Utility.chatMessages.Add(new ChatMessage { Content = question, IsUser = true, Document = documentContent });

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Resim ile birlikte OpenAI API'ye soruyu gönder
                apiResponse = await _openAIService.SendImageToGPTAsync(filePath, prompt);
            }
            else
            {

                string prompt = $"{documentContent}\n\n{question}";

                Utility.chatMessages.Add(new ChatMessage { Content = question, IsUser = true, Document = documentContent });

                // Resim olmadan sadece metin bazlı soruyu gönder
                apiResponse = await _openAIService.SendMessageToChatGPTAsync(prompt);
            }

            // Yanıtı JSON formatında parse et
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(apiResponse);

            // content kısmını al
            var response = jsonResponse.GetProperty("choices")[0]
                                       .GetProperty("message")
                                       .GetProperty("content").GetString();

            // Sadece content kısmını View'e gönder
            //ViewBag.Response = response;
            Utility.chatMessages.Add(new ChatMessage { Content = response, IsUser = false, Document = documentContent });

            return View("Index", Utility.chatMessages);
        }
        public IActionResult Index()
        {
            return View();
        }


        //private void SaveMessageToSession(ChatMessage message)
        //{
        //    var messages = HttpContext.Session.Get<List<ChatMessage>>("ChatMessages") ?? new List<ChatMessage>();
        //    messages.Add(message);
        //    HttpContext.Session.Set("ChatMessages", messages);
        //}

        //private List<ChatMessage> GetMessagesFromSession()
        //{
        //    return HttpContext.Session.Get<List<ChatMessage>>("ChatMessages") ?? new List<ChatMessage>();
        //}

    }
}
