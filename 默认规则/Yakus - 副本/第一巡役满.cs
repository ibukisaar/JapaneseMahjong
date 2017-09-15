using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 第一巡役满 : Yaku {
		public override int OrderIndex => 1000;

		public override YakuType Type => YakuType.役满 | YakuType.环境;

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.第一巡) == 0) return false;
			if ((env & YakuEnvironment.自摸) != 0) {
				if ((env & YakuEnvironment.自风东) != 0) {
					result.Add(YakuValue.FromFullYaku("天和", 1));
				} else {
					result.Add(YakuValue.FromFullYaku("地和", 1));
				}
			} else {
				result.Add(YakuValue.FromFullYaku("人和", 1));
			}
			return true;
		}
	}
}
