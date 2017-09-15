using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 国士无双 : SpecialYaku {
		private static readonly long CheckFlags =
			(1L << Tiles.一万.SortedIndex)
			| (1L << Tiles.九万.SortedIndex)
			| (1L << Tiles.一饼.SortedIndex)
			| (1L << Tiles.九饼.SortedIndex)
			| (1L << Tiles.一索.SortedIndex)
			| (1L << Tiles.九索.SortedIndex)
			| (1L << Tiles.东.SortedIndex)
			| (1L << Tiles.南.SortedIndex)
			| (1L << Tiles.西.SortedIndex)
			| (1L << Tiles.北.SortedIndex)
			| (1L << Tiles.白.SortedIndex)
			| (1L << Tiles.发.SortedIndex)
			| (1L << Tiles.中.SortedIndex);

		public override int OrderIndex => 1001;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int[] kindCountsFromTiles) {
			return kindCountsFromTiles[0] >= 2 && kindCountsFromTiles[1] >= 2 && kindCountsFromTiles[2] >= 2 && kindCountsFromTiles[3] >= 7;
		}

		protected internal override int Syanten(SortedTilesEnumerator tiles) {
			if (tiles.Count == 13) {
				int terminalCount = 0;
				int pairFlag = 0;

				for (int i = 0; i < 34; i++) {
					if (tiles.Tiles[i] == 0) continue;
					if (!BaseTile.AllTiles[i].IsTerminal) continue;

					if (pairFlag == 0 && tiles.Tiles[i] >= 2) {
						pairFlag = 1;
					}
					terminalCount++;
				}

				return 13 - pairFlag - terminalCount;
			}
			return 6;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			long flags = 0;
			for (int i = 0; i < tiles.Length; i++) flags |= (1L << tiles[i].BaseTile.SortedIndex);
			if (flags != CheckFlags) return true;

			var last = tiles[tiles.Length - 1];
			if (tiles.Count(t => t == last) == 2) {
				result.Add(YakuValue.FromFullYaku("国士无双十三面", 13));
			} else {
				result.Add(YakuValue.FromFullYaku("国士无双", 13));
			}
			return true;
		}
	}
}
