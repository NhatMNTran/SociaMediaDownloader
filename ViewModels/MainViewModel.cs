using SocialMediaDownloader.Helpers;
using SocialMediaDownloader.Models;
using SocialMediaDownloader.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SocialMediaDownloader.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
                url = value;
                OnPropertyChanged();
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

        public ObservableCollection<DownloadOption> Formats { get; set; }
            = new();

        private DownloadOption selectedFormat;
        public DownloadOption SelectedFormat
        {
            get => selectedFormat;
            set
            {
                selectedFormat = value;
                OnPropertyChanged();
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
            Status = "Checking media...";

            MediaInfo info =
                await ytDlpService.GetMediaInfo(Url);

            Title = info.Title;
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
                SelectedFormat);

            Progress = 100;

            Status = "Download Complete";
        }

        private void OnPropertyChanged(
            [CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(name));
        }
    }
}