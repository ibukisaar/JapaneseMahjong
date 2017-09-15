using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public enum Yakus : int {
		// 一番
		立直, // 门前清
		一发, // 门前清
		门前清自摸和, // 门前清
		断幺九,
		平和, // 门前清
		一杯口, // 门前清
		自风东,
		自风南,
		自风西,
		自风北,
		场风东,
		场风南,
		场风西,
		场风北,
		役牌白,
		役牌发,
		役牌中,
		海底捞月,
		河底摸鱼,
		岭上开花,
		抢杠,

		// 二番
		三色同顺, // 非门前清一番
		一气通贯, // 非门前清一番
		对对和,
		三暗刻,
		七对子, // 门前清
		混全带幺九, // 非门前清一番
		混老头,
		三色同刻,
		三杠子,
		小三元,
		双立直, // 门前清

		// 三番
		混一色, // 非门前清二番
		纯全带幺九, // 非门前清二番
		二杯口, // 门前清

		// 五翻以上
		荒牌满贯,
		清一色, // 六翻，非门前清五翻

		// 役满
		天和, // 门前清
		地和, // 门前清
		人和, // 门前清
		大三元,
		四暗刻, // 门前清
		国士无双, // 门前清
		字一色,
		小四喜,
		大四喜,
		绿一色,
		清老头,
		九莲宝灯, // 门前清
		四杠子,

		// 特殊役满
		四暗刻单骑, // 门前清
		国士无双十三面, // 门前清
		纯正九莲宝灯, // 门前清
		大七星, // 门前清

		// 宝牌
		宝牌,
		里宝牌,
		赤宝牌,
	}

	/// <summary>
	/// 役
	/// </summary>
	public class Yaku : IComparable<Yaku> {
		public enum Environment : uint {
			无 = 0,
			立直 = 1 << 0,
			自摸 = 1 << 1,
			一发 = 1 << 2,
			双立直 = 1 << 3,
			抢杠 = 1 << 4,
			/// <summary>
			/// 天和、地和、人和的可能性
			/// </summary>
			第一巡 = 1 << 5,
			自风东 = 1 << 6,
			自风南 = 1 << 7,
			自风西 = 1 << 8,
			自风北 = 1 << 9,
			场风东 = 1 << 10,
			场风南 = 1 << 11,
			场风西 = 1 << 12,
			场风北 = 1 << 13,
			/// <summary>
			/// 海底或河底
			/// </summary>
			海底 = 1 << 14,
			岭上开花 = 1 << 15,
			门前清 = 1 << 16
		}

		public Yakus YakuValue { get; }
		public int MainValue { get; }
		public int AddedValue { get; }

		public Yaku(Yakus yakusValue, int value, bool isMain = true) {
			this.YakuValue = yakusValue;
			if (isMain) MainValue = value; else AddedValue = value;
		}

		public int CompareTo(Yaku other) {
			return this.YakuValue - other.YakuValue;
		}

		private static KanjiTile.Kanji GetKanjis(Environment env) {
			KanjiTile.Kanji kanji = 0;
			if ((env & (Environment.场风东 | Environment.自风东)) != 0) kanji |= KanjiTile.Kanji.东;
			if ((env & (Environment.场风南 | Environment.自风南)) != 0) kanji |= KanjiTile.Kanji.南;
			if ((env & (Environment.场风西 | Environment.自风西)) != 0) kanji |= KanjiTile.Kanji.西;
			if ((env & (Environment.场风北 | Environment.自风北)) != 0) kanji |= KanjiTile.Kanji.北;
			return kanji;
		}

		// 天和 地和 人和
		//
		//
		/// <summary>
		/// 在和牌的状态下，检查各个役以及对应的番数
		/// </summary>
		/// <param name="tiles">tiles的个数要保证除以3余数是2，且已经是和牌的状态</param>
		/// <param name="openGroups"></param>
		/// <param name="env"></param>
		/// <param name="doras"></param>
		/// <param name="ganDoras"></param>
		/// <returns></returns>
		public static List<Yaku> Check(IEnumerable<Tile> tiles, ICollection<Group> openGroups, Environment env, IEnumerable<BaseTile> doras, IEnumerable<BaseTile> ganDoras) {
			ICollection<Yaku> result = new SortedSet<Yaku>();
			bool sevenPairs, thirteenTerminal;

			//if (Check国士无双(result, tiles, extra)) {
			//	if (Check第一巡(result, env)) {

			//	}
			//}

			//foreach (var closedGroups in Mahjong.Ron(tiles)) {

			//	Check第一巡(result, env);
			//	if (Check国士无双(result, tiles, extra) || Check九莲宝灯(result, openGroups, extra) || Check大七星(result, tiles)) goto 役满结束;
			//	Check四杠子(result, openGroups);
			//	Check四暗刻(result, openGroups, extra);
			//	Check四喜(result, openGroups);
			//	Check字一色(result, openGroups);
			//	Check大三元(result, openGroups);
			//	Check清老头(result, openGroups);
			//	Check绿一色(result, openGroups);
			//}
			//役满结束:


			return null;
		}

		private static bool Check门前清(IEnumerable<Group> groups) => groups.All(g => g.Type != GroupType.副露);

		private static bool Check第一巡(ICollection<Yaku> result, Environment env) {
			if ((env & Environment.第一巡) != 0) {
				if ((env & Environment.自摸) != 0) {
					if ((env & Environment.自风东) != 0) {
						result.Add(new Yaku(Yakus.天和, 13));
					} else {
						result.Add(new Yaku(Yakus.地和, 13));
					}
				} else {
					result.Add(new Yaku(Yakus.人和, 13));
				}
				return true;
			}
			return false;
		}

		private static bool Check大七星(ICollection<Yaku> result, SortedTilesEnumerator tiles) {
			if (tiles.Count == 14) {
				for (int i = Tiles.东.SortedIndex; i <= Tiles.中.SortedIndex; i++) {
					if (tiles.Tiles[i] != 2) return false;
				}
				result.Add(new Yaku(Yakus.大七星, 13));
				return true;
			}

			return false;
		}

		private static bool Check字一色(ICollection<Yaku> result, IEnumerable<Group> groups) {
			if (groups.Any(g => !(g.Key is KanjiTile))) return false;
			result.Add(new Yaku(Yakus.字一色, 13));
			return true;
		}

		private static bool Check四暗刻(ICollection<Yaku> result, IEnumerable<Group> groups, Wind self) {
			Group pair = null;
			foreach (var group in groups) {
				if (group.Type == GroupType.副露 || !(group.IsPung || group is Pair)) return false;
				if (group.Type == GroupType.和牌 && group is Pung && group.AddedWind != self) return false;
				if (group is Pair) pair = group;
			}

			if (pair.Type == GroupType.和牌)
				result.Add(new Yaku(Yakus.四暗刻单骑, 13));
			else
				result.Add(new Yaku(Yakus.四暗刻, 13));

			return true;
		}

		private static bool Check四喜(ICollection<Yaku> result, IEnumerable<Group> groups) {
			int count = 0;
			bool 大四喜 = true;
			const KanjiTile.Kanji check = KanjiTile.Kanji.东 | KanjiTile.Kanji.南 | KanjiTile.Kanji.西 | KanjiTile.Kanji.北;

			foreach (var group in groups) {
				if (group.Key is KanjiTile) {
					KanjiTile.Kanji value = (group.Key as KanjiTile).Value;
					if ((value & check) != 0) {
						count++;
						if (group is Pair) {
							大四喜 = false;
						}
					}
				}
			}

			if (count == 4) {
				result.Add(大四喜 ? new Yaku(Yakus.大四喜, 13) : new Yaku(Yakus.小四喜, 13));
				return true;
			}

			return false;
		}

		private static bool Check大三元(ICollection<Yaku> result, IEnumerable<Group> groups) {
			int count = 0;
			const KanjiTile.Kanji check = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中;

			foreach (var group in groups) {
				if (group.Key is KanjiTile) {
					KanjiTile.Kanji value = (group.Key as KanjiTile).Value;
					if ((value & check) != 0) {
						if (group is Pair) {
							return false;
						}
						count++;
					}
				}
			}

			if (count == 3) {
				result.Add(new Yaku(Yakus.大三元, 13));
				return true;
			}

			return false;
		}

		private static bool Check四杠子(ICollection<Yaku> result, IEnumerable<Group> groups) {
			if (groups.Count(g => g is Gan) == 4) {
				result.Add(new Yaku(Yakus.四杠子, 13));
				return true;
			}
			return false;
		}

		static readonly HashSet<BaseTile> 绿一色集 = new HashSet<BaseTile> { Tiles.二索, Tiles.三索, Tiles.四索, Tiles.六索, Tiles.八索, Tiles.发 };

		private static bool Check绿一色(ICollection<Yaku> result, IEnumerable<Group> groups) {
			// [2,3,4,6,8]索, 发
			foreach (var g in groups) {
				if (g is Junko) {
					if (!(g.Key is SouTile t) || t.Number != 2) return false;
				} else if (!绿一色集.Contains(g.Key)) {
					return false;
				}
			}

			result.Add(new Yaku(Yakus.绿一色, 13));
			return true;
		}

		static readonly HashSet<BaseTile> 清老头集 = new HashSet<BaseTile> { Tiles.一万, Tiles.九万, Tiles.一索, Tiles.九索, Tiles.一饼, Tiles.九饼 };

		private static bool Check清老头(ICollection<Yaku> result, IEnumerable<Group> groups) {
			foreach (var g in groups) {
				if (g is Junko || !清老头集.Contains(g.Key)) {
					return false;
				}
			}

			result.Add(new Yaku(Yakus.清老头, 13));
			return true;
		}

		class NumberTileComparer : IEqualityComparer<Group> {
			public bool Equals(Group x, Group y) {
				return x.Key == y.Key;
			}

			public int GetHashCode(Group obj) {
				return obj.Key.GetHashCode();
			}
		}

		private static IEnumerable<Tile> GetTiles(IEnumerable<Group> groups) {
			foreach (var g in groups) {
				foreach (var t in g.Tiles) {
					yield return t;
				}
			}
		}

		private static bool Check九莲宝灯(ICollection<Yaku> result, IEnumerable<Group> groups) {
			Tile extra = null;
			foreach (var g in groups) {
				if (g is Gan) return false;
				if (g.Type == GroupType.副露) return false;
				if (!(g.Key is NumberTile)) return false;
				if (g.Type == GroupType.和牌) extra = g.Tiles[g.AddedIndex];
			}
			if (groups.Any(g => g.Key.SortedLevel != extra.BaseTile.SortedLevel)) return false;

			int[] counts = new int[9] { -3, -1, -1, -1, -1, -1, -1, -1, -3 };
			var numbers = GetTiles(groups).Cast<NumberTile>().ToArray();
			foreach (var num in numbers) {
				counts[num.Number - 1]++;
			}

			int value = 0;
			foreach (int count in counts) value |= count;

			if (value == 1) {
				int num = counts.ElementAt(1) + 1;
				if ((extra.BaseTile as NumberTile).Number == num) {
					result.Add(new Yaku(Yakus.纯正九莲宝灯, 13));
				} else {
					result.Add(new Yaku(Yakus.九莲宝灯, 13));
				}
				return true;
			}

			return false;
		}

		private static bool Check国士无双(ICollection<Yaku> result, SortedTilesEnumerator tiles, BaseTile extra) {
			if (tiles.Count == 14) {
				bool isMatch = tiles.PerfectMatch(
					Tiles.一万,
					Tiles.九万,
					Tiles.一饼,
					Tiles.九饼,
					Tiles.一索,
					Tiles.九索,
					Tiles.东,
					Tiles.南,
					Tiles.西,
					Tiles.北,
					Tiles.白,
					Tiles.发,
					Tiles.中
				);
				if (!isMatch) return false;

				if (tiles.Tiles[extra.SortedIndex] == 2) {
					result.Add(new Yaku(Yakus.国士无双十三面, 13));
				} else {
					result.Add(new Yaku(Yakus.国士无双, 13));
				}
				return true;
			}

			return false;
		}

		private static bool CheckEnvironment(ICollection<Yaku> result, Environment env) {
			int count = result.Count;
			if ((env & Environment.立直) != 0) result.Add(new Yaku(Yakus.立直, 1));
			if ((env & Environment.双立直) != 0) result.Add(new Yaku(Yakus.双立直, 2));
			if ((~env & (Environment.自摸 | Environment.门前清)) == 0) result.Add(new Yaku(Yakus.门前清自摸和, 1));
			if ((env & Environment.一发) != 0) result.Add(new Yaku(Yakus.一发, 1));
			if ((env & Environment.岭上开花) != 0) result.Add(new Yaku(Yakus.岭上开花, 1));
			if ((env & Environment.抢杠) != 0) result.Add(new Yaku(Yakus.抢杠, 1));
			if ((env & Environment.海底) != 0)
				if ((env & Environment.自摸) != 0)
					result.Add(new Yaku(Yakus.海底捞月, 1));
				else
					result.Add(new Yaku(Yakus.河底摸鱼, 1));
			return count != result.Count;
		}

		private static bool Check七对子(ICollection<Yaku> result, SortedTilesEnumerator tiles) {
			if (Mahjong.FastCheckRon七对子(tiles)) {
				result.Add(new Yaku(Yakus.七对子, 2));
				return true;
			}
			return false;
		}

		private static bool Check平和(ICollection<Yaku> result, IEnumerable<Group> groups, Environment env) {
			Group pair = null;
			foreach (var g in groups) {
				if (g.Type == GroupType.副露 || g.IsPung) return false;
				if (g.Type == GroupType.和牌) {
					if (g is Pair) return false;
					if (g.AddedIndex == 1) return false; //嵌张
					if ((g.Key as NumberTile).Number == 1 && g.AddedIndex == 2) return false; //边张
					if ((g.Key as NumberTile).Number == 7 && g.AddedIndex == 0) return false; //边张
				}
				if (g is Pair) pair = g;
			}

			if (pair.Key is KanjiTile) {
				KanjiTile.Kanji yakuTile = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中 | GetKanjis(env);
				if (((pair.Key as KanjiTile).Value & yakuTile) != 0) return false;
			}

			result.Add(new Yaku(Yakus.平和, 1));
			return false;
		}

		private static bool Check断幺九(ICollection<Yaku> result, IEnumerable<Group> groups) {
			if (groups.All(g => g is Junko ? (g.Key as NumberTile).Number >= 2 && (g.Key as NumberTile).Number <= 6 : !g.Key.IsTerminal)) {
				result.Add(new Yaku(Yakus.断幺九, 1));
				return true;
			}
			return false;
		}

		private static bool Check对对和(ICollection<Yaku> result, IEnumerable<Group> groups) {
			if (groups.Count(g => g.IsPung) == 4) {
				result.Add(new Yaku(Yakus.对对和, 2));
				return true;
			}
			return false;
		}

		private static bool Check三色同顺(ICollection<Yaku> result, IEnumerable<Group> groups) {
			bool[,] flags = new bool[3, 7];
			int hanValue = 2;

			foreach (var g in groups) {
				if (g is Junko) {
					flags[g.Key.SortedLevel, g.Key.SortedIndex % 9] = true;
				}
				if (g.Type == GroupType.副露) hanValue = 1;
			}

			for (int i = 0; i < 7; i++) {
				if (flags[0, i] && flags[1, i] && flags[2, i]) {
					result.Add(new Yaku(Yakus.三色同顺, hanValue));
					return true;
				}
			}

			return false;
		}
	}
}
