using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 场风 : Yaku {
		public override int OrderIndex => throw new NotImplementedException();

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount > 0;
		}

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] > 0;
		}

		private static KanjiTile.Kanji Get场风(YakuEnvironment env) {
			return (KanjiTile.Kanji) (((int) env >> 10) & 0xf);
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			var fieldWind = Get场风(env);
			if (groups.PungList.Any(g => g.Key is KanjiTile t && t.Value == fieldWind)) {
				result.Add(YakuValue.FromFanValue("场风" + fieldWind, 1));
				return true;
			}
			return false;
		}
	}
}
