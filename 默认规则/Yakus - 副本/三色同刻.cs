using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 三色同刻 : Yaku {
		public override int OrderIndex => 18;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount >= 3;
		}

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[0] > 0 && kindCountsWithoutPair[1] > 0 && kindCountsWithoutPair[2] > 0;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			int[] counts = new int[9];
			foreach (var g in groups.PungList) {
				if (g.Key is NumberTile t) {
					counts[t.Number]++;
				}
			}
			for (int i = 0; i < 9; i++) {
				if (counts[i] == 3) {
					result.Add(YakuValue.FromFanValue("三色同刻", 2));
					return true;
				}
			}
			return false;
		}
	}
}
