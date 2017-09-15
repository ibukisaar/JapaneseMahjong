using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 清老头 : Yaku {
		public override int OrderIndex => 1008;

		public override YakuType Type => YakuType.役满;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4;
		}

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCounts[3] == 0
				&& (kindCounts[0] >= 1 && kindCounts[0] <= 2)
				&& (kindCounts[1] >= 1 && kindCounts[1] <= 2)
				&& (kindCounts[2] >= 1 && kindCounts[2] <= 2);
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if (groups.Any(g => !g.Key.IsTerminal)) return false;
			result.Add(YakuValue.FromFullYaku("清老头", 1));
			return true;
		}
	}
}
