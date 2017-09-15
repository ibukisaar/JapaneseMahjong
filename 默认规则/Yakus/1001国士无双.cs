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

		protected override void Syanten(IBaseTiles tiles, ref int result) {
			int terminalCount = 0;
			int pairFlag = 0;

			for (int i = 0; i < 34; i++) {
				if (tiles.Sorted.Tiles[i] == 0) continue;
				if (!BaseTile.AllTiles[i].IsTerminal) continue;

				if (pairFlag == 0 && tiles.Sorted.Tiles[i] >= 2) {
					pairFlag = 1;
				}
				terminalCount++;
			}

			int num = 13 - pairFlag - terminalCount;
			if (num < result) result = num;
		}

		protected override bool TestRon(ITiles tiles, YakuEnvironment env) {
			long flags = 0;
			for (int i = 0; i < tiles.Count; i++) flags |= 1L << tiles[i].BaseTile.SortedIndex;
			return flags == CheckFlags;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (!TestRon(tiles, env)) return true;

			if (tiles.Count(t => t.BaseTile == tiles.Added.BaseTile) == 2) {
				result.Add(YakuValue.FromFullYaku(this, "国士无双十三面", 13));
			} else {
				result.Add(YakuValue.FromFullYaku(this, "国士无双", 13));
			}
			return true;
		}
	}
}
