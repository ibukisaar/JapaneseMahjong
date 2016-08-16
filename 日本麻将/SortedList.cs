using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace 日本麻将 {
	class SortedList<T> : IEnumerable<T> where T : IComparable<T> {
		private List<T> buffer;

		public SortedList() {
			buffer = new List<T>();
		}

		public SortedList(IEnumerable<T> values) {
			buffer = new List<T>(values.OrderByDescending(value => value));
		}

		public void Add(T value) {
			int i = buffer.Count;
			for (; i > 0 && buffer[i - 1].CompareTo(value) < 0; i--) ;
			buffer.Insert(i, value);
		}

		public bool Remove(T value) {
			int i = buffer.Count - 1;
			for (; i >= 0 && buffer[i].CompareTo(value) < 0; i--) ;

			if (i >= 0 && buffer[i].CompareTo(value) == 0) {
				buffer.RemoveAt(i);
				return true;
			}
			return false;
		}

		public T this[int index] {
			get {
				return buffer[buffer.Count - 1 - index];
			}
		}

		public int Count { get { return buffer.Count; } }

		public bool DifferentNext(T value, out T nextValue) {
			int i = buffer.Count - 1;
			for (; i >= 0 && buffer[i].CompareTo(value) <= 0; i--) ;

			if (i >= 0) {
				nextValue = buffer[i];
				return true;
			} else {
				nextValue = default(T);
				return false;
			}
		}

		public SortedList<T> Clone() {
			SortedList<T> other = new SortedList<T>();
			other.buffer = new List<T>(buffer);
			return other;
		}

		public IEnumerator<T> GetEnumerator() {
			for (int i = buffer.Count - 1; i >= 0; i--) {
				yield return buffer[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
