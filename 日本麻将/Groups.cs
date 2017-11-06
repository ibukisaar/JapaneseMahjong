using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	// 一般和牌形式可能的情况：
	//      雀头   顺子   刻子   暗杠  大明杠  加杠 
	// 门清  √      √      √      √
	// 副露         √      √            √      √
	// 和牌  √      √      √
	/// <summary>
	/// 表示雀头、顺子、刻子、杠子。并包含来源信息。
	/// </summary>
	public abstract class Group : IComparable<Group> {
		/// <summary>
		/// 排序用键值
		/// </summary>
		public BaseTile Key { get; protected set; }
		public Tile[] Tiles { get; protected set; }
		public GroupType Type { get; protected set; }
		/// <summary>
		/// 是否属于刻子
		/// </summary>
		public virtual bool IsPung => false;
		/// <summary>
		/// 附加牌的位置，副露、荣和或自摸
		/// </summary>
		public int AddedIndex { get; protected set; }
		/// <summary>
		/// 副露或荣和来源，可以是自己
		/// </summary>
		public Wind AddedWind => Type != GroupType.门清 ? Tiles[AddedIndex].Owner : Wind.None;
		public abstract int SortedLevel { get; }
		public abstract Group Clone();
		public abstract Group Remodeling(Tile[] tiles, GroupType type, int addedIndex);
		public virtual bool IsTerminal => Key.IsTerminal;
		public virtual bool IsImportant => true;

		internal Group() { }

		public int CompareTo(Group other) {
			if (Type != other.Type) {
				if (Type == GroupType.副露) return -1;
				if (other.Type == GroupType.副露) return 1;
			}

			int cmp = Key.CompareTo(other.Key);
			if (cmp != 0) return cmp;
			cmp = SortedLevel - other.SortedLevel;
			if (cmp != 0) return cmp;
			cmp = Type - other.Type;
			if (cmp != 0) return cmp;
			if (Type == GroupType.门清) return 0;
			cmp = AddedIndex - other.AddedIndex;
			if (cmp != 0) return cmp;
			if (this is Gan gan1 && other is Gan gan2) {
				return gan1.Added == gan2.Added ? 0 : gan1.Added ? -1 : 1;
			}
			return cmp;
		}

		public static int CalcIndex(Wind self, Wind other) {
			if (self == Wind.None || other == Wind.None) throw new ArgumentException($"值不能为{nameof(Wind)}.{Wind.None}");
			return self < other ? (self - other) + 4 : self - other;
		}

		protected void SetFieldsFor副露(Tile extra, Wind self, bool isMinGan = false) {
			var tempList = new List<Tile>(this.Tiles);
			if (self == Wind.None) throw new ArgumentException("副露必须指定自风", nameof(self));
			if (self == extra.Owner) throw new InvalidOperationException($"{nameof(self)}不能等于{nameof(extra)}.{nameof(Tile.Owner)}");
			if (!tempList.Remove(extra)) throw new ArgumentException($"{nameof(Tiles)}必须包含{nameof(extra)}", nameof(extra));

			int index = CalcIndex(self, extra.Owner) - 1;
			if (isMinGan && index == 2) index = 3;
			this.AddedIndex = index;
			tempList.Insert(index, extra);
			this.Tiles = tempList.ToArray();
		}

		protected void SetFieldsFor和牌(Tile extra) {
			int index = Array.IndexOf(this.Tiles, extra);
			if (index < 0) throw new ArgumentException($"{nameof(this.Tiles)}必须包含{nameof(extra)}", nameof(extra));
			this.AddedIndex = index;
		}

		public override string ToString() {
			var sb = new StringBuilder();
			for (int i = 0; i < Tiles.Length; i++) {
				sb.Append(Tiles[i].BaseTile);
				if (Type == GroupType.副露 && i == AddedIndex) sb.Append('*');
				else if (Type == GroupType.和牌 && i == AddedIndex) sb.Append('!');
				if (i != Tiles.Length - 1) {
					sb.Append(' ');
				}
			}
			return sb.ToString();
		}
	}

	/// <summary>
	/// 顺子
	/// </summary>
	public sealed class Junko : Group {
		public override int SortedLevel => 2;
		public override bool IsTerminal => Key is NumberTile t && (t.Number == 1 || t.Number == 7);

		private Junko() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tiles">必须按顺子顺序</param>
		/// <param name="type"></param>
		/// <param name="extra"></param>
		/// <param name="self"></param>
		public Junko(Tile[] tiles, GroupType type = GroupType.门清, Tile extra = null, Wind self = Wind.None) {
			this.Tiles = tiles;
			Array.Sort(this.Tiles);
			this.Key = this.Tiles[0].BaseTile;
			
			this.Type = type;
			if (type == GroupType.门清) return;

			if (type == GroupType.副露) {
				SetFieldsFor副露(extra, self);
			} else {
				SetFieldsFor和牌(extra);
			}
		}

		public override Group Clone()
			=> new Junko() { Key = Key, Tiles = Tiles.Clone() as Tile[], Type = Type, AddedIndex = AddedIndex };

		public override Group Remodeling(Tile[] tiles, GroupType type, int addedIndex) {
			return new Junko() {
				Key = tiles.Min(t => t.BaseTile),
				Tiles = tiles.Clone() as Tile[],
				Type = type,
				AddedIndex = addedIndex
			};
		}
	}

	/// <summary>
	/// 刻子
	/// </summary>
	public sealed class Pung : Group {
		public override int SortedLevel => 1;

		private Pung() { }

		public override bool IsPung => true;

		public Pung(Tile[] tiles, GroupType type = GroupType.门清, Tile extra = null, Wind self = Wind.None) {
			this.Key = tiles[0].BaseTile;
			this.Tiles = tiles;
			this.Type = type;
			if (type == GroupType.门清) return;

			if (type == GroupType.副露) {
				SetFieldsFor副露(extra, self);
			} else {
				SetFieldsFor和牌(extra);
			}
		}

		public override Group Clone()
			=> new Pung() { Key = Key, Tiles = Tiles.Clone() as Tile[], Type = Type, AddedIndex = AddedIndex };

		public override Group Remodeling(Tile[] tiles, GroupType type, int addedIndex) {
			return new Pung() {
				Key = tiles.Min(t => t.BaseTile),
				Tiles = tiles.Clone() as Tile[],
				Type = type,
				AddedIndex = addedIndex
			};
		}
	}

	/// <summary>
	/// 雀头
	/// </summary>
	public sealed class Pair : Group {
		public override int SortedLevel => 0;

		private Pair() { }

		public Pair(Tile[] tiles, GroupType type = GroupType.门清, Tile extra = null) {
			if (type == GroupType.副露) throw new ArgumentException("雀头不能副露", nameof(type));

			this.Key = tiles[0].BaseTile;
			this.Tiles = tiles;
			this.Type = type;

			if (type == GroupType.和牌) {
				SetFieldsFor和牌(extra);
			}
		}

		public override Group Clone()
			=> new Pair() { Key = Key, Tiles = Tiles.Clone() as Tile[], Type = Type, AddedIndex = AddedIndex };

		public override Group Remodeling(Tile[] tiles, GroupType type, int addedIndex) {
			return new Pair() {
				Key = tiles.Min(t => t.BaseTile),
				Tiles = tiles.Clone() as Tile[],
				Type = type,
				AddedIndex = addedIndex
			};
		}
	}

	/// <summary>
	/// 杠
	/// </summary>
	public sealed class Gan : Group {
		public override int SortedLevel => 3;
		public override bool IsPung => true;
		public bool Added { get; private set; } = false;

		private Gan() { }

		/// <summary>
		/// 暗杠或大明杠
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="type"></param>
		/// <param name="extra"></param>
		/// <param name="self"></param>
		public Gan(Tile[] tiles, GroupType type = GroupType.门清, Tile extra = null, Wind self = Wind.None) {
			if (type == GroupType.和牌) throw new ArgumentException("杠子无法导致和牌", nameof(type));

			this.Key = tiles[0].BaseTile;
			this.Tiles = tiles;
			this.Type = type;

			if (type == GroupType.副露) {
				SetFieldsFor副露(extra, self, true); // 大明杠
			}
		}

		/// <summary>
		/// 加杠
		/// </summary>
		/// <param name="pung"></param>
		/// <param name="addedTile"></param>
		public Gan(Pung pung, Tile addedTile) {
			if (pung.Type != GroupType.副露) throw new ArgumentException("副露的刻子才能加杠", nameof(pung));
			if (pung.Key != addedTile.BaseTile) throw new ArgumentException("加杠牌和明刻不一致", nameof(addedTile));

			this.Key = pung.Key;
			this.Tiles = new List<Tile>(pung.Tiles) { addedTile }.ToArray();
			this.Type = GroupType.副露;
			this.AddedIndex = pung.AddedIndex;
			this.Added = true;
		}

		public override Group Clone()
			=> new Gan() { Key = Key, Tiles = Tiles.Clone() as Tile[], Type = Type, AddedIndex = AddedIndex, Added = Added };

		public override Group Remodeling(Tile[] tiles, GroupType type, int addedIndex) {
			throw new NotImplementedException();
		}
	}

	public sealed class Pull : Group {
		public override int SortedLevel => 4;

		public override bool IsPung => false;

		public override bool IsImportant => false;

		public override bool IsTerminal => false;

		public Pull(Tile tile) {
			Key = tile.BaseTile;
			Tiles = new[] { tile };
		}

		public override Group Clone() => this;

		public override Group Remodeling(Tile[] tiles, GroupType type, int addedIndex) => this;
	}

	internal struct SortedGroupSet : IEnumerable<Group>, IComparable<SortedGroupSet> {
		public Group[] Groups;

		public SortedGroupSet(IEnumerable<Group> groups) {
			Groups = groups.ToArray();
			Array.Sort(Groups);
		}

		public int CompareTo(SortedGroupSet other) {
			int cmp = Groups.Length - other.Groups.Length;
			if (cmp != 0) return cmp;

			for (int i = 0; i < Groups.Length; i++) {
				cmp = Groups[i].CompareTo(other.Groups[i]);
				if (cmp != 0) return cmp;
			}
			return 0;
		}

		public override bool Equals(object obj) {
			return obj is SortedGroupSet other && this.CompareTo(other) == 0;
		}

		public override int GetHashCode() {
			int hash = 0;
			foreach (var g in Groups) {
				foreach (var t in g.Tiles) {
					hash ^= t.BaseTile.GetHashCode();
				}
			}
			return hash;
		}

		public IEnumerator<Group> GetEnumerator() {
			foreach (var g in Groups) yield return g;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
