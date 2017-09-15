using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public sealed class PinTile : NumberTile {
		internal PinTile(int number) : base(number) { }

		public override int SortedLevel => 1;
	}
}
