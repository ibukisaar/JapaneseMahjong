using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public abstract class NumberTile : BaseTile {
		public int Number { get; }

		public NumberTile Next => Number <= 8 ? BaseTile.AllTiles[SortedIndex + 1] as NumberTile : null;

		public override bool IsTerminal => (Number & 7) == 1;

		internal NumberTile(int number) {
			if (number < 1 || number > 9)
				throw new ArgumentOutOfRangeException("number必须是1~9");

			Number = number;
			SortedIndex = SortedLevel * 9 + Number - 1;
		}
	}
}
