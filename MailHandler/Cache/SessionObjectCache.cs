using System;
using System.Collections.Generic;

namespace MailHandler.Cache
{
	/// <summary>
	/// A cache object which defaults to a getter when a key cannot be found
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class SessionObjectCache<TKey, TValue>
	{
		/// <summary>
		/// The cache
		/// </summary>
		protected Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

		/// <summary>
		/// The source
		/// </summary>
		private readonly Func<TKey, TValue> _source;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionObjectCache{TKey, TValue}"/> class.
		/// </summary>
		/// <param name="source">The source.</param>
		public SessionObjectCache(Func<TKey, TValue> source)
		{
			_source = source;
		}

		/// <summary>
		/// Gets the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// The associated value.
		/// </returns>
		public TValue Get(TKey key)
		{
			if (cache.ContainsKey(key))
			{
				return cache[key];
			}
			else
			{
				TValue value = _source.Invoke(key);
				cache[key] = value;
				return value;
			}
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			cache.Clear();
		}
	}
}
