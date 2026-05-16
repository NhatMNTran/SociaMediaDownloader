namespace SocialMediaDownloader.Models
{
    public class DownloadOption
    {
        public string Label { get; set; }

        public string FormatString { get; set; }

        public bool IsAudio { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}