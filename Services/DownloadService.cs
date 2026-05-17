using SocialMediaDownloader.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SocialMediaDownloader.Services
{
    public class DownloadService
    {
        private readonly string ytDlpPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "yt-dlp.exe");

        private readonly string ffmpegPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets");

        public async Task Download(
            string url,
            string destination,
            string fileName,
            DownloadOption option)
        {
            string arguments;
            /*string outputTemplate =
                Path.Combine(
                    destination,
                    "%(title)s_[%(height)sp].%(ext)s");*/

            string safeName = fileName;

            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "download";

            string outputTemplate =
                Path.Combine(destination, $"{safeName}_%(height)sp.%(ext)s");

            if (option.IsAudio)
            {
                arguments =
                    $"-f bestaudio " +
                    $"-x --audio-format mp3 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"-o \"{destination}\\%(title)s.%(ext)s\" " +
                    $"\"{url}\"";
            }
            else if (url.Contains("youtube.com") ||
                    url.Contains("youtu.be"))
            {
                // YOUTUBE
                arguments =
                    //$"-f \"bv*+ba/b\" " +
                    $"-f \"{option.FormatString}\" " +
                    $"--merge-output-format mp4 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"--js-runtimes node " +
                    $"--no-part " +
                    $"--force-overwrites " +
                    $"--windows-filenames " +
                    $"--postprocessor-args \"ffmpeg:-c:v copy -c:a aac\" " +
                    $"-o \"{outputTemplate}\" " +
                    $"\"{url}\"";
            }
            else if (url.Contains("twitter.com") ||
                     url.Contains("x.com"))
            {
                // TWITTER / X
                arguments =
                    $"-f \"b\" " +
                    $"--merge-output-format mp4 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"--force-overwrites " +
                    $"--windows-filenames " +
                    $"--postprocessor-args \"ffmpeg:-c:v copy -c:a aac\" " +
                    $"-o \"{outputTemplate}\" " +
                    $"\"{url}\"";
            }
            else
            {
                // DEFAULT (Instagram / TikTok / Others)
                arguments =
                    $"-f \"{option.FormatString}\" " +
                    //$"-f \"bestvideo+bestaudio/best\" " +
                    $"--merge-output-format mp4 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"--force-overwrites " +
                    $"--windows-filenames " +
                    $"-o \"{outputTemplate}\" " +
                    $"\"{url}\"";
            }

            ProcessStartInfo psi = new()
            {
                FileName = ytDlpPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process process = new();

            process.StartInfo = psi;

            process.Start();

            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            /*if (!string.IsNullOrWhiteSpace(stderr))
            {
                File.WriteAllText(
                    Path.Combine(destination, "yt-dlp-error.log"),
                    stderr);
            }*/
        }
    }
}