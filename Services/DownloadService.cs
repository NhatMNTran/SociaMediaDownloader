using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SocialMediaDownloader.Services
{
    public class DownloadService
    {
        private readonly string ytDlpPath =
            Path.Combine("Assets", "yt-dlp.exe");

        public async Task Download(
            string url,
            string destination,
            string formatId)
        {
            ProcessStartInfo psi = new()
            {
                FileName = ytDlpPath,
                Arguments =
                    $"-f {formatId} -o \"{destination}\\%(title)s.%(ext)s\" {url}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(psi);

            await process.WaitForExitAsync();
        }
    }
}