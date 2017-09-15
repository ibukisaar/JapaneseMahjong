using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 抢杠 : Yaku {
		public override int OrderIndex => 4;

		public override YakuType Type => YakuType.环境;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.抢杠) == 0) return false;
			result.Add(YakuValue.FromFanValue(this, "抢杠", 1));
			return true;
		}
	}
}
