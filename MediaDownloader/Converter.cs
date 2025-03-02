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

        /// <summary>
        /// Extracts audio from a video file and converts it to MP3 using ffmpeg.
        /// This method extracts the audio stream and encodes it to MP3 format with high quality.
        /// </summary>
        /// <param name="inputFile">The path to the source video file.</param>
        /// <param name="outputFile">The path to the output MP3 file.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task ExtractAudioToMp3Async(string inputFile, string outputFile)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
                throw new ArgumentException("Input file must be specified", nameof(inputFile));
            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentException("Output file must be specified", nameof(outputFile));

            Console.WriteLine($"Starting audio extraction: {inputFile} -> {outputFile}");

            // Build the ffmpeg arguments to extract audio to MP3.
            // The -q:a 0 option specifies the best quality for MP3 encoding.
            // The -map a option selects only the audio stream.
            string arguments = $"-i \"{inputFile}\" -q:a 0 -map a \"{outputFile}\"";
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
            Console.WriteLine("FFmpeg process started for audio extraction...");

            // Capture standard output and error for logging purposes.
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();
            Console.WriteLine("FFmpeg process exited for audio extraction.");

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"FFmpeg error output: {error}");
                throw new Exception($"FFmpeg exited with error code {process.ExitCode}");
            }

            Console.WriteLine("Audio extraction completed successfully.");
        }
    }
}
