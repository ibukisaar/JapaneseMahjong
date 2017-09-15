using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 混一色 : Yaku {
		public override int OrderIndex => 21;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			if (kindCountsFromTiles[3] == 0) return false;
			int sum = kindCountsFromTiles[0] + kindCountsFromTiles[1] + kindCountsFromTiles[2];
			return kindCountsFromTiles[0] == sum || kindCountsFromTiles[1] == sum || kindCountsFromTiles[2] == sum;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			result.Add(YakuValue.FromFanValue("混一色", 2));
			return true;
		}
	}
}
