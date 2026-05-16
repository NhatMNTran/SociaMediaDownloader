namespace SocialMediaDownloader.Models
{
    public class DownloadOption
    {
        public string FormatId { get; set; }

        public string Extension { get; set; }

        public string Resolution { get; set; }

        public bool AudioOnly { get; set; }

        public string DisplayText =>
            AudioOnly
                ? $"{Extension.ToUpper()} Audio"
                : $"{Extension.ToUpper()} {Resolution}";
    }
}