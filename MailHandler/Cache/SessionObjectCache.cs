using System;
using System.Collections.Generic;

namespace MailHandler.Cache
{
	public class SessionObjectCache<TKey, TValue>
	{
		protected Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

		private readonly Func<TKey, TValue> _source;

		public SessionObjectCache(Func<TKey, TValue> source)
		{
			_source = source;
		}

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

		public void Clear()
		{
			cache.Clear();
		}
	}
}
