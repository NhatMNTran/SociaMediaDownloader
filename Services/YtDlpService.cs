using Newtonsoft.Json.Linq;
using SocialMediaDownloader.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaDownloader.Services
{
    public class YtDlpService
    {
        private readonly string ytDlpPath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Assets",
                "yt-dlp.exe");

        public async Task<MediaInfo> GetMediaInfo(string url)
        {
            ProcessStartInfo psi = new()
            {
                FileName = ytDlpPath,
                Arguments = $"-J \"{url}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(psi);

            string json =
                await process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            JObject obj = JObject.Parse(json);

            List<DownloadOption> formats = new();

            var allFormats = obj["formats"];

            var videoFormats = allFormats
                .Where(f =>
                    f["vcodec"]?.ToString() != "none" &&
                    f["height"] != null)
                .GroupBy(f => f["height"]?.ToString())
                .Select(g => g.First())
                .OrderByDescending(f => (int?)f["height"]);

            foreach (var format in videoFormats)
            {
                string height =
                    format["height"]?.ToString();

                string formatId =
                    format["format_id"]?.ToString();

                formats.Add(new DownloadOption
                {
                    Label = $"MP4 {height}p",
                    FormatString =
                        $"{formatId}+bestaudio",
                    IsAudio = false
                });
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