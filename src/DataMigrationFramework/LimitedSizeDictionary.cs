using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataMigrationFramework
{
    /// <summary>
    /// Limits the size of the dictionary to certain items.
    /// </summary>
    /// <typeparam name="TKey">
    /// Dictionary key.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// Dictionary value.
    /// </typeparam>
    internal class LimitedSizeDictionary<TKey, TValue>
    {
        /// <summary>
        /// Size of the dictionary.
        /// </summary>
        private readonly int _size;

        private readonly int _trimSize;
        // private readonly IEqualityComparer<TKey> _comparer;
        private readonly Comparer<TKey> _comparer;

        /// <summary>
        /// Actual Dictionary.
        /// </summary>
        private IDictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Backend list to maintain the dictionary items and trim whenever it is needed.
        /// </summary>
        private IList<MutableKeyValuePair<TKey, TValue>> _backendList = new List<MutableKeyValuePair<TKey, TValue>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedSizeDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="size">
        /// Size of the dictionary.
        /// </param>
        /// <param name="trimSize">
        /// Trim size where it will be trimmed by.
        /// <example>
        /// For size 100 and trim size =10 , whenever the size reaches to 110 it will  be trimmed by 10 to make it 100 again.
        /// </example>
        /// </param>
        /// <param name="comparer">
        /// A <see cref="IEqualityComparer{T}"/> comparer.
        /// </param>
        public LimitedSizeDictionary(int size, int trimSize, Comparer<TKey> comparer)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            if (trimSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(trimSize));
            }

            this._size = size + trimSize;
            this._trimSize = trimSize;
            this._comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            this._dictionary = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// Gets under laying dictionary as read only.
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> Dictionary => new ReadOnlyDictionary<TKey, TValue>(this._dictionary);

        /// <summary>
        /// Adds entry and trims if the size limit is reached.
        /// </summary>
        /// <param name="entry">
        /// A <see cref="KeyValuePair{TKey,TValue}"/> entry.
        /// </param>
        public void Add(KeyValuePair<TKey, TValue> entry)
        {
            if (this._dictionary.ContainsKey(entry.Key))
            {
                this._dictionary[entry.Key] = entry.Value;
                var found = this._backendList.FirstOrDefault(item => this._comparer.Compare(item.Key, entry.Key) == 0);
                if (found != null)
                {
                    found.Value = entry.Value;
                }
            }
            else
            {
                this._dictionary[entry.Key] = entry.Value;
                this._backendList.Add(new MutableKeyValuePair<TKey, TValue>(entry.Key, entry.Value));
            }

            if (this._backendList.Count >= this._size)
            {
                this._backendList = this._backendList.Skip(this._trimSize).ToList();
                this._dictionary = this._backendList.ToDictionary(item => item.Key, item => item.Value);
            }
        }

        /// <summary>
        /// Removes the item from dictionary.
        /// </summary>
        /// <param name="id">
        /// Key vale.
        /// </param>
        public void Remove(TKey id)
        {
            this._dictionary.Remove(id);
            var found = this._backendList.FirstOrDefault(item => this._comparer.Compare(item.Key, id) == 0);
            if (found != null)
            {
                this._backendList.Remove(found);
            }
        }

        /// <summary>
        /// Key value pair where a value can be updated.
        /// </summary>
        /// <typeparam name="TKey1">
        /// Key type name.
        /// </typeparam>
        /// <typeparam name="TValue1">
        /// Value type name.
        /// </typeparam>
        private class MutableKeyValuePair<TKey1, TValue1>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MutableKeyValuePair{TKey1,TValue1}"/> class.
            /// </summary>
            /// <param name="key">
            /// Name of the key.
            /// </param>
            /// <param name="val">
            /// Value of the key.
            /// </param>
            public MutableKeyValuePair(TKey1 key, TValue1 val)
            {
                this.Key = key;
                this.Value = val;
            }

            /// <summary>
            /// Gets key name.
            /// </summary>
            public TKey1 Key { get; }

            /// <summary>
            /// Gets or sets value of the key.
            /// </summary>
            public TValue1 Value { get; set; }
        }
    }
}
