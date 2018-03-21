using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 纯全带幺九 : Yaku {
		public override int OrderIndex => 22;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount > 0; // 纯全带幺九必须有一个顺子，否则就是清老头
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCounts[3] == 0; // 不能有字牌
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (groups.Any(g => !g.IsTerminal)) return false;

			result.Add(YakuValue.FromFanValue(this, "纯全带幺九", (env & YakuEnvironment.门前清) != 0 ? 3 : 2));
			return true;
		}
	}
}
