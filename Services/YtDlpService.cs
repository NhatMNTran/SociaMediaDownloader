using Newtonsoft.Json.Linq;
using SocialMediaDownloader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace SocialMediaDownloader.Services
{
    public class YtDlpService
    {
        // Path to the yt-dlp executable, which is included in the Assets folder of the application
        private readonly string ytDlpPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "yt-dlp.exe");

        // Method to get media information from a given URL using yt-dlp
        public async Task<MediaInfo> GetMediaInfo(string url)
        {
            // Set up the process start info to run yt-dlp with the appropriate arguments
            ProcessStartInfo psi = new()
            {
                FileName = ytDlpPath,
                Arguments = $"-J \"{url}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process and read the output, which is expected to be in JSON format
            using Process process = Process.Start(psi);

            string json =
                await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            JObject obj = JObject.Parse(json);

            List<DownloadOption> formats = new();

            var allFormats = obj["formats"];

            // Filter formats to get only video formats, group by height (1080, 720, 480, etc.)to avoid duplicates, and order by resolution
            var videoFormats = allFormats
                .Where(f =>
                    f["vcodec"]?.ToString() != "none" &&
                    f["height"] != null)
                .GroupBy(f => f["height"]?.ToString())
                .Select(g => g.First())
                .OrderByDescending(f => (int?)f["height"]);
            
            // For Twitter/X, we can only get the best video format, so we add a single option for that
            if (url.Contains("twitter.com") ||
                url.Contains("x.com"))
            {
                formats.Add(new DownloadOption
                {
                    Label = "MP4 Video",
                    FormatString = "b",
                    IsAudio = false
                });
            }
            else
            {
                foreach (var format in videoFormats)
                {
                    string height =
                        format["height"]?.ToString();

                    string formatId =
                        format["format_id"]?.ToString();

                    if (string.IsNullOrWhiteSpace(formatId))
                        continue;

                    formats.Add(new DownloadOption
                    {
                        Label = $"MP4 {height}p",
                        FormatString =
                            $"{formatId}+bestaudio",
                        IsAudio = false
                    });
                }
            }
            

            formats.Add(new DownloadOption
            {
                Label = "MP3 Audio",
                FormatString = "bestaudio",
                IsAudio = true
            });

            return new MediaInfo
            {
                Title = obj["title"]?.ToString(),
                Platform = obj["extractor"]?.ToString(),
                Formats = formats
            };
        }
    }
}