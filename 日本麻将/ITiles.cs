using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public interface ITiles : IReadOnlyList<Tile> {
		/// <summary>
		/// 所有牌，包含Groups。
		/// </summary>
		IReadOnlyList<Tile> AllTiles { get; }
		/// <summary>
		/// 只有未副露的手牌。
		/// </summary>
		IReadOnlyList<Tile> HandTiles { get; }
		/// <summary>
		/// 荣和或自摸的牌。
		/// </summary>
		Tile Added { get; }
		IBaseTiles BaseTiles { get; }
	}
}
