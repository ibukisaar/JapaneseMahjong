using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public class 岭上开花 : Yaku {
		public override int OrderIndex => 3;

		public override YakuType Type => YakuType.环境;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.岭上开花) == 0) return false;
			result.Add(YakuValue.FromFanValue(this, "岭上开花", 1));
			return true;
		}
	}
}
