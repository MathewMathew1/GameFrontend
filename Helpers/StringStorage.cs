using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGameFrontend.Helpers
{
    public class StringStorage
    {
        private readonly HashSet<string> _storage;

        public StringStorage()
        {
            _storage = new HashSet<string>();
        }

        public bool ContainsString(string item)
        {
            var contains = _storage.Contains(item);
            if (!contains)
            {
                _storage.Add(item);
            }
            return contains;
        }

        public bool AddString(string item)
        {
            return _storage.Add(item);
        }


        public bool RemoveString(string item)
        {
            return _storage.Remove(item);
        }

        public async Task RunIfNotExistsAsync(string value, Func<Task> function)
        {
            if (ContainsString(value))
            {
                Console.WriteLine($"Operation '{value}' is already in progress.");
                return;
            }

            AddString(value);

            try
            {
                await function();
            }
            finally
            {
                RemoveString(value);
            }
        }
    }
}
