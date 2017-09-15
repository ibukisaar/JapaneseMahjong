using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将;

namespace 默认规则 {
	public sealed class DefaultGame : Game {
		public Tile[] AllTiles { get; }

		public override string Name => "默认规则";

		protected override int GetTileId(int baseSortedIndex, int subIndex) => baseSortedIndex * 4 + subIndex + 1;

		public override Tile GetTile(int id) => AllTiles[id - 1];

		public DefaultGame() {
			var tiles = new Tile[BaseTile.AllTiles.Length * 4];
			for (int i = 0; i < BaseTile.AllTiles.Length; i++) {
				for (int j = 0; j < 4; j++) {
					tiles[4 * i + j] = new Tile(BaseTile.AllTiles[i], GetTileId(i, j));
				}
			}
			this.AllTiles = tiles;
		}
	}
}
