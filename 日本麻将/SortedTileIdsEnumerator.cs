using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	struct SortedTileIdsEnumerator : IEnumerable<int> {
		public uint[] Tiles;
		public int Count;
		public int Index;

		public SortedTileIdsEnumerator(IEnumerable<Tile> tiles) {
			Tiles = new uint[34];
			Count = 0;
			Index = 0;
			foreach (var t in tiles) {
				ref uint ids = ref Tiles[t.BaseTile.SortedIndex];
				ids = (ids << 8) | (uint) t.Id;
				Count++;
			}
		}

		public IEnumerator<int> GetEnumerator() {
			for (int i = 0; i < 34; i++) {
				for (uint ids = Tiles[i]; ids != 0; ids >>= 8) {
					yield return (int) (ids & 0xff);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void MoveIndex() {
			while (Tiles[Index] == 0) Index++;
		}

		public ref uint Current => ref Tiles[Index];

		public ref uint Next => ref Tiles[Index + 1];

		public ref uint NextNext => ref Tiles[Index + 2];
	}
}
