using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	class WanTile : NumberTile {
		public WanTile(int number, bool isDora = false) : base(number, isDora) { }

		public override int SortedLevel {
			get {
				return 0;
			}
		}

		public override string ToString() {
			return NumberKanji[Number - 1] + "万";
		}
	}
}
