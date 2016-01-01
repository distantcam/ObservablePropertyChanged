using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ObservablePropertyChanged
{
    public static class ObservablePropertyChangedExtensions
    {
        public static IObservable<PropertyChangeData> WhenPropertyChanging(this IObservablePropertyChanging observable, string propertyName)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return observable.Changing.ForProperty(propertyName);
        }

        public static IObservable<PropertyChangeData> WhenPropertiesChanging(this IObservablePropertyChanging observable, params string[] propertyNames)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (propertyNames == null || propertyNames.Length == 0)
                throw new ArgumentException($"{nameof(propertyNames)} is null or empty.", nameof(propertyNames));

            return observable.Changing.ForProperties(propertyNames);
        }

        public static IObservable<PropertyChangeData> WhenPropertyChanged(this IObservablePropertyChanged observable, string propertyName)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return observable.Changed.ForProperty(propertyName);
        }

        public static IObservable<PropertyChangeData> WhenPropertiesChanged(this IObservablePropertyChanged observable, params string[] propertyNames)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (propertyNames == null || propertyNames.Length == 0)
                throw new ArgumentException($"{nameof(propertyNames)} is null or empty.", nameof(propertyNames));

            return observable.Changed.ForProperties(propertyNames);
        }

        public static IObservable<PropertyChangeData> ForProperty(this IObservable<PropertyChangeData> observable, string propertyName)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{nameof(propertyName)} is null or empty.", nameof(propertyName));

            return observable.Where(p => p.PropertyName == propertyName);
        }

        public static IObservable<PropertyChangeData> ForProperties(this IObservable<PropertyChangeData> observable, params string[] propertyNames)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (propertyNames == null || propertyNames.Length == 0)
                throw new ArgumentException($"{nameof(propertyNames)} is null or empty.", nameof(propertyNames));

            if (propertyNames.Length == 1)
                return ForProperty(observable, propertyNames[0]);

            var hashset = new HashSet<string>(propertyNames);
            return observable.Where(p => hashset.Contains(p.PropertyName));
        }

        public static IDisposable ExecuteCommand(this IObservable<PropertyChangeData> observable, ICommand command)
        {
            if (observable == null)
                throw new ArgumentNullException(nameof(observable), $"{nameof(observable)} is null.");
            if (command == null)
                throw new ArgumentNullException(nameof(command), $"{nameof(command)} is null.");

            return observable.Subscribe(arg =>
            {
                if (command.CanExecute(arg.Value))
                    command.Execute(arg.Value);
            });
        }
    }
}