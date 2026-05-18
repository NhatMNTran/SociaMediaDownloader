namespace SocialMediaDownloader.Services
{
    public class FileDialogService
    {
        // Method to open a folder browser dialog and return the selected path
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
