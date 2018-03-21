using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 断幺九 : Yaku {
		public override int OrderIndex => 8;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsFromTiles[3] == 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (tiles.Any(t => t.BaseTile.IsTerminal)) return false;

			result.Add(YakuValue.FromFanValue(this, "断幺九", 1));
			return true;
		}
	}
}
