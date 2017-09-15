using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 四杠子 : Yaku {
		public override int OrderIndex => 1010;

		public override YakuType Type => YakuType.役满;

		protected override bool FilterTest(int junkoCount, int pungCount) {
			return pungCount == 4;
		}

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if (groups.PungList.Any(g => !(g is Gan))) return false;
			result.Add(YakuValue.FromFullYaku(this, "四杠子", 1));
			return true;
		}
	}
}
