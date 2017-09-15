using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将.Yakus;

namespace 日本麻将 {
	public sealed class AdvancedGroups : IEnumerable<IGroups> {
		public Group[] Groups { get; }
		public (int GroupIndex, int TileIndex)[] Indexes { get; }
		public (int GroupIndex, int TileIndex) ExtraIndex { get; }
		public int[] KindCounts { get; }
		public int[] KindCountsWithoutPair { get; }

		public AdvancedGroups(Group[] closeGroups, Tile extra, Group[] openGroups) {
			Groups = closeGroups.Concat(openGroups).ToArray();
			KindCounts = new int[4];
			KindCountsWithoutPair = new int[4];
			foreach (var g in Groups) {
				if (!g.IsImportant) continue;
				KindCounts[g.Key.SortedLevel]++;
				if (g is Pair) continue;
				KindCountsWithoutPair[g.Key.SortedLevel]++;
			}
			
			var indexes = new Dictionary<(int SortedLevel, int TileIndex), int/*GroupIndex*/>();
			for (int i = 0; i < closeGroups.Length; i++) {
				for (int j = 0; j < closeGroups[i].Tiles.Length; j++) {
					if (closeGroups[i].Tiles[j].BaseTile == extra.BaseTile) {
						if (closeGroups[i].Tiles[j].Id == extra.Id) ExtraIndex = (i, j);
						if (!indexes.ContainsKey((closeGroups[i].SortedLevel, j))) {
							indexes.Add((closeGroups[i].SortedLevel, j), i);
						}
						break; // 此处保证任意Group只计算一处索引
					}
				}
			}

			Indexes = indexes.Select(kv => (kv.Value, kv.Key.TileIndex)).ToArray();
		}

		public IEnumerator<IGroups> GetEnumerator() {
			void Swap(ref Tile x, ref Tile y) => (x, y) = (y, x);

			foreach (var swapTarget in Indexes) {
				var tempGroups = new Group[Groups.Length];
				var tempTiles = Groups.Select(g => g.Tiles.Clone() as Tile[]).ToArray();

				if (!swapTarget.Equals(ExtraIndex)) {
					Swap(ref tempTiles[swapTarget.GroupIndex][swapTarget.TileIndex],
						 ref tempTiles[ExtraIndex.GroupIndex][ExtraIndex.TileIndex]);
				}

				for (int i = 0; i < tempGroups.Length; i++) {
					if (i == swapTarget.GroupIndex) {
						tempGroups[i] = Groups[i].Remodeling(tempTiles[i], GroupType.和牌, swapTarget.TileIndex);
					} else if (i == ExtraIndex.GroupIndex) {
						tempGroups[i] = Groups[i].Remodeling(tempTiles[i], Groups[i].Type, Groups[i].AddedIndex);
					} else {
						tempGroups[i] = Groups[i];
					}
				}

				yield return new GroupCollection(tempGroups);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override string ToString() => string.Join(" # ", this);
	}
}
