using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public interface ITiles : IReadOnlyList<Tile> {
		Tile[] Tiles { get; }
		/// <summary>
		/// 荣和或自摸的牌。
		/// </summary>
		Tile Added { get; }
		IBaseTiles BaseTiles { get; }
	}
}
