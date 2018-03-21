using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 大四喜 : Yaku {
		public override int OrderIndex => 1005;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] == 4;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			const KanjiTile.Kanji check = KanjiTile.Kanji.东 | KanjiTile.Kanji.南 | KanjiTile.Kanji.西 | KanjiTile.Kanji.北;
			if (groups.PungList.Any(g => !(g.Key is KanjiTile t) || (t.Value & check) == 0)) return false;
			result.Add(YakuValue.FromFullYaku(this, "大四喜", 1));
			return true;
		}
	}
}
