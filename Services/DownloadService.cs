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
        //String paths for yt-dlp and ffmpeg, which are included in the Assets folder of the project
        private readonly string ytDlpPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "yt-dlp.exe");

        private readonly string ffmpegPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets");

        // The main method for downloading media, which takes in the URL, destination folder, file name, and download options
        public async Task Download(
            string url,
            string destination,
            string fileName,
            DownloadOption option)
        {
            string arguments;

            //String template for yt-dlp argument
            string safeName = fileName;

            if (string.IsNullOrWhiteSpace(safeName))
                safeName = "download";

            /*string outputTemplate =
                Path.Combine(destination, $"{safeName}_%(height)sp.%(ext)s");*/

            string outputTemplate =
                Path.Combine(destination, $"{safeName}.%(ext)s");

            //Full arguments for yt-dlp, varies based on type of media & platform
            //If want to download files as their best quality video & audio, uncomment the -f options for each playform (remember to comment out the option.FormatString)
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
                // TWITTER
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
                // Instagram / TikTok / Others
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

            // Set up the process start info for yt-dlp, including the arguments and redirection of output
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

            // Read the standard output and error streams asynchronously, and wait for the process to exit
            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            // If there is any error output, write it to a log file in the destination folder, UNCOMMENT THIS IF YOU WANT TO DEBUG ERRORS
            /*if (!string.IsNullOrWhiteSpace(stderr))
            {
                File.WriteAllText(
                    Path.Combine(destination, "yt-dlp-error.log"),
                    stderr);
            }*/
        }
    }
}