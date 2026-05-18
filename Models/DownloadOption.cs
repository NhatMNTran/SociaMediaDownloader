namespace SocialMediaDownloader.Models
{
    public class DownloadOption
    {
        // The label is what is shown to the user, while the format string is what is passed to yt-dlp for parameters
        public string Label { get; set; }

        public string FormatString { get; set; }

        //Check if the option is an audio file
        public bool IsAudio { get; set; }

        public override string ToString()
        {
            return Label;
        }
    }
}