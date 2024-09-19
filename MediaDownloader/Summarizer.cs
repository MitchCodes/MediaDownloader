using MediaDownloader;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MediaDownloaderApp
{
    /// <summary>
    /// Handles summarization of text using ChatGPT.
    /// </summary>
    public class Summarizer
    {
        /// <summary>
        /// Generates a summary of the transcription using ChatGPT.
        /// </summary>
        /// <param name="transcriptionFilePath">The path to the transcription text file.</param>
        /// <param name="customPrompt">A custom prompt for summarization (optional).</param>
        public void GenerateSummary(string transcriptionFilePath, string customPrompt)
        {
            Console.WriteLine("Generating summary...");

            string transcriptionText = File.ReadAllText(transcriptionFilePath);
            string prompt = string.IsNullOrWhiteSpace(customPrompt)
                ? "Please provide a concise summary of the following text:\n\n" + transcriptionText
                : customPrompt + "\n\n" + transcriptionText;

            string summary = GetChatGPTResponse(prompt).Result;

            string summaryFilePath = Path.Combine(
                Path.GetDirectoryName(transcriptionFilePath),
                Path.GetFileNameWithoutExtension(transcriptionFilePath) + "_summary.txt"
            );

            File.WriteAllText(summaryFilePath, summary);
            Console.WriteLine($"Summary saved to: {summaryFilePath}");
        }

        /// <summary>
        /// Sends a prompt to the ChatGPT API and returns the response.
        /// </summary>
        /// <param name="prompt">The prompt to send.</param>
        /// <returns>The response text from ChatGPT.</returns>
        private async Task<string> GetChatGPTResponse(string prompt)
        {
            string apiKey = OpenAIKeyLoader.LoadApiKey();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[] {
                        new { role = "user", content = prompt }
                    }
                };

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                // Parse the result to get the assistant's reply
                var jsonDoc = System.Text.Json.JsonDocument.Parse(result);
                var reply = jsonDoc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return reply;
            }
        }
    }
}
