using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 自摸 : Yaku {
		public override int OrderIndex => 2;

		public override YakuType Type => YakuType.环境;

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			const YakuEnvironment cmp = YakuEnvironment.门前清 | YakuEnvironment.自摸;
			if ((env & cmp) != cmp) return false;
			result.Add(YakuValue.FromFanValue("门前清自摸和", 1));
			return true;
		}
	}
}
