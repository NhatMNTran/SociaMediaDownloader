using System.Windows.Forms;

namespace SocialMediaDownloader.Services
{
    public class FileDialogService
    {
        public string SelectFolder()
        {
            using FolderBrowserDialog dialog = new();

            return dialog.ShowDialog() ==
                   DialogResult.OK
                ? dialog.SelectedPath
                : string.Empty;
        }
    }
}
