using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Caliburn.Micro;
using ObservablePropertyChanged;

namespace ExampleApp
{
    public class ObservableScreen : Screen, IObservablePropertyChanged
    {
        private ObservablePropertyChangeHelper observableHelper = new ObservablePropertyChangeHelper();

        public ObservableScreen()
        {
            Changed = observableHelper.ChangedObservable;
        }

        public IObservable<PropertyChangeData> Changed { get; }

        public override void NotifyOfPropertyChange([CallerMemberName] string propertyName = null)
        {
            base.NotifyOfPropertyChange(propertyName);
            observableHelper.PropertyChanged(this, propertyName);
        }

        public virtual void Dispose()
        {
            Interlocked.Exchange(ref observableHelper, null)?.Dispose();
        }
    }
}