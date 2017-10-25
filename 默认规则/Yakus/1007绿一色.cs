using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 绿一色 : Yaku {
		private static readonly long CheckFlags =
			(1L << Tiles.二索.SortedIndex)
			| (1L << Tiles.三索.SortedIndex)
			| (1L << Tiles.四索.SortedIndex)
			| (1L << Tiles.六索.SortedIndex)
			| (1L << Tiles.八索.SortedIndex)
			| (1L << Tiles.发.SortedIndex);

		public override int OrderIndex => 1007;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount + pungCount > 0;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			if (kindCounts[3] > 1) return false;
			return kindCounts[0] == 0 && kindCounts[1] == 0 && kindCounts[2] >= 3;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			long flags = 0;
			for (int i = 0; i < tiles.Count; i++) flags |= (1L << tiles[i].BaseTile.SortedIndex);
			if ((flags | CheckFlags) != CheckFlags) return false;
			result.Add(YakuValue.FromFullYaku(this, "绿一色", 1));
			return true;
		}
	}
}
