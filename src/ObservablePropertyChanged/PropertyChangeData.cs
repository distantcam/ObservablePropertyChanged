namespace ObservablePropertyChanged
{
    public class PropertyChangeData
    {
        public PropertyChangeData(object sender, string propertyName, object value)
        {
            Sender = sender;
            PropertyName = propertyName;
            Value = value;
        }

        public object Sender { get; }

        public string PropertyName { get; }

        public object Value { get; }
    }
}