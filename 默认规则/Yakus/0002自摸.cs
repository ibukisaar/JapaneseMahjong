using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 自摸 : Yaku {
		public override int OrderIndex => 2;

		public override YakuType Type => YakuType.环境 | YakuType.门前清;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			const YakuEnvironment cmp = YakuEnvironment.自摸;
			if ((env & cmp) != cmp) return false;
			result.Add(YakuValue.FromFanValue(this, "门前清自摸和", 1));
			return true;
		}
	}
}
