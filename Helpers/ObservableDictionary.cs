using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BoardGameFrontend.Helpers
{
    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        public new bool Remove(TKey key)
        {
            if (TryGetValue(key, out var value) && base.Remove(key))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                return true;
            }
            return false;
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}