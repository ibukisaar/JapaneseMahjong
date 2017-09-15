using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 三暗刻 : Yaku {
		public override int OrderIndex => 16;

		public override YakuType Type => YakuType.听牌形式;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount >= 3;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			int count = 0;
			foreach (var g in groups.PungList) {
				if (g.Type == GroupType.副露) continue;
				if (g.Type == GroupType.和牌 && !SelfWindEquals(g.AddedWind, env)) continue;
				count++;
			}
			if (count < 3) return false;
			result.Add(YakuValue.FromFanValue(this, "三暗刻", 2));
			return true;
		}
	}
}
