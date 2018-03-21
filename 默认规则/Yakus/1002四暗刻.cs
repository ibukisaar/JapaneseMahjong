using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 四暗刻 : Yaku {
		public override int OrderIndex => 1002;

		public override YakuType Type => YakuType.役满 | YakuType.门前清;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			foreach (var g in groups.PungList) {
				if (g.Type == GroupType.副露) return false;
				if (g.Type == GroupType.和牌 && !SelfWindEquals(g.AddedWind, env)) return false;
			}

			if (groups.Pair.Type == GroupType.和牌) {
				result.Add(YakuValue.FromFullYaku(this, "四暗刻单骑", 1));
			} else {
				result.Add(YakuValue.FromFullYaku(this, "四暗刻", 1));
			}
			return true;
		}
	}
}
