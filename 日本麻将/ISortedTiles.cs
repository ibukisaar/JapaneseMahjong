using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	/// <summary>
	/// 已排序的牌的集合
	/// </summary>
	interface ISortedTiles : ICollection<BaseTile> {
		bool HasTile(BaseTile tile);
		/// <summary>
		/// 是否包含指定的所有牌
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		bool HasAllTiles(params BaseTile[] tiles);
		/// <summary>
		/// 获得与 <paramref name="firstTile"/> 不同的下一张牌
		/// </summary>
		/// <param name="firstTile"></param>
		/// <param name="secondTile"></param>
		/// <returns></returns>
		bool DifferentNext(BaseTile firstTile, out BaseTile secondTile);
		ISortedTiles Clone();
		/// <summary>
		/// 获得一个牌种枚举器
		/// </summary>
		/// <returns></returns>
		IEnumerable<BaseTile> GetDifferentEnumerator();
		/// <summary>
		/// 获得指定牌的数量
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		int GetTileCount(BaseTile tile);
	}
}
