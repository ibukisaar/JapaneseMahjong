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
	class Yaku : IComparable<Yaku> {
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
		public int HanValue { get; }

		public Yaku(Yakus yakusValue, int hanValue) {
			this.YakuValue = yakusValue;
			this.HanValue = hanValue;
		}

		/// <summary>
		/// 在和牌的状态下，检查各个役以及对应的番数
		/// </summary>
		/// <param name="tiles">tiles的个数要保证除以3余数是2，且已经是和牌的状态</param>
		/// <param name="openGroups"></param>
		/// <param name="extra"></param>
		/// <param name="env"></param>
		/// <param name="doras"></param>
		/// <param name="ganDoras"></param>
		/// <returns></returns>
		public static List<Yaku> Check(SortedList<Tile> tiles, ICollection<Mahjong.Group> openGroups, Tile extra, Environment env, IEnumerable<Tile> doras, IEnumerable<Tile> ganDoras) {
			ICollection<Yaku> result = new SortedSet<Yaku>();
			bool sevenPairs, thirteenTerminal;

			if (Check国士无双(result, tiles, extra)) {
				if (Check第一巡(result, env)) {

				}
			}

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

		private static bool Check大七星(ICollection<Yaku> result, SortedList<Tile> tiles) {
			if (tiles.Count == 14) {
				Tile pre = null;
				for (int i = 0; i < 14; i += 2) {
					if (!(tiles[i] is KanjiTile) || pre == tiles[i] || tiles[i] != tiles[i + 1]) return false;
					pre = tiles[i];
				}
				result.Add(new Yaku(Yakus.大七星, 13));
				return true;
			}

			return false;
		}

		private static bool Check字一色(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);
			foreach (Mahjong.Group group in groups) {
				if (!(group.Value is KanjiTile)) {
					return false;
				}
			}
			result.Add(new Yaku(Yakus.字一色, 13));
			return true;
		}

		private static bool Check四暗刻(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups, Tile extra) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);

			Tile pair = null;
			foreach (Mahjong.Group group in groups) {
				if (!(group as Mahjong.IPung)?.IsClosed ?? true) {
					return false;
				} else if (group is Mahjong.Pair) {
					pair = group.Value;
				}
			}

			if (pair == extra)
				result.Add(new Yaku(Yakus.四暗刻单骑, 13));
			else
				result.Add(new Yaku(Yakus.四暗刻, 13));

			return true;
		}

		private static bool Check四喜(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);

			int count = 0;
			bool 大四喜 = true;
			const KanjiTile.Kanji check = KanjiTile.Kanji.东 | KanjiTile.Kanji.南 | KanjiTile.Kanji.西 | KanjiTile.Kanji.北;

			foreach (var group in groups) {
				if (group.Value is KanjiTile) {
					KanjiTile.Kanji value = (group.Value as KanjiTile).Value;
					if ((value & check) != 0) {
						count++;
						if (group is Mahjong.Pair) {
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

		private static bool Check大三元(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);

			int count = 0;
			const KanjiTile.Kanji check = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中;

			foreach (var group in groups) {
				if (group.Value is KanjiTile) {
					KanjiTile.Kanji value = (group.Value as KanjiTile).Value;
					if ((value & check) != 0) {
						if (group is Mahjong.Pair) {
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

		private static bool Check四杠子(ICollection<Yaku> result, IEnumerable<Mahjong.Group> openGroups) {
			if (openGroups.Count(group => group is Mahjong.Gan) == 4) {
				result.Add(new Yaku(Yakus.四杠子, 13));
				return true;
			}
			return false;
		}

		static readonly HashSet<Tile> 绿一色集 = new HashSet<Tile> { new SouTile(2), new SouTile(3), new SouTile(4), new SouTile(6), new SouTile(8), new KanjiTile(KanjiTile.Kanji.发) };

		private static bool Check绿一色(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);

			// [2,3,4,6,8]索, 发
			foreach (var g in groups) {
				if (g is Mahjong.Junko) {
					if (!(g.Value is SouTile) || (g.Value as NumberTile).Number != 2) return false;
				} else if (!绿一色集.Contains(g.Value)) {
					return false;
				}
			}

			result.Add(new Yaku(Yakus.绿一色, 13));
			return true;
		}

		static readonly HashSet<Tile> 清老头集 = new HashSet<Tile> { new WanTile(1), new WanTile(9), new PinTile(1), new PinTile(9), new SouTile(1), new SouTile(9) };

		private static bool Check清老头(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, IEnumerable<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);
			foreach (var g in groups) {
				if (g is Mahjong.Junko || !清老头集.Contains(g.Value)) {
					return false;
				}
			}

			result.Add(new Yaku(Yakus.清老头, 13));
			return true;
		}

		class NumberTileComparer : IEqualityComparer<Mahjong.Group> {
			public bool Equals(Mahjong.Group x, Mahjong.Group y) {
				return x.Value.SortedLevel == y.Value.SortedLevel;
			}

			public int GetHashCode(Mahjong.Group obj) {
				return obj.Value.SortedIndex;
			}
		}

		private static SortedList<Tile> GetTiles(IEnumerable<Mahjong.Group> groups) {
			SortedList<Tile> tiles = new SortedList<Tile>();
			foreach (var g in groups.Reverse()) {
				if (g is Mahjong.Junko) {
					tiles.Add(g.Value);
					tiles.Add((g.Value as NumberTile).Next);
					tiles.Add((g.Value as NumberTile).Next.Next);
				} else {
					tiles.Add(g.Value);
					tiles.Add(g.Value);
					if (g is Mahjong.IPung) {
						tiles.Add(g.Value);
					}
				}
			}
			return tiles;
		}

		private static bool Check九莲宝灯(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, Tile extra) {
			if (closedGroups.Count() != 5 || closedGroups.Any(g => !g.IsClosed) || closedGroups.Any(g => !(g.Value is NumberTile)) || closedGroups.Distinct(new NumberTileComparer()).Count() > 1) return false;

			int[] counts = new int[9] { -3, -1, -1, -1, -1, -1, -1, -1, -3 };
			var numbers = GetTiles(closedGroups).Cast<NumberTile>().ToArray();
			foreach (var num in numbers) {
				counts[num.Number - 1]++;
			}

			int value = 0;
			foreach (int count in counts) value |= count;

			if (value == 1) {
				int num = counts.ElementAt(1) + 1;
				if ((extra as NumberTile).Number == num) {
					result.Add(new Yaku(Yakus.纯正九莲宝灯, 13));
				} else {
					result.Add(new Yaku(Yakus.九莲宝灯, 13));
				}
				return true;
			}

			return false;
		}

		private static bool Check国士无双(ICollection<Yaku> result, SortedList<Tile> tiles, Tile extra) {
			if (tiles.Count == 14) {
				int terminalCount = 1;
				Tile curr = tiles[0];
				do {
					if (!curr.IsTerminal) return false;
					terminalCount++;
				} while (tiles.DifferentNext(curr, out curr));

				if (terminalCount == 13) {
					if (tiles.Count(t => t == extra) == 2) {
						result.Add(new Yaku(Yakus.国士无双十三面, 13));
					} else {
						result.Add(new Yaku(Yakus.国士无双, 13));
					}
					return true;
				}
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

		private static bool Check七对子(ICollection<Yaku> result, SortedList<Tile> tiles) {
			//if (Mahjong.FastCheckRon七对子(tiles)) {
			//	result.Add(new Yaku(Yakus.七对子, 2));
			//	return true;
			//}
			return false;
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

		private static bool Check平和(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, ICollection<Mahjong.Group> openGroups, Tile extra, Environment env) {
			if (!(extra is NumberTile) || (openGroups != null && openGroups.Count > 0) || closedGroups.Any(g => g is Mahjong.IPung)) return false;
			// if (closedGroups.Any(g => !(g is Mahjong.Pair) && (g as Mahjong.Junko).IndexOf(extra) == 1))
			List<Mahjong.Junko> junkos = new List<Mahjong.Junko>();
			Mahjong.Pair pair = null;

			foreach (var g in closedGroups) {
				if (g is Mahjong.Junko) {
					junkos.Add(g as Mahjong.Junko);
				} else {
					pair = g as Mahjong.Pair;
				}
			}

			if (pair.Value is KanjiTile) {
				KanjiTile.Kanji yakuTile = KanjiTile.Kanji.白 | KanjiTile.Kanji.发 | KanjiTile.Kanji.中 | GetKanjis(env);
				if (((pair.Value as KanjiTile).Value & yakuTile) != 0) return false;
			}

			NumberTile num = extra as NumberTile;
			if (num.Number == 3) {
				if (junkos.Any(j => j.IndexOf(extra) == 0)) {
					result.Add(new Yaku(Yakus.平和, 1));
					return true;
				}
			} else if (num.Number == 7) {
				if (junkos.Any(j => j.IndexOf(extra) == 2)) {
					result.Add(new Yaku(Yakus.平和, 1));
					return true;
				}
			} else {
				if (junkos.Any(j => j.IndexOf(extra) == 0 || j.IndexOf(extra) == 2)) {
					result.Add(new Yaku(Yakus.平和, 1));
					return true;
				}
			}

			return false;
		}

		private static bool Check断幺九(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, ICollection<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);
			if (groups.All(g => g is Mahjong.Junko ? (g.Value as NumberTile).Number >= 2 && (g.Value as NumberTile).Number <= 6 : !g.Value.IsTerminal)) {
				result.Add(new Yaku(Yakus.断幺九, 1));
				return true;
			}
			return false;
		}

		private static bool Check对对和(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, ICollection<Mahjong.Group> openGroups) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);
			if (groups.Count(g => g is Mahjong.IPung) == 4) {
				result.Add(new Yaku(Yakus.对对和, 2));
				return true;
			}
			return false;
		}

		private static bool Check三色同顺(ICollection<Yaku> result, IEnumerable<Mahjong.Group> closedGroups, ICollection<Mahjong.Group> openGroups, Environment env) {
			var groups = openGroups == null ? closedGroups : closedGroups.Concat(openGroups);

			List<Mahjong.Junko> wan = new List<Mahjong.Junko>(),
				pin = new List<Mahjong.Junko>(),
				sou = new List<Mahjong.Junko>();

			foreach (var g in groups) {
				if (g is Mahjong.Junko) {
					(g.Value is WanTile ? wan : g.Value is PinTile ? pin : sou).Add(g as Mahjong.Junko);
				}
			}

			if (wan.Count + pin.Count + sou.Count < 3) return false;

			foreach (var j in wan) {
				// if ()
			}

			return false;
		}
	}
}
