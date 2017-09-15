using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 七对子 : SpecialYaku {
		public override int OrderIndex => 14;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int[] kindCountsFromTiles) {
			return kindCountsFromTiles[0] % 2 + kindCountsFromTiles[1] % 2 + kindCountsFromTiles[2] % 2 + kindCountsFromTiles[3] % 2 == 0;
		}

		protected override bool HookCalculateFu(ref int fu, IGroups groups, YakuEnvironment env) {
			fu = 25;
			return true;
		}

		protected override void Syanten(IBaseTiles tiles, ref int result) {
			int c1 = 0, c2 = 0;
			for (int i = 0; i < 34; i++) {
				if (tiles.Sorted.Tiles[i] >= 1) c1++; else continue;
				if (tiles.Sorted.Tiles[i] >= 2) c2++;
			}

			int num = 6 - c2;
			if (c1 < 7) num += 7 - c1;
			if (num < result) result = num;
		}

		protected override bool TestRon(ITiles tiles, YakuEnvironment env) {
			return tiles.BaseTiles.Sorted.Tiles.All(tc => tc == 0 || tc == 2);
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (!TestRon(tiles, env)) return false;
			result.Add(YakuValue.FromFanValue(this, "七对子", 2));
			return true;
		}
	}
}
