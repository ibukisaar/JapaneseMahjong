using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	abstract class NumberTile : Tile {
		protected const string NumberKanji = "一二三四五六七八九";

		public int Number { get; }

		public NumberTile Next {
			get {
				return Number <= 8 ? Tile.AllTiles[SortedIndex + 1] as NumberTile : null;
			}
		}

		protected NumberTile(int number, bool isDora) : base(isDora) {
			if (number < 1 || number > 9)
				throw new ArgumentOutOfRangeException("number必须是1~9");

			Number = number;
			SortedIndex = SortedLevel * 9 + Number - 1;
		}
	}
}
