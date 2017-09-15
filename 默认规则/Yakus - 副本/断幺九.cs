using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 断幺九 : Yaku {
		public override int OrderIndex => 8;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsFromTiles[3] == 0;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if (tiles.Any(t => t.BaseTile.IsTerminal)) return false;

			result.Add(YakuValue.FromFanValue("断幺九", 1));
			return true;
		}
	}
}
