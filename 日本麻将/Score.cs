using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public sealed class Score {
		public int Fu { get; }
		public int FanValue { get; }
		public int FullYaku { get; }
		public int DoraValue { get; }
		public int AllFanValue => FanValue + DoraValue;
		public YakuValue[] YakuValues { get; }

		public Score(int fu, IEnumerable<YakuValue> yakuValues) {
			Fu = fu;
			YakuValues = yakuValues.OrderBy(y => y.Source).ToArray();
			FanValue = YakuValues.Sum(y => y.FanValue);
			FullYaku = YakuValues.Sum(y => y.FullYaku);
			DoraValue = YakuValues.Sum(y => y.DoraValue);
		}

		public override string ToString() {
			if (FullYaku > 0) {
				return $"{FullYaku}倍役满";
			} else {
				return $"{Fu}符 {AllFanValue}翻";
			}
		}
	}
}
