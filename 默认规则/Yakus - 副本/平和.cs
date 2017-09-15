using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 平和 : Yaku {
		public override int OrderIndex => 7;

		public override YakuType Type => YakuType.None;

		protected internal override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount == 4;
		}

		private static KanjiTile.Kanji GetKanjis(YakuEnvironment env) {
			int slefWind = ((int) env >> 6) & 0xf;
			int fieldWind = ((int) env >> 10) & 0xf;
			return (KanjiTile.Kanji) (slefWind | fieldWind);
		}

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.门前清) == 0) return false;
			if (groups.Pair.Type == GroupType.和牌) return false;
			foreach (var g in groups.JunkoList) {
				if (g.Type == GroupType.副露) return false;
				if (g.Type == GroupType.和牌) {
					if (g.AddedIndex == 1) return false; //嵌张
					if ((g.Key as NumberTile).Number == 1 && g.AddedIndex == 2) return false; //边张
					if ((g.Key as NumberTile).Number == 7 && g.AddedIndex == 0) return false; //边张
				}
			}

			if (groups.Pair.Key is KanjiTile t) {
				var excludeKanji = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中 | GetKanjis(env);
				if ((t.Value & excludeKanji) != 0) return false;
			}

			result.Add(YakuValue.FromFanValue("平和", 1));
			return true;
		}
	}
}
