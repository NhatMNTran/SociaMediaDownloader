using Newtonsoft.Json.Linq;
using SocialMediaDownloader.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SocialMediaDownloader.Services
{
    public class YtDlpService
    {
        private readonly string ytDlpPath =
            Path.Combine("Assets", "yt-dlp.exe");

        public async Task<MediaInfo> GetMediaInfo(string url)
        {
            ProcessStartInfo psi = new()
            {
                FileName = ytDlpPath,
                Arguments = $"-J {url}",
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

            foreach (var format in obj["formats"])
            {
                string ext = format["ext"]?.ToString();

                if (ext == null)
                    continue;

                formats.Add(new DownloadOption
                {
                    FormatId = format["format_id"]?.ToString(),
                    Extension = ext,
                    Resolution = format["resolution"]?.ToString(),
                    AudioOnly =
                        format["vcodec"]?.ToString() == "none"
                });
            }

            return new MediaInfo
            {
                Title = obj["title"]?.ToString(),
                Platform = obj["extractor"]?.ToString(),
                Formats = formats
            };
        }
    }
}