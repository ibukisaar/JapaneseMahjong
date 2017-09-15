using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	/// <summary>
	/// 牌的基础信息，并包含ID、赤宝牌、宝牌、里宝牌、牌归属方、牌状态信息。
	/// </summary>
	public class Tile : IComparable<Tile> {
		public BaseTile BaseTile { get; }
		public int Id { get; }
		public bool IsRedDora { get; set; } = false;
		public Wind Owner { get; set; } = Wind.None;
		public TileState State { get; set; } = TileState.牌山;
		public int Dora { get; set; }
		public int InDora { get; set; }

		public Tile(BaseTile baseTile, int id) {
			this.BaseTile = baseTile;
			this.Id = id;
		}

		public int CompareTo(Tile other) {
			return Id - other.Id;
		}

		public override string ToString()
			=> $"{BaseTile}[{Id}]";
	}
}
