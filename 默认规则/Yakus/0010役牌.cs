using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji = 日本麻将.KanjiTile.Kanji;

namespace 日本麻将.Yakus {
	public sealed class 役牌 : Yaku {
		public override int OrderIndex => 10;

		public override YakuType Type => YakuType.None;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount > 0;
		}

		protected override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] > 0;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			Kanji Get自风(YakuEnvironment e) => (Kanji) (((int) e >> 6) & 0xf);
			Kanji Get场风(YakuEnvironment e) => (Kanji) (((int) e >> 10) & 0xf);

			Kanji selfWind = Get自风(env), fieldWind = Get场风(env);
			var cmp = selfWind | fieldWind | Kanji.白 | Kanji.发 | Kanji.中;
			bool yes = false;
			foreach (var g in groups.PungList) {
				if (g.Key is KanjiTile t) {
					if ((t.Value & cmp) != 0) {
						yes = true;
						if (t.Value == selfWind) result.Add(YakuValue.FromFanValue(this, "自风" + selfWind, 1));
						else if (t.Value == fieldWind) result.Add(YakuValue.FromFanValue(this, "场风" + fieldWind, 1));
						else if (t.Value == Kanji.白) result.Add(YakuValue.FromFanValue(this, "役牌白", 1));
						else if (t.Value == Kanji.发) result.Add(YakuValue.FromFanValue(this, "役牌发", 1));
						else result.Add(YakuValue.FromFanValue(this, "役牌中", 1));
					}
				}
			}
			return yes;
		}
	}
}
