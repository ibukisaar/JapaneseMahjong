using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public abstract class BaseTile : IComparable<BaseTile> {
		public static readonly BaseTile[] AllTiles = {
			Tiles.一万, Tiles.二万, Tiles.三万, Tiles.四万, Tiles.五万, Tiles.六万, Tiles.七万, Tiles.八万, Tiles.九万,
			Tiles.一饼, Tiles.二饼, Tiles.三饼, Tiles.四饼, Tiles.五饼, Tiles.六饼, Tiles.七饼, Tiles.八饼, Tiles.九饼,
			Tiles.一索, Tiles.二索, Tiles.三索, Tiles.四索, Tiles.五索, Tiles.六索, Tiles.七索, Tiles.八索, Tiles.九索,
			Tiles.东, Tiles.南, Tiles.西, Tiles.北, Tiles.白, Tiles.发, Tiles.中,
		};
		public static readonly string[] LongNames = {
			"一万", "二万", "三万", "四万", "五万", "六万", "七万", "八万", "九万",
			"一饼", "二饼", "三饼", "四饼", "五饼", "六饼", "七饼", "八饼", "九饼",
			"一索", "二索", "三索", "四索", "五索", "六索", "七索", "八索", "九索",
			"东", "南", "西", "北", "白", "发", "中"
		};
		public const string ShortNames = "一二三四五六七八九①②③④⑤⑥⑦⑧⑨123456789东南西北白发中";

		protected static readonly Dictionary<string, BaseTile> MapByLongName = new Dictionary<string, BaseTile>(AllTiles.Length) {
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

			//{ "赤一万", Tiles.赤一万 },
			//{ "赤二万", Tiles.赤二万 },
			//{ "赤三万", Tiles.赤三万 },
			//{ "赤四万", Tiles.赤四万 },
			//{ "赤五万", Tiles.赤五万 },
			//{ "赤六万", Tiles.赤六万 },
			//{ "赤七万", Tiles.赤七万 },
			//{ "赤八万", Tiles.赤八万 },
			//{ "赤九万", Tiles.赤九万 },
			//{ "赤一饼", Tiles.赤一饼 },
			//{ "赤二饼", Tiles.赤二饼 },
			//{ "赤三饼", Tiles.赤三饼 },
			//{ "赤四饼", Tiles.赤四饼 },
			//{ "赤五饼", Tiles.赤五饼 },
			//{ "赤六饼", Tiles.赤六饼 },
			//{ "赤七饼", Tiles.赤七饼 },
			//{ "赤八饼", Tiles.赤八饼 },
			//{ "赤九饼", Tiles.赤九饼 },
			//{ "赤一索", Tiles.赤一索 },
			//{ "赤二索", Tiles.赤二索 },
			//{ "赤三索", Tiles.赤三索 },
			//{ "赤四索", Tiles.赤四索 },
			//{ "赤五索", Tiles.赤五索 },
			//{ "赤六索", Tiles.赤六索 },
			//{ "赤七索", Tiles.赤七索 },
			//{ "赤八索", Tiles.赤八索 },
			//{ "赤九索", Tiles.赤九索 },
			//{ "赤东", Tiles.赤东 },
			//{ "赤南", Tiles.赤南 },
			//{ "赤西", Tiles.赤西 },
			//{ "赤北", Tiles.赤北 },
			//{ "赤白", Tiles.赤白 },
			//{ "赤发", Tiles.赤发 },
			//{ "赤中", Tiles.赤中 },
		};

		public static readonly char[] Suffixs = { 'm', 'p', 's', 'z' };

		public static IEnumerable<BaseTile> ParseSuffixExpr(string expr) {
			var tempList = new List<int>();
			foreach (var c in expr) {
				int level;
				switch (c) {
					case 'm': level = 0; break;
					case 'p': level = 1; break;
					case 's': level = 2; break;
					case 'z': level = 3; break;
					default:
						if (c == '0')
							tempList.Add(4);
						else
							tempList.Add(c - '1');
						continue;
				}

				foreach (var index in tempList) {
					yield return AllTiles[level * 9 + index];
				}
				tempList.Clear();
			}
		}

		public static string ToSuffixExpr(IEnumerable<BaseTile> tiles) {
			var sb = new StringBuilder(28);
			BaseTile prev = null;
			foreach (var t in tiles) {
				if (prev != null && prev.SortedLevel != t.SortedLevel) {
					sb.Append(Suffixs[prev.SortedLevel]);
				}

				sb.Append((char) (t.SortedIndex % 9 + '1'));
				prev = t;
			}
			if (prev != null) sb.Append(Suffixs[prev.SortedLevel]);
			return sb.ToString();
		}

		public abstract int SortedLevel { get; }

		public int SortedIndex { get; protected set; }

		/// <summary>
		/// 幺九牌（字牌、数牌1或9）
		/// </summary>
		public abstract bool IsTerminal { get; }

		internal BaseTile() { }

		public int CompareTo(BaseTile other) {
			return SortedIndex - other.SortedIndex;
		}

		public static BaseTile Parse(string tileName) {
			return MapByLongName[tileName];
		}

		public override string ToString() => ShortNames[SortedIndex].ToString();

		public string LongName => LongNames[SortedIndex];
	}
}
