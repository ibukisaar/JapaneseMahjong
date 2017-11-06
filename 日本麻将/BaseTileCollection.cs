using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public class BaseTileCollection : IBaseTiles {
		private BaseTile[] baseTiles;
		private SortedTilesEnumerator sorted;

		public BaseTile this[int index] => baseTiles[index];
		public BaseTile[] BaseTiles => baseTiles;
		public SortedTilesEnumerator Sorted => sorted;
		public int Count => baseTiles.Length;

		public BaseTileCollection(IEnumerable<BaseTile> baseTiles) {
			this.baseTiles = baseTiles.ToArray();
			Array.Sort(this.baseTiles);
			this.sorted = new SortedTilesEnumerator(baseTiles);
		}

		public BaseTileCollection(BaseTile[] baseTiles) {
			this.baseTiles = baseTiles;
			this.sorted = new SortedTilesEnumerator(baseTiles);
		}

		public IEnumerator<BaseTile> GetEnumerator() {
			for (int i = 0; i < baseTiles.Length; i++) yield return baseTiles[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public BaseTile Replace(int tileIndex, BaseTile newTile) {
			var old = baseTiles[tileIndex];
			baseTiles[tileIndex] = newTile;
			sorted.Tiles[old.SortedIndex]--;
			sorted.Tiles[newTile.SortedIndex]++;
			return old;
		}
	}
}
