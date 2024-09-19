using MediaDownloader;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MediaDownloaderApp
{
    /// <summary>
    /// Handles transcription of media using ffmpeg and OpenAI Whisper.
    /// </summary>
    public class Transcriber
    {
        /// <summary>
        /// Converts video to audio and transcribes using Whisper.
        /// </summary>
        /// <param name="videoFilePath">The path to the video file.</param>
        /// <returns>The file path of the transcription text file.</returns>
        public string CreateTranscription(string videoFilePath)
        {
            Console.WriteLine("Converting video to audio...");

            string audioFilePath = ConvertVideoToAudio(videoFilePath);
            Console.WriteLine("Transcribing audio...");

            string transcription = TranscribeAudio(audioFilePath).Result;

            string transcriptionFilePath = Path.ChangeExtension(videoFilePath, ".txt");
            File.WriteAllText(transcriptionFilePath, transcription);

            Console.WriteLine($"Transcription saved to: {transcriptionFilePath}");
            return transcriptionFilePath;
        }

        /// <summary>
        /// Converts a video file to an MP3 audio file using ffmpeg.
        /// </summary>
        /// <param name="videoFilePath">The path to the video file.</param>
        /// <returns>The file path of the audio file.</returns>
        private string ConvertVideoToAudio(string videoFilePath)
        {
            string audioFilePath = Path.ChangeExtension(videoFilePath, ".mp3");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{videoFilePath}\" -q:a 5 -b:a 128k -map a \"{audioFilePath}\" -y",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            return audioFilePath;
        }

        /// <summary>
        /// Transcribes an audio file using OpenAI Whisper API.
        /// </summary>
        /// <param name="audioFilePath">The path to the audio file.</param>
        /// <returns>The transcription text.</returns>
        private async Task<string> TranscribeAudio(string audioFilePath)
        {
            string apiKey = OpenAIKeyLoader.LoadApiKey();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                using var content = new MultipartFormDataContent();
                content.Add(new StringContent("whisper-1"), "model");
                var audioContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));
                audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
                content.Add(audioContent, "file", Path.GetFileName(audioFilePath));

                var response = await httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                // Assuming the API returns plain text
                return result;
            }
        }
    }
}
