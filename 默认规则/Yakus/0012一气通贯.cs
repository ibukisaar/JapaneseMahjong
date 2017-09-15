using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 一气通贯 : Yaku {
		public override int OrderIndex => 12;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount >= 3;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[0] >= 3 || kindCountsWithoutPair[1] >= 3 || kindCountsWithoutPair[2] >= 3;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			bool[,] flags = new bool[3, 7];
			foreach (var g in groups.JunkoList) {
				flags[g.Key.SortedLevel, (g.Key as NumberTile).Number - 1] = true;
			}
			for (int i = 0; i < 3; i++) {
				if (flags[i, 0] && flags[i, 3] && flags[i, 6]) {
					result.Add(YakuValue.FromFanValue(this, "一气通贯", (env & YakuEnvironment.门前清) != 0 ? 2 : 1));
					return true;
				}
			}
			return false;
		}
	}
}
