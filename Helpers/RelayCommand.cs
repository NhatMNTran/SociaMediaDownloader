using System;
using System.Windows.Input;

namespace SocialMediaDownloader.Helpers
{
    //Relay commands from the WPF interface to the code
    public class RelayCommand : ICommand
    {
        private readonly Action execute;

        //When a button is clicked, the action is executed
        public RelayCommand(Action execute)
        {
            this.execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        //execute the stored command
        public void Execute(object parameter)
        {
            execute();
        }
    }
}