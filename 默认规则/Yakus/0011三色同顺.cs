using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 三色同顺 : Yaku {
		public override int OrderIndex => 11;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount >= 3;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[0] > 0 && kindCountsWithoutPair[1] > 0 && kindCountsWithoutPair[2] > 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			bool[,] flags = new bool[3, 7];
			foreach (var g in groups.JunkoList) {
				flags[g.Key.SortedLevel, (g.Key as NumberTile).Number - 1] = true;
			}
			for (int i = 0; i < 7; i++) {
				if (flags[0, i] && flags[1, i] && flags[2, i]) {
					result.Add(YakuValue.FromFanValue(this, "三色同顺", 2));
					return true;
				}
			}
			return false;
		}
	}
}
