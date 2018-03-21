using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 三色同刻 : Yaku {
		public override int OrderIndex => 18;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount >= 3;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[0] > 0 && kindCountsWithoutPair[1] > 0 && kindCountsWithoutPair[2] > 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			int[] counts = new int[9];
			foreach (var g in groups.PungList) {
				if (g.Key is NumberTile t) {
					counts[t.Number - 1]++;
				}
			}
			for (int i = 0; i < 9; i++) {
				if (counts[i] == 3) {
					result.Add(YakuValue.FromFanValue(this, "三色同刻", 2));
					return true;
				}
			}
			return false;
		}
	}
}
