using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public sealed class AdvancedGroups : IReadOnlyList<IGroups> {
		private IGroups[] resultGroups;

		public Group[] Groups { get; }
		public int[] KindCounts { get; }
		public int[] KindCountsWithoutPair { get; }
		public int Count => resultGroups.Length;

		public IGroups this[int index] => resultGroups[index];

		public AdvancedGroups(Group[] closeGroups, Tile extra, Group[] openGroups) {
			Array.Sort(closeGroups);
			Groups = new Group[closeGroups.Length + openGroups.Length];
			closeGroups.CopyTo(Groups, 0);
			openGroups.CopyTo(Groups, closeGroups.Length);

			KindCounts = new int[4];
			KindCountsWithoutPair = new int[4];
			foreach (var g in Groups) {
				if (!g.IsImportant) continue;
				KindCounts[g.Key.SortedLevel]++;
				if (g is Pair) continue;
				KindCountsWithoutPair[g.Key.SortedLevel]++;
			}

			(int GroupIndex, int TileIndex) extraIndex = default;
			for (int i = 0; i < closeGroups.Length; i++) {
				for (int j = 0; j < closeGroups[i].Tiles.Length; j++) {
					if (closeGroups[i].Tiles[j].Id == extra.Id) {
						extraIndex = (i, j);
						goto Exit;
					}
				}
			}
			Exit:

			var indexes = new Dictionary<(int SortedLevel, int TileIndex), int/*GroupIndex*/>();
			for (int i = 0; i < closeGroups.Length; i++) {
				for (int j = 0; j < closeGroups[i].Tiles.Length; j++) {
					if (closeGroups[i].Tiles[j].BaseTile == extra.BaseTile) {
						if (!indexes.ContainsKey((closeGroups[i].SortedLevel, j))) {
							indexes.Add((closeGroups[i].SortedLevel, j), i);
						}
						break; // 此处保证任意Group只计算一处索引
					}
				}
			}

			resultGroups = new IGroups[indexes.Count];
			int dstIndex = 0;
			foreach (var kv in indexes) {
				(int GroupIndex, int TileIndex) swapTarget = (kv.Value, kv.Key.TileIndex);
				resultGroups[dstIndex++] = Build(Groups, swapTarget, extraIndex);
			}
		}

		private static void Swap(ref Tile x, ref Tile y) {
			var t = x;
			x = y;
			y = t;
		}

		private static IGroups Build(Group[] groups, (int GroupIndex, int TileIndex) swapTarget, (int GroupIndex, int TileIndex) extraIndex) {
			var tempGroups = new Group[groups.Length];
			var tempTiles = new Tile[tempGroups.Length][];
			for (int i = 0; i < tempGroups.Length; i++) {
				tempTiles[i] = groups[i].Tiles.Clone() as Tile[];
			}

			if (!swapTarget.Equals(extraIndex)) {
				Swap(ref tempTiles[swapTarget.GroupIndex][swapTarget.TileIndex],
					 ref tempTiles[extraIndex.GroupIndex][extraIndex.TileIndex]);
			}

			for (int i = 0; i < tempGroups.Length; i++) {
				if (i == swapTarget.GroupIndex) {
					tempGroups[i] = groups[i].Remodeling(tempTiles[i], GroupType.和牌, swapTarget.TileIndex);
				} else if (i == extraIndex.GroupIndex) {
					tempGroups[i] = groups[i].Remodeling(tempTiles[i], groups[i].Type, groups[i].AddedIndex);
				} else {
					tempGroups[i] = groups[i];
				}
			}

			return new GroupCollection(tempGroups);
		}

		public IEnumerator<IGroups> GetEnumerator() {
			foreach (var g in resultGroups) yield return g;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override string ToString() => string.Join(" # ", this);
	}
}
