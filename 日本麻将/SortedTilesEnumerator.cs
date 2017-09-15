using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public struct SortedTilesEnumerator : IEnumerable<BaseTile> {
		public int[] Tiles;
		public int Count;
		public int Index;

		public SortedTilesEnumerator(IEnumerable<BaseTile> tiles) {
			Tiles = new int[34];
			Count = 0;
			Index = 0;
			foreach (var tile in tiles) {
				Tiles[tile.SortedIndex]++;
				Count++;
			}
		}

		public void MoveIndex() {
			while (Index < 34 && Tiles[Index] == 0) Index++;
		}

		public bool PerfectMatch(params BaseTile[] tiles) {
			long bitmap1 = 0, bitmap2 = 0;
			foreach (var t in tiles) {
				bitmap1 |= 1L << t.SortedIndex;
			}
			for (int i = 0; i < 34; i++) {
				if (Tiles[i] == 0) continue;
				bitmap2 |= 1L << i;
			}
			return bitmap1 == bitmap2;
		}

		public IEnumerator<BaseTile> GetEnumerator() {
			for (int i = 0; i < 34; i++) {
				for (int count = 0; count < Tiles[i]; count++) {
					yield return BaseTile.AllTiles[i];
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public SortedTilesEnumerator Clone() {
			return new SortedTilesEnumerator {
				Tiles = Tiles.Clone() as int[],
				Count = Count,
				Index = Index
			};
		}

		public SortedTilesEnumerator CloneAndReset() {
			return new SortedTilesEnumerator {
				Tiles = Tiles.Clone() as int[],
				Count = Count,
				Index = 0
			};
		}

		public ref int Current => ref Tiles[Index];

		public ref int Next => ref Tiles[Index + 1];

		public ref int NextNext => ref Tiles[Index + 2];
	}
}
