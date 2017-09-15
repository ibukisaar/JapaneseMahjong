using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public sealed class SouTile : NumberTile {
		internal SouTile(int number) : base(number) { }

		public override int SortedLevel => 2;
	}
}
