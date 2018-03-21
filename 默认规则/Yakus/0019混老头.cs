using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 混老头 : Yaku {
		public override int OrderIndex => 19;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4 || junkoCount + pungCount == 0; // 也可以是七对子
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsFromTiles[0] + kindCountsFromTiles[1] + kindCountsFromTiles[2] > 0 && kindCountsFromTiles[3] > 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (tiles.Any(t => !t.BaseTile.IsTerminal)) return false;
			result.Add(YakuValue.FromFanValue(this, "混老头", 2));
			return true;
		}
	}
}
