using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class 立直 : Yaku {
		public override int OrderIndex => 0;

		public override YakuType Type => YakuType.环境;

		protected internal override bool Test(ICollection<YakuValue> result, Tile[] tiles, IGroups groups, YakuEnvironment env) {
			if ((env & YakuEnvironment.立直) != 0) { result.Add(YakuValue.FromFanValue("立直", 1)); return true; }
			if ((env & YakuEnvironment.双立直) != 0) { result.Add(YakuValue.FromFanValue("W立直", 2)); return true; }
			return false;
		}
	}
}
