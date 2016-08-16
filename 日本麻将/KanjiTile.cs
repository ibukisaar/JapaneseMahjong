using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	class KanjiTile : Tile {
		public enum Kanji : uint {
			东 = 1 << 0,
			南 = 1 << 1,
			西 = 1 << 2,
			北 = 1 << 3,
			白 = 1 << 4,
			发 = 1 << 5,
			中 = 1 << 6
		}

		private readonly static int[] IndexesMap = { 7, 0, 1, 7, 2, 4, 7, 7, 3, 6, 5 };

		public Kanji Value { get; }

		public bool IsWind {
			get {
				return (Value & (Kanji.东 | Kanji.南 | Kanji.西 | Kanji.北)) != 0;
			}
		}

		public bool IsDragon {
			get {
				return (Value & (Kanji.白 | Kanji.发 | Kanji.中)) != 0;
			}
		}

		public KanjiTile(Kanji value, bool isDora = false) : base(isDora) {
			Value = value;
			SortedIndex = 27 + IndexesMap[(int) Value % 11];
		}

		public override int SortedLevel {
			get {
				return 3;
			}
		}

		public override string ToString() {
			return Value.ToString();
		}
	}
}
