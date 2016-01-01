using System;

namespace ObservablePropertyChanged
{
    public interface IObservablePropertyChanged : IDisposable
    {
        IObservable<PropertyChangeData> Changed { get; }
    }
}