using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	abstract class Tile : IComparable<Tile> {
		public static readonly Tile[] AllTiles = {
			Tiles.一万, Tiles.二万, Tiles.三万, Tiles.四万, Tiles.五万, Tiles.六万, Tiles.七万, Tiles.八万, Tiles.九万,
			Tiles.一饼, Tiles.二饼, Tiles.三饼, Tiles.四饼, Tiles.五饼, Tiles.六饼, Tiles.七饼, Tiles.八饼, Tiles.九饼,
			Tiles.一索, Tiles.二索, Tiles.三索, Tiles.四索, Tiles.五索, Tiles.六索, Tiles.七索, Tiles.八索, Tiles.九索,
			Tiles.东, Tiles.南, Tiles.西, Tiles.北, Tiles.白, Tiles.发, Tiles.中,
		};
		protected static readonly Dictionary<string, Tile> Map = new Dictionary<string, Tile>(AllTiles.Length) {
			{ "一万", Tiles.一万 },
			{ "二万", Tiles.二万 },
			{ "三万", Tiles.三万 },
			{ "四万", Tiles.四万 },
			{ "五万", Tiles.五万 },
			{ "六万", Tiles.六万 },
			{ "七万", Tiles.七万 },
			{ "八万", Tiles.八万 },
			{ "九万", Tiles.九万 },
			{ "一饼", Tiles.一饼 },
			{ "二饼", Tiles.二饼 },
			{ "三饼", Tiles.三饼 },
			{ "四饼", Tiles.四饼 },
			{ "五饼", Tiles.五饼 },
			{ "六饼", Tiles.六饼 },
			{ "七饼", Tiles.七饼 },
			{ "八饼", Tiles.八饼 },
			{ "九饼", Tiles.九饼 },
			{ "一索", Tiles.一索 },
			{ "二索", Tiles.二索 },
			{ "三索", Tiles.三索 },
			{ "四索", Tiles.四索 },
			{ "五索", Tiles.五索 },
			{ "六索", Tiles.六索 },
			{ "七索", Tiles.七索 },
			{ "八索", Tiles.八索 },
			{ "九索", Tiles.九索 },
			{ "东", Tiles.东 },
			{ "南", Tiles.南 },
			{ "西", Tiles.西 },
			{ "北", Tiles.北 },
			{ "白", Tiles.白 },
			{ "发", Tiles.发 },
			{ "中", Tiles.中 },
		};

		public abstract int SortedLevel { get; }

		public int SortedIndex { get; protected set; }

		public bool IsDora { get; set; }

		/// <summary>
		/// 幺九牌（字牌、数牌1或9）
		/// </summary>
		public bool IsTerminal {
			get {
				return this is KanjiTile || ((this as NumberTile).Number & 7) == 1;
			}
		}

		protected Tile(bool isDora) {
			this.IsDora = isDora;
		}

		public int CompareTo(Tile other) {
			//int subSortLevel = this.SortedLevel - other.SortedLevel;
			//if (subSortLevel == 0) {
			//	if (this is NumberTile) {
			//		return (this as NumberTile).Number - (other as NumberTile).Number;
			//	} else {
			//		return (int) (this as KanjiTile).Value - (int) (other as KanjiTile).Value;
			//	}
			//} else {
			//	return subSortLevel;
			//}
			return SortedIndex - other.SortedIndex;
		}

		public override bool Equals(object obj) {
			Tile other = obj as Tile;
			if (other != null)
				return this.CompareTo(other) == 0;
			else
				return false;
		}

		public override int GetHashCode() {
			return this.SortedIndex;
		}

		public static Tile Parse(string tileName) {
			return Map[tileName];
		}

		public static bool operator ==(Tile t1, Tile t2) {
			if (ReferenceEquals(t1, t2)) {
				return true;
			} else {
				return !ReferenceEquals(t1, null) && !ReferenceEquals(t2, null) && t1.CompareTo(t2) == 0;
			}
		}

		public static bool operator !=(Tile t1, Tile t2) {
			if (ReferenceEquals(t1, t2)) {
				return false;
			} else {
				return ReferenceEquals(t1, null) || ReferenceEquals(t2, null) || t1.CompareTo(t2) != 0;
			}
		}
	}
}
