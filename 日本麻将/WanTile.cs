using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public sealed class WanTile : NumberTile {
		internal WanTile(int number) : base(number) { }

		public override int SortedLevel => 0;
	}
}
