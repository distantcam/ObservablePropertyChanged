using System;

namespace ObservablePropertyChanged
{
    public interface IObservablePropertyChanging : IDisposable
    {
        IObservable<PropertyChangeData> Changing { get; }
    }
}