using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 海底捞月 : Yaku {
		public override int OrderIndex => 5;

		public override YakuType Type => YakuType.环境;

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			const YakuEnvironment cmp = YakuEnvironment.自摸 | YakuEnvironment.海底;
			if ((env & cmp) != cmp) return false;
			result.Add(YakuValue.FromFanValue("海底摸月", 1));
			return true;
		}
	}
}
