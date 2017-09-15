using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public class ScoreSystem : IComparer<Score> {
		/// <summary>
		/// 计算基本点。
		/// </summary>
		/// <param name="score"></param>
		/// <returns></returns>
		public virtual int GetBasePoint(Score score) {
			if (score.FullYaku > 0) {
				return 8000 * score.FullYaku;
			}

			int allFanValue = score.AllFanValue;
			if (allFanValue <= 4) {
				return Math.Min(score.Fu * (1 << allFanValue + 2), 2000);
			} else if (allFanValue == 5) {
				return 2000;
			} else if (allFanValue >= 6 && allFanValue <= 7) {
				return 3000;
			} else if (allFanValue >= 8 && allFanValue <= 10) {
				return 4000;
			} else if (allFanValue >= 11 && allFanValue <= 12) {
				return 6000;
			} else {
				return 8000;
			}
		}

		/// <summary>
		/// 基本点乘以<paramref name="multiple"/>，并按百进位。
		/// </summary>
		/// <param name="basePoint"></param>
		/// <param name="multiple"></param>
		/// <returns></returns>
		public virtual int GetPoint(int basePoint, int multiple) {
			int point = basePoint * multiple;
			return (point + 99) / 100 * 100;
		}

		public int Compare(Score x, Score y) {
			int cmp = GetBasePoint(x) - GetBasePoint(y);
			if (cmp != 0) return cmp;
			cmp = x.AllFanValue - y.AllFanValue;
			if (cmp != 0) return cmp;
			cmp = x.Fu - y.Fu;
			return cmp;
		}
	}
}
