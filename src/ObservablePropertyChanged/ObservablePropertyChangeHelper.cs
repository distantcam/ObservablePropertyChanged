using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ObservablePropertyChanged
{
    public class ObservablePropertyChangeHelper : IDisposable
    {
        private static Func<object, object> nullGetter = sender => null;
        private static IDictionary<Type, IDictionary<string, Func<object, object>>> getterCache = new Dictionary<Type, IDictionary<string, Func<object, object>>>();

        private Subject<PropertyChangeData> changing = new Subject<PropertyChangeData>();
        private Subject<PropertyChangeData> changed = new Subject<PropertyChangeData>();

        public ObservablePropertyChangeHelper()
        {
            ChangingObservable = changing.AsObservable();
            ChangedObservable = changed.AsObservable();
        }

        public IObservable<PropertyChangeData> ChangingObservable { get; }
        public IObservable<PropertyChangeData> ChangedObservable { get; }

        public void PropertyChanging(object sender, string propertyName)
        {
            var getter = GetPropertyGetter(sender, propertyName);
            changing.OnNext(new PropertyChangeData(sender, propertyName, getter(sender)));
        }

        public void PropertyChanging(object sender, string propertyName, object value)
        {
            changing.OnNext(new PropertyChangeData(sender, propertyName, value));
        }

        public void PropertyChanged(object sender, string propertyName)
        {
            var getter = GetPropertyGetter(sender, propertyName);
            changed.OnNext(new PropertyChangeData(sender, propertyName, getter(sender)));
        }

        public void PropertyChanged(object sender, string propertyName, object value)
        {
            changed.OnNext(new PropertyChangeData(sender, propertyName, value));
        }

        public void Dispose()
        {
            var localChanged = Interlocked.Exchange(ref changed, null);
            if (localChanged != null)
            {
                localChanged.OnCompleted();
                localChanged.Dispose();
            }
        }

        private static Func<object, object> GetPropertyGetter(object sender, string propertyName)
        {
            var type = sender.GetType();
            var propertyCache = GetOrAdd(getterCache, type, () => new Dictionary<string, Func<object, object>>());
            var getter = GetOrAdd(propertyCache, propertyName, () =>
            {
                var property = type.GetProperties().FirstOrDefault(p => p.Name == propertyName);
                if (property == null)
                    return nullGetter;

                var senderArg = Expression.Parameter(typeof(object));
                Expression body = Expression.Property(
                        Expression.Convert(senderArg, type),
                        propertyName);
                if (property.PropertyType.IsValueType)
                    body = Expression.Convert(body, typeof(object));

                return Expression.Lambda<Func<object, object>>(body, senderArg).Compile();
            });

            return getter;
        }

        private static TValue GetOrAdd<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFunc)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueFunc();
                dictionary.Add(key, value);
            }
            return value;
        }
    }
}