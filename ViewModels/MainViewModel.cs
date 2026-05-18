using SocialMediaDownloader.Helpers;
using SocialMediaDownloader.Models;
using SocialMediaDownloader.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace SocialMediaDownloader.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Services for interacting with yt-dlp, handling downloads, and showing file dialogs
        private readonly YtDlpService ytDlpService = new();
        private readonly DownloadService downloadService = new();
        private readonly FileDialogService fileDialogService = new();

        public event PropertyChangedEventHandler PropertyChanged;

        private string url;
        public string Url
        {
            get => url;
            set
            {
                if (url != value)
                {
                    url = value;

                    Progress = 0;
                    Status = "Ready";

                    Formats.Clear();

                    SelectedFormat = null;

                    OnPropertyChanged();
                }
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        private string platform;
        public string Platform
        {
            get => platform;
            set
            {
                platform = value;
                OnPropertyChanged();
            }
        }

        private string destinationPath;
        public string DestinationPath
        {
            get => destinationPath;
            set
            {
                destinationPath = value;
                OnPropertyChanged();
            }
        }

        private double progress;
        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged();
            }
        }

        private string status;
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }

        private string fileName;

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged();
            }
        }

        // Method to sanitize file names by replacing invalid characters with underscores
        private string SanitizeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name;
        }

        // Collection of available download options for the media, which is populated after checking the media info
        public ObservableCollection<DownloadOption> Formats { get; set; }
            = new();

        private DownloadOption selectedFormat;
        public DownloadOption SelectedFormat
        {
            get => selectedFormat;
            set
            {
                if (selectedFormat != value)
                {
                    selectedFormat = value;

                    Progress = 0;
                    Status = "Ready";

                    OnPropertyChanged();
                }
            }
        }

        public ICommand CheckCommand => new RelayCommand(async () =>
        {
            await CheckMedia();
        });

        public ICommand BrowseCommand => new RelayCommand(() =>
        {
            DestinationPath =
                fileDialogService.SelectFolder();
        });

        public ICommand DownloadCommand => new RelayCommand(async () =>
        {
            await DownloadMedia();
        });

        private async Task CheckMedia()
        {
            Progress = 0;
            Status = "Checking media...";

            MediaInfo info =
                await ytDlpService.GetMediaInfo(Url);

            Title = info.Title;
            FileName = SanitizeFileName(info.Title);
            Platform = info.Platform;

            Formats.Clear();

            foreach (var format in info.Formats)
            {
                Formats.Add(format);
            }

            Status = "Ready";
        }

        private async Task DownloadMedia()
        {
            if (SelectedFormat == null)
                return;

            Status = "Downloading...";

            await downloadService.Download(
                Url,
                DestinationPath,
                FileName,
                SelectedFormat);

            Progress = 100;

            Status = "Download Complete";
        }

        // Helper method to raise the PropertyChanged event when a property value changes, allowing the UI to update accordingly
        private void OnPropertyChanged(
            [CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name));
        }
    }
}