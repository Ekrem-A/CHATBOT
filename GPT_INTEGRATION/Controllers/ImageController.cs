using GPT_INTEGRATION.Models;
using GPT_INTEGRATION.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;
using iText.Kernel.Pdf;
using System.Text;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;


namespace GPT_INTEGRATION.Controllers
{
    public class ImageController(OpenAIService openAIService) : Controller
    {

        private readonly OpenAIService _openAIService = openAIService;

        public string documentContent = ExtractTextFromPdf("Anayasa.pdf");

        public static string ExtractTextFromPdf(string pathf)
        {
            using (var pdfReader = new PdfReader(pathf))
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
            string apiResponse="";
            bool isFile = false;
            if (image != null && image.Length > 0)
            {

                var fileExtension = Path.GetExtension(image.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    isFile = true;
                    // Dosya resim değil, uygun bir hata mesajı döndürebilirsiniz

                }
                else
                {
                    var tempDirectory = Path.GetTempPath();
                    var filePath = Path.Combine(tempDirectory, image.FileName);

                    // Resmi geçici bir klasöre kaydet
                    Utility.chatMessages.Add(new ChatMessage { Content = question, IsUser = true /*Document = documentContent*/ });

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Resim ile birlikte OpenAI API'ye soruyu gönder
                    apiResponse = await _openAIService.SendImageToGPTAsync(filePath, question);
                }


            }
            else if (!string.IsNullOrEmpty(question) && isFile == false )
            {
               

                //string pdfFilePath = "Anayasa.pdf";
                //var uploadedFile = await _openAIService.UploadFileToAssistantAsync(pdfFilePath);
                Utility.chatMessages.Add(new ChatMessage { Content = question, IsUser = true });
                // Asistan oluştur ve soruyu yönlendir
                //string assistantCreationResponse = await _openAIService.CreateAssistantWithFileAsync(uploadedFile);

                //apiResponse = assistantCreationResponse;


                // Resim olmadan sadece metin bazlı soruyu gönder
                apiResponse = await _openAIService.SendMessageToChatGPTAsync(question);
            }
            //else 
            //{
            //    // PDF'yi dosya olarak yükle ve asistan oluştur
            //    //string pdfFilePath = "Anayasa.pdf";
            //    //var uploadedFile = await _openAIService.UploadFileToAssistantAsync(pdfFilePath);

            //    //// Asistan oluştur ve soruyu yönlendir
            //    //string assistantCreationResponse = await _openAIService.CreateAssistantWithFileAsync(uploadedFile);

            //    //apiResponse = assistantCreationResponse;
            //}

            // Yanıtı JSON formatında parse et
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(apiResponse);

            // content kısmını al
            var response = jsonResponse.GetProperty("choices")[0]
                                       .GetProperty("message")
                                       .GetProperty("content").GetString();

            // Sadece content kısmını View'e gönder
            //ViewBag.Response = response;
            Utility.chatMessages.Add(new ChatMessage { Content = response, IsUser = false/*, Document = documentContent*/ });

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
