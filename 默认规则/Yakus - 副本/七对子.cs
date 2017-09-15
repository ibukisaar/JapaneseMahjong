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

		protected internal override int Syanten(SortedTilesEnumerator tiles) {
			if (tiles.Count == 13) {
				int c1 = 0, c2 = 0;
				for (int i = 0; i < 34; i++) {
					if (tiles.Tiles[i] >= 1) c1++; else continue;
					if (tiles.Tiles[i] >= 2) c2++;
				}

				int num = 6 - c2;
				if (c1 < 7) num += 7 - c1;
				return num;
			}
			return 6;
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			var enumer = new SortedTilesEnumerator(tiles.Select(t => t.BaseTile));
			if (enumer.Tiles.Any(tc => tc != 0 && tc != 2)) return false;
			result.Add(YakuValue.FromFanValue("七对子", 2));
			return true;
		}
	}
}
