using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 一发 : Yaku {
		public override int OrderIndex => 1;

		public override YakuType Type => YakuType.环境 | YakuType.门前清;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.一发) == 0) return false;
			result.Add(YakuValue.FromFanValue(this, "一发", 1));
			return true;
		}
	}
}
