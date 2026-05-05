using System;
using System.Diagnostics;
using System.IO;

namespace MediaDownloaderApp
{
    /// <summary>
    /// Handles downloading media using yt-dlp.
    /// </summary>
    public class MediaDownloader
    {
        /// <summary>
        /// Downloads media from the provided link using yt-dlp.
        /// </summary>
        /// <param name="mediaLink">The URL of the media to download.</param>
        /// <param name="timeRange">The time range to download (optional).</param>
        /// <param name="downloadFolder">The folder to save the downloaded media.</param>
        /// <param name="downloadQuality">The requested download quality profile.</param>
        /// <returns>The file path of the downloaded media.</returns>
        public string DownloadMedia(string mediaLink, string timeRange, string downloadFolder, string downloadQuality)
        {
            Console.WriteLine("Downloading media...");

            // Build yt-dlp arguments
            string qualitySelector = GetFormatSelector(downloadQuality);
            string formatArgument = string.IsNullOrWhiteSpace(qualitySelector)
                ? string.Empty
                : $"-f \"{qualitySelector}\" ";
            string arguments = $"{formatArgument}-o \"{downloadFolder}\\%(title)s.%(ext)s\" \"{mediaLink}\"";

            // Add time range if specified
            if (!string.IsNullOrWhiteSpace(timeRange))
            {
                arguments += $" --download-sections \"*{timeRange}\"";
            }

            // Start yt-dlp process
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = arguments,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            // Find the downloaded file
            string downloadedFile = GetLatestFile(downloadFolder);

            Console.WriteLine($"Media downloaded to: {downloadedFile}");
            return downloadedFile;
        }

        private static string GetFormatSelector(string downloadQuality)
        {
            if (string.IsNullOrWhiteSpace(downloadQuality))
            {
                return "bv*+ba/b";
            }

            return downloadQuality.Trim().ToLowerInvariant() switch
            {
                "best" => "bv*+ba/b",
                "worst" => "wv*+wa/w",
                "default" => string.Empty,
                _ => "bv*+ba/b"
            };
        }

        /// <summary>
        /// Gets the most recently modified file in a directory.
        /// </summary>
        /// <param name="directory">The directory to search.</param>
        /// <returns>The file path of the most recent file.</returns>
        private string GetLatestFile(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var file = directoryInfo.GetFiles()
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();
            return file?.FullName;
        }
    }
}
