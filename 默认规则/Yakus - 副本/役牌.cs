using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 役牌 : Yaku {
		public override int OrderIndex => 10;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount > 0;
		}

		protected internal override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return kindCountsWithoutPair[3] > 0;
		}

		private static KanjiTile.Kanji Get自风(YakuEnvironment env) {
			return (KanjiTile.Kanji) (((int) env >> 6) & 0xf);
		}

		private static KanjiTile.Kanji Get场风(YakuEnvironment env) {
			return (KanjiTile.Kanji) (((int) env >> 10) & 0xf);
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			KanjiTile.Kanji selfWind = Get自风(env), fieldWind = Get场风(env);
			var cmp = selfWind | fieldWind | KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中;
			bool yes = false;
			foreach (var g in groups.PungList) {
				if (g.Key is KanjiTile t) {
					if ((t.Value & cmp) != 0) {
						yes = true;
						if (t.Value == selfWind) result.Add(YakuValue.FromFanValue("自风" + selfWind, 1));
						else if (t.Value == fieldWind) result.Add(YakuValue.FromFanValue("场风" + fieldWind, 1));
						else if (t.Value == KanjiTile.Kanji.白) result.Add(YakuValue.FromFanValue("役牌白", 1));
						else if (t.Value == KanjiTile.Kanji.发) result.Add(YakuValue.FromFanValue("役牌发", 1));
						else result.Add(YakuValue.FromFanValue("役牌中", 1));
					}
				}
			}
			return yes;
		}
	}
}
