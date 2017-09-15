using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 河底捞鱼 : Yaku {
		public override int OrderIndex => 6;

		public override YakuType Type => YakuType.环境;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.海底) == 0) return false;
			if ((env & YakuEnvironment.自摸) != 0) return false;
			result.Add(YakuValue.FromFanValue(this, "河底捞鱼", 1));
			return true;
		}
	}
}
