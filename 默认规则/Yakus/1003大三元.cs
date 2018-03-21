using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 大三元 : Yaku {
		public override int OrderIndex => 1003;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount >= 3;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] >= 3;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			const KanjiTile.Kanji check = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中;
			int count = groups.PungList.Count(g => g.Key is KanjiTile t && (t.Value & check) != 0);
			if (count < 3) return false;

			result.Add(YakuValue.FromFullYaku(this, "大三元", 1));
			return true;
		}
	}
}
