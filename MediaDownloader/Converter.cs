using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MediaDownloaderApp
{
    public class Converter
    {
        /// <summary>
        /// Converts a video file from one format to another using ffmpeg.
        /// The video and audio streams are copied to preserve the original quality.
        /// </summary>
        /// <param name="inputFile">The path to the source video file.</param>
        /// <param name="outputFile">The path to the output video file.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Thrown if input or output file is null or whitespace.</exception>
        /// <exception cref="Exception">Thrown if ffmpeg exits with a non-zero exit code.</exception>
        public async Task ConvertVideoAsync(string inputFile, string outputFile)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentException("Input file must be specified", nameof(inputFile));
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentException("Output file must be specified", nameof(outputFile));

            Console.WriteLine($"Starting video conversion: {inputFile} -> {outputFile}");

            // Build the ffmpeg arguments. This command copies the video and audio streams.
            string arguments = $"-i \"{inputFile}\" -c:v copy -c:a copy \"{outputFile}\"";
            Console.WriteLine($"Running command: ffmpeg {arguments}");

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using Process process = new Process { StartInfo = startInfo };

            process.Start();
            Console.WriteLine("FFmpeg process started...");

            // Capture standard output and error (optional, useful for logging or debugging).
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            Console.WriteLine("FFmpeg process exited.");

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"FFmpeg error output: {error}");
                throw new Exception($"FFmpeg exited with error code {process.ExitCode}");
            }

            Console.WriteLine("Video conversion completed successfully.");
        }
    }
}
