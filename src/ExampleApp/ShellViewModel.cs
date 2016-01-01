using System;
using System.Diagnostics;
using System.Windows.Input;
using ObservablePropertyChanged;

namespace ExampleApp
{
    public class ShellViewModel : ObservableScreen
    {
        public ShellViewModel()
        {
            Message = new MessageCommand();

            this.WhenPropertyChanged(nameof(Item)).ExecuteCommand(Message);
        }

        private string item;

        public string Item
        {
            get { return item; }
            set
            {
                item = value;
                NotifyOfPropertyChange(nameof(Item));
            }
        }

        public ICommand Message { get; }

        public override void Dispose()
        {
            base.Dispose();
            Trace.WriteLine("Dispose");
        }

        private class MessageCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return ((string)parameter).Length % 2 == 0;
            }

            public void Execute(object parameter)
            {
                Trace.WriteLine(parameter);
            }
        }
    }
}