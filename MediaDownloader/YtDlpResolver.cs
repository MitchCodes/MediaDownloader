using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MediaDownloaderApp
{
    public sealed class YtDlpResolution
    {
        public YtDlpResolution(string executablePath, bool isFromApplicationDirectory)
        {
            ExecutablePath = executablePath;
            IsFromApplicationDirectory = isFromApplicationDirectory;
        }

        public string ExecutablePath { get; }

        public bool IsFromApplicationDirectory { get; }
    }

    public static class YtDlpResolver
    {
        public static YtDlpResolution ResolveAndUpdate()
        {
            YtDlpResolution resolution = Resolve();
            string sourceDescription = resolution.IsFromApplicationDirectory
                ? "application directory"
                : "PATH";

            Console.WriteLine($"Using yt-dlp from {sourceDescription}: {resolution.ExecutablePath}");
            CheckForUpdates(resolution.ExecutablePath);

            return resolution;
        }

        private static YtDlpResolution Resolve()
        {
            string executableName = GetExecutableName();
            string localCandidate = Path.Combine(AppContext.BaseDirectory, executableName);

            if (File.Exists(localCandidate))
            {
                return new YtDlpResolution(localCandidate, true);
            }

            string? pathCandidate = FindOnPath(executableName);
            if (!string.IsNullOrWhiteSpace(pathCandidate))
            {
                return new YtDlpResolution(pathCandidate, false);
            }

            throw new FileNotFoundException(
                $"Could not find {executableName} next to the application or on PATH.");
        }

        private static void CheckForUpdates(string executablePath)
        {
            Console.WriteLine("Checking yt-dlp for updates...");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                Arguments = "-U",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            try
            {
                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    Console.WriteLine("Unable to start yt-dlp update check.");
                    return;
                }

                string standardOutput = process.StandardOutput.ReadToEnd();
                string standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(standardOutput))
                {
                    Console.WriteLine(standardOutput.Trim());
                }

                if (!string.IsNullOrWhiteSpace(standardError))
                {
                    Console.WriteLine(standardError.Trim());
                }

                if (string.IsNullOrWhiteSpace(standardOutput) && string.IsNullOrWhiteSpace(standardError))
                {
                    Console.WriteLine("yt-dlp update check completed with no output.");
                }

                Console.WriteLine($"yt-dlp update check finished with exit code {process.ExitCode}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"yt-dlp update check failed: {ex.Message}");
            }
        }

        private static string GetExecutableName()
        {
            return OperatingSystem.IsWindows() ? "yt-dlp.exe" : "yt-dlp";
        }

        private static string? FindOnPath(string executableName)
        {
            string? pathValue = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(pathValue))
            {
                return null;
            }

            foreach (string candidateDirectory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                foreach (string candidatePath in GetPathCandidates(candidateDirectory, executableName))
                {
                    if (File.Exists(candidatePath))
                    {
                        return Path.GetFullPath(candidatePath);
                    }
                }
            }

            return null;
        }

        private static IEnumerable<string> GetPathCandidates(string directory, string executableName)
        {
            yield return Path.Combine(directory, executableName);

            if (!OperatingSystem.IsWindows())
            {
                yield break;
            }

            if (Path.HasExtension(executableName))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(executableName);
                yield return Path.Combine(directory, fileNameWithoutExtension);
                yield break;
            }

            string? pathExt = Environment.GetEnvironmentVariable("PATHEXT");
            if (string.IsNullOrWhiteSpace(pathExt))
            {
                yield break;
            }

            foreach (string extension in pathExt.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return Path.Combine(directory, executableName + extension);
            }
        }
    }
}