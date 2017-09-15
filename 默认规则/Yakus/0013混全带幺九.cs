using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 混全带幺九 : Yaku {
		public override int OrderIndex => 13;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount > 0; // 混全带幺九必须有顺子，否则就是混老头
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCounts[0] + kindCounts[1] + kindCounts[2] > 0 && kindCounts[3] > 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (groups.Any(g => !g.IsTerminal)) return false;
			result.Add(YakuValue.FromFanValue(this, "混全带幺九", (env & YakuEnvironment.门前清) != 0 ? 2 : 1));
			return true;
		}
	}
}
