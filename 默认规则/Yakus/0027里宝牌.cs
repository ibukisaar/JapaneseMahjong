using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;
using 日本麻将.Yakus;

namespace 默认规则.Yakus {
	public sealed class 里宝牌 : Yaku {
		public override int OrderIndex => 27;

		public override YakuType Type => YakuType.Dora;

		protected override bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env) {
			int dora = tiles.Sum(t => t.InDora);
			if (dora == 0) return false;
			result.Add(YakuValue.FromFanValue(this, "里宝牌", 0, dora));
			return true;
		}
	}
}
