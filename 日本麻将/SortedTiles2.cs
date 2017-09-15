using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	class SortedTiles2 : ISortedTiles {
		private int count = 0;
		private long flags = 0;
		private byte[] tilesCount = new byte[BaseTile.AllTiles.Length];

		public SortedTiles2() { }

		public SortedTiles2(IEnumerable<BaseTile> tiles) {
			foreach (BaseTile tile in tiles) {
				Add(tile);
			}
		}

		public int Count => count;

		public bool IsReadOnly => false;

		public void Add(BaseTile tile) {
			int index = tile.SortedIndex;
			tilesCount[index]++;
			flags |= 1L << index;
			count++;
		}

		public bool Remove(BaseTile tile) {
			int index = tile.SortedIndex;
			flags &= ~(((4L - --tilesCount[index]) >> 2) << index);
			count--;
			return true;
		}

		public IEnumerator<BaseTile> GetEnumerator() {
			long temp = flags;
			while (temp != 0) {
				int i = FastLog2(temp & -temp);
				int count = tilesCount[i];
				while (count-- > 0) {
					yield return BaseTile.AllTiles[i];
				}
				temp &= temp - 1;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Clear() {
			count = 0;
			flags = 0;
			Array.Clear(tilesCount, 0, tilesCount.Length);
		}

		public ISortedTiles Clone() {
			SortedTiles2 other = new SortedTiles2();
			other.count = count;
			other.flags = flags;
			Array.Copy(tilesCount, other.tilesCount, tilesCount.Length);
			return other;
		}

		/// <summary>
		/// 快速求2为底的对数
		/// </summary>
		/// <param name="x">必须大于0，函数内不检查</param>
		/// <returns></returns>
		unsafe private static int FastLog2(long x) {
			float f = x;
			return (*(int*) &f >> 23) - 127;
		}

		public bool DifferentNext(BaseTile firstTile, out BaseTile secondTile) {
			long temp = flags & (-2L << firstTile.SortedIndex);
			temp &= -temp;
			secondTile = temp != 0 ? BaseTile.AllTiles[FastLog2(temp)] : null;
			return !ReferenceEquals(secondTile, null);
		}

		public bool Contains(BaseTile tile) {
			return (flags & (1L << tile.SortedIndex)) != 0;
		}

		public IEnumerable<BaseTile> GetDifferentEnumerator() {
			long temp = flags;
			while (temp != 0) {
				yield return BaseTile.AllTiles[FastLog2(temp & -temp)];
				temp &= temp - 1;
			}
		}

		public int GetTileCount(BaseTile tile) {
			return tilesCount[tile.SortedIndex];
		}

		public bool HasTile(BaseTile tile) {
			long flags = 1L << tile.SortedIndex;
			return (this.flags & flags) != 0;
		}

		public bool HasAllTiles(params BaseTile[] tiles) {
			long flags = 0;
			for (int i = tiles.Length - 1; i >= 0; i--) {
				flags |= 1L << tiles[i].SortedIndex;
			}

			return (this.flags & flags) == flags;
		}

		public void CopyTo(BaseTile[] array, int arrayIndex) {
			foreach (var tile in this) {
				array[arrayIndex++] = tile;
			}
		}
	}
}
