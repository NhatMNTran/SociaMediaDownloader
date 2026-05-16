using SocialMediaDownloader.Models;
using System;
using System.Diagnostics;
using System.IO;
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
            DownloadOption option)
        {
            string arguments;


            if (option.IsAudio)
            {
                arguments =
                    $"-f bestaudio " +
                    $"-x --audio-format mp3 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"-o \"{destination}\\%(title)s.%(ext)s\" " +
                    $"\"{url}\"";
            }
            else
            {
                /*arguments =
                    $"-f \"bv*+ba/b\" " +
                    $"--merge-output-format mp4 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"--no-part " +
                    $"--force-overwrites " +
                    $"--windows-filenames " +
                    $"--postprocessor-args \"ffmpeg:-c:v copy -c:a aac\" " +
                    $"-o \"{destination}\\%(title)s_[%(height)sp].%(ext)s\" " +
                    $"\"{url}\"";*/
                string outputTemplate =
                    Path.Combine(destination, "%(title)s_[%(height)sp].%(ext)s");
                arguments =
                    $"-f \"{option.FormatString}\" " +
                    $"--merge-output-format mp4 " +
                    $"--ffmpeg-location \"{ffmpegPath}\" " +
                    $"--force-overwrites " +
                    $"--windows-filenames " +
                    $"--postprocessor-args \"ffmpeg:-c:v copy -c:a aac\" " +
                    $"-o \"{outputTemplate}\" " +
                    $"\"{url}\"";
            }

            Debug.WriteLine("YT-DLP: " + ytDlpPath);
            Debug.WriteLine("FFMPEG: " + ffmpegPath);
            Debug.WriteLine("ARGS: " + arguments);

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

            if (!string.IsNullOrWhiteSpace(stderr))
            {
                File.WriteAllText(
                    Path.Combine(destination, "yt-dlp-error.log"),
                    stderr);
            }
        }
    }
}