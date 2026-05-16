using System.Collections.Generic;

namespace SocialMediaDownloader.Models
{
    public class MediaInfo
    {
        public string Title { get; set; }

        public string Platform { get; set; }

        public List<DownloadOption> Formats { get; set; }
    }
}
