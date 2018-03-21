using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则.Yakus {
	public sealed class 宝牌 : Yaku {
		public override int OrderIndex => 25;

		public override YakuType Type => YakuType.Dora;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			int dora = tiles.Sum(t => t.Dora);
			if (dora == 0) return false;
			result.Add(YakuValue.FromFanValue(this, "宝牌", 0, dora));
			return true;
		}
	}
}
