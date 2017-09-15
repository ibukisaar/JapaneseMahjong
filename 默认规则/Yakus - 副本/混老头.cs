using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 混老头 : Yaku {
		public override int OrderIndex => 19;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4 || junkoCount + pungCount == 0; // 也可以是七对子
		}

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsFromTiles[0] + kindCountsFromTiles[1] + kindCountsFromTiles[2] > 0 && kindCountsFromTiles[3] > 0;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if (tiles.Any(t => !t.BaseTile.IsTerminal)) return false;
			result.Add(YakuValue.FromFanValue("混老头", 2));
			return true;
		}
	}
}
