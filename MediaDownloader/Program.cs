using System;
using System.IO;
using System.Diagnostics;
using MediaDownloader;

namespace MediaDownloaderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Make sure api key is loaded
            OpenAIKeyLoader.LoadApiKey();

            // Prompt 1: Media Link
            Console.Write("1) Media Link (YouTube, Instagram, etc): ");
            string mediaLink = Console.ReadLine();

            // Prompt 2: Time Range
            Console.Write("2) Enter a time range using hh:mm:ss-hh:mm:ss or leave blank for the full video. Range: ");
            string timeRange = Console.ReadLine();

            // Prompt 3: Download Folder
            Console.Write("3) Choose a folder to download to. Leave blank for the user's downloads directory. Folder: ");
            string downloadFolder = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(downloadFolder))
            {
                downloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
            }

            // Prompt 4: Create Captions
            Console.Write("4) Do you want to create captions? Type [Y] for yes or anything else for no: ");
            string createCaptionsResponse = Console.ReadLine();
            bool createCaptions = createCaptionsResponse.Equals("Y", StringComparison.OrdinalIgnoreCase);

            bool summarizeVideo = false;
            string customPrompt = string.Empty;

            // Prompt 5: Summarize Video
            if (createCaptions)
            {
                Console.Write("5) Do you want to use ChatGPT to summarize the video? Type [Y] for yes or anything else for no: ");
                string summarizeResponse = Console.ReadLine();
                summarizeVideo = summarizeResponse.Equals("Y", StringComparison.OrdinalIgnoreCase);

                // Prompt 6: Custom Summarization Prompt
                if (summarizeVideo)
                {
                    Console.Write("6) Write the prompt used to summarize or leave blank to use a default prompt: ");
                    customPrompt = Console.ReadLine();
                }
            }

            // Initialize downloader and download media
            MediaDownloader downloader = new MediaDownloader();
            string downloadedFilePath = downloader.DownloadMedia(mediaLink, timeRange, downloadFolder);

            // Create captions if requested
            if (createCaptions)
            {
                Transcriber transcriber = new Transcriber();
                string transcriptionFilePath = transcriber.CreateTranscription(downloadedFilePath);

                // Summarize if requested
                if (summarizeVideo)
                {
                    Summarizer summarizer = new Summarizer();
                    summarizer.GenerateSummary(transcriptionFilePath, customPrompt);
                }
            }

            Console.WriteLine("Process completed. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
