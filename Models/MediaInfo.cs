using System.Collections.Generic;

namespace SocialMediaDownloader.Models
{
    public class MediaInfo
    {
        // The title of the media, used for naming the file and displaying to the user
        public string Title { get; set; }

        //The social media platform the media is from
        public string Platform { get; set; }

        // The available download options for the media, such as video quality or audio format
        public List<DownloadOption> Formats { get; set; }
    }
}
