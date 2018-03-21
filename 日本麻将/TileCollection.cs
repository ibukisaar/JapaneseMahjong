using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public sealed class TileCollection : ITiles {
		private IBaseTiles baseTiles;
		private IReadOnlyList<Tile> allTiles, handTiles;

		public Tile this[int index] => allTiles[index];
		public IReadOnlyList<Tile> AllTiles => allTiles;
		public IReadOnlyList<Tile> HandTiles => handTiles;
		public Tile Added => handTiles[handTiles.Count - 1];
		public IBaseTiles BaseTiles => baseTiles;
		public int Count => allTiles.Count;

		public TileCollection(IEnumerable<Tile> tiles, IEnumerable<Group> openGroups = null) {
			handTiles = tiles.OrderBy(t => t.BaseTile).ToArray();
			if (openGroups != null) {
				var allTiles = new List<Tile>(handTiles);
				foreach (var g in openGroups) allTiles.AddRange(g.Tiles);
				allTiles.Sort();
				this.allTiles = allTiles;
			} else {
				allTiles = handTiles;
			}
			baseTiles = new BaseTileCollection(allTiles.Select(t => t.BaseTile));
		}

		public IEnumerator<Tile> GetEnumerator() {
			for (int i = 0; i < allTiles.Count; i++)
				yield return allTiles[i];
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
