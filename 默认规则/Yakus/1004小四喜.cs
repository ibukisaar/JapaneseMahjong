using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 小四喜 : Yaku {
		public override int OrderIndex => 1004;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount >= 3;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] >= 3 && kindCounts[3] >= 4;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			const KanjiTile.Kanji check = KanjiTile.Kanji.东 | KanjiTile.Kanji.南 | KanjiTile.Kanji.西 | KanjiTile.Kanji.北;
			int count = groups.PungList.Count(g => g.Key is KanjiTile t && (t.Value & check) != 0);
			if (count != 3) return false;
			if (!(groups.Pair.Key is KanjiTile t2) || (t2.Value & check) == 0) return false;
			result.Add(YakuValue.FromFullYaku(this, "小四喜", 1));
			return true;
		}
	}
}
