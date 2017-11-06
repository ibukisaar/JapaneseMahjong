using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public sealed class SuggestResult : IReadOnlyDictionary<BaseTile, SuggestResult.SuggestItem> {
		public sealed class SuggestItem : IComparable<SuggestItem> {
			public BaseTile Key { get; }
			public (BaseTile Tile, int Count)[] Values { get; }
			public int TotalCount { get; }

			internal SuggestItem(BaseTile key, IEnumerable<(BaseTile Tile, int Count)> values) {
				Key = key;
				Values = values.ToArray();
				TotalCount = Values.Sum(v => v.Count);
			}

			public override string ToString() => $"打“{Key}” 摸{TotalCount}枚: {string.Join(" ", Values.Select(v => $"{v.Tile}({v.Count})"))}";

			public int CompareTo(SuggestItem other) => TotalCount - other.TotalCount;
		}

		private Dictionary<BaseTile, SuggestItem> resultDict = new Dictionary<BaseTile, SuggestItem>();

		public int Syanten { get; }
		public SuggestItem this[BaseTile key] => resultDict[key];
		public IEnumerable<BaseTile> Keys => resultDict.Keys;
		public IEnumerable<SuggestItem> Values => resultDict.Values;

		public int Count => resultDict.Count;

		internal SuggestResult(int syanten) => Syanten = syanten;

		internal void Add(SuggestItem item) => resultDict.Add(item.Key, item);

		public bool ContainsKey(BaseTile key) => resultDict.ContainsKey(key);

		public IEnumerator<KeyValuePair<BaseTile, SuggestItem>> GetEnumerator() => resultDict.GetEnumerator();

		public bool TryGetValue(BaseTile key, out SuggestItem value) => resultDict.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override string ToString() {
			switch (Syanten) {
				case -1: return $"和牌，{resultDict.Count}条建议";
				case 0: return $"听牌，{resultDict.Count}条建议";
				default: return $"{Syanten}向听，{resultDict.Count}条建议";
			}
		}

		public void Sort() {
			var values = resultDict.Values.OrderByDescending(v => v).ToList();
			resultDict.Clear();
			foreach (var value in values) {
				Add(value);
			}
		}
	}
}
