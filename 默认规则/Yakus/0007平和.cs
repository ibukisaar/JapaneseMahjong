using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 平和 : Yaku {
		public override int OrderIndex => 7;

		public override YakuType Type => YakuType.听牌形式 | YakuType.门前清;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return junkoCount == 4;
		}

		protected override bool HookCalculateFu(ref int fu, IGroups groups, YakuEnvironment env) {
			fu = env.HasFlag(YakuEnvironment.自摸) ? 20 : 30;
			return true;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			KanjiTile.Kanji GetKanjis(YakuEnvironment e) {
				int slefWind = ((int) e >> 6) & 0xf;
				int fieldWind = ((int) e >> 10) & 0xf;
				return (KanjiTile.Kanji) (slefWind | fieldWind);
			}
			
			if (groups.Pair.Type == GroupType.和牌) return false;
			foreach (var g in groups.JunkoList) {
				if (g.Type == GroupType.副露) return false;
				if (g.Type == GroupType.和牌) {
					var numTile = g.Key as NumberTile;
					switch (g.AddedIndex) {
						case 0 when numTile.Number == 7: //边张
						case 1: //嵌张
						case 2 when numTile.Number == 1: //边张
							return false;
					}
				}
			}

			if (groups.Pair.Key is KanjiTile t) {
				var excludeKanji = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中 | GetKanjis(env);
				if ((t.Value & excludeKanji) != 0) return false;
			}

			result.Add(YakuValue.FromFanValue(this, "平和", 1));
			return true;
		}
	}
}
