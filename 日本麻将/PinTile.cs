using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	class PinTile : NumberTile {
		public PinTile(int number, bool isDora = false) : base(number, isDora) { }

		public override int SortedLevel {
			get {
				return 1;
			}
		}

		public override string ToString() {
			return NumberKanji[Number - 1] + "饼";
		}
	}
}
