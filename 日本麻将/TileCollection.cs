using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public sealed class TileCollection : ITiles {
		private IBaseTiles baseTiles;
		private Tile[] tiles;

		public Tile this[int index] => tiles[index];
		public Tile[] Tiles => tiles;
		public Tile Added => tiles.Length == 14 ? tiles[13] : null;
		public IBaseTiles BaseTiles => baseTiles;
		public int Count => tiles.Length;

		public TileCollection(IEnumerable<Tile> tiles) {
			this.tiles = tiles.ToArray();
			baseTiles = new BaseTileCollection(Array.ConvertAll(this.tiles, t => t.BaseTile));
		}

		public IEnumerator<Tile> GetEnumerator() {
			for (int i = 0; i < tiles.Length; i++)
				yield return tiles[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
