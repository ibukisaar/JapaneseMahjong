using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public static class Ex {
		private static readonly Random random = new Random();

		public static void Random<T>(this T[] array, int count, Random random = null) {
			random = random ?? Ex.random;

			int length = array.Length;
			for (int i = 0; i < count; i++) {
				int j = random.Next(i, length);
				T t = array[i];
				array[i] = array[j];
				array[j] = t;
			}
		}

		public static void Deconstruction<TKey, TValue>(this KeyValuePair<TKey, TValue> kv, out TKey key, out TValue value) {
			key = kv.Key;
			value = kv.Value;
		}
	}
}
