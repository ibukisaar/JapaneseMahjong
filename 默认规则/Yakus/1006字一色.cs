using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 字一色 : Yaku {
		public override int OrderIndex => 1006;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4 || junkoCount + pungCount == 0; // 或者大七星
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsFromTiles[0] + kindCountsFromTiles[1] + kindCountsFromTiles[2] == 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			result.Add(YakuValue.FromFullYaku(this, "字一色", 1));
			return true;
		}
	}
}
