using System.Windows;
using SocialMediaDownloader.ViewModels;

namespace SocialMediaDownloader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}