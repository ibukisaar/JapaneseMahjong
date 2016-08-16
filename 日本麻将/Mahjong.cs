using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	static class Mahjong {
		readonly static Tile[] RandomTiles = new Tile[Tile.AllTiles.Length * 4];
		readonly static Random random = new Random();

		static Mahjong() {
			for (int i = 0; i < 4; i++)
				Array.Copy(Tile.AllTiles, 0, RandomTiles, i * Tile.AllTiles.Length, Tile.AllTiles.Length);
		}


		public interface IGroup : IComparable<IGroup> {
			Tile Value { get; set; }
			bool IsClosed { get; set; }
		}

		public interface IPung : IGroup {
		}

		public abstract class Group : IGroup {
			public Tile Value { get; set; }
			/// <summary>
			/// 副露为false，否则为true
			/// </summary>
			public bool IsClosed { get; set; } = true;

			public int CompareTo(IGroup other) {
				return this.Value.CompareTo(other.Value);
			}
		}

		/// <summary>
		/// 顺子
		/// </summary>
		public class Junko : Group {
			public override string ToString() {
				NumberTile t1 = Value as NumberTile;
				NumberTile t2 = t1.Next;
				NumberTile t3 = t2.Next;
				return $"{t1}, {t2}, {t3}";
			}

			public int IndexOf(Tile tile) {
				if (tile is NumberTile) {
					int sub = (tile as NumberTile).Number - (this.Value as NumberTile).Number;
					return sub >= 0 && sub <= 2 ? sub : -1;
				}
				return -1;
			}
		}

		/// <summary>
		/// 刻子
		/// </summary>
		public class Pung : Group, IPung {
			public override string ToString() {
				return $"{Value}, {Value}, {Value}";
			}
		}

		/// <summary>
		/// 雀头
		/// </summary>
		public class Pair : Group {
			public override string ToString() {
				return $"{Value}, {Value}";
			}
		}

		/// <summary>
		/// 杠
		/// </summary>
		public class Gan : Group, IPung {
			public override string ToString() {
				return $"{Value}, {Value}, {Value}, {Value}";
			}
		}

		public enum Wind {
			东, 南, 西, 北
		}

		/// <summary>
		/// 获得所有和牌组合
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static List<IGroup[]> Analysis(IEnumerable<Tile> tiles) {
			SortedTiles list = new SortedTiles(tiles);
			List<IGroup[]> result = new List<IGroup[]>();
			if (list.Count % 3 == 2) {
				Analysis(list, new Stack<IGroup>(), false, result);
			}
			return result;
		}

		private static void Analysis(ISortedTiles tiles, Stack<IGroup> groups, bool hasPair, List<IGroup[]> result) {
			if (tiles.Count == 0) {
				result.Add(groups.ToArray());
				return;
			}

			Tile t1, t2, t3;
			IEnumerator<Tile> itor = tiles.GetEnumerator();
			t1 = itor.Next();
			t2 = itor.Next();
			if (t1 == t2) {
				if (!hasPair) {
					// 检查雀头
					tiles.Remove(t1);
					tiles.Remove(t2);
					groups.Push(new Pair() { Value = t1 });
					Analysis(tiles, groups, true, result);
					groups.Pop();
					tiles.Add(t2);
					tiles.Add(t1);
				}

				if (tiles.Count >= 3) {
					// 检查刻子
					t3 = itor.Next();
					if (t2 == t3) {
						tiles.Remove(t1);
						tiles.Remove(t2);
						tiles.Remove(t3);
						groups.Push(new Pung() { Value = t1 });
						Analysis(tiles, groups, hasPair, result);
						groups.Pop();
						tiles.Add(t3);
						tiles.Add(t2);
						tiles.Add(t1);
					}
				}
			}

			// 检查顺子
			if (tiles.Count >= 3) {
				t2 = (t1 as NumberTile)?.Next;
				t3 = (t2 as NumberTile)?.Next;
				if (t3 != null && tiles.HasAllTiles(t2, t3)) {
					tiles.Remove(t1);
					tiles.Remove(t2);
					tiles.Remove(t3);
					groups.Push(new Junko() { Value = t1 });
					Analysis(tiles, groups, hasPair, result);
					groups.Pop();
					tiles.Add(t3);
					tiles.Add(t2);
					tiles.Add(t1);
				}
			}
		}

		/// <summary>
		/// 快速检查是否和牌
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static bool FastCheckRon(IEnumerable<Tile> tiles) {
			ISortedTiles list = new SortedTiles(tiles);
			if (list.Count % 3 == 2) {
				return FastCheckRon(list);
			}
			return false;
		}

		/// <summary>
		/// 获得所有正在听的牌
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static List<Tile> AllReadyHand(IEnumerable<Tile> tiles) {
			ISortedTiles list = new SortedTiles(tiles);
			List<Tile> result = new List<Tile>();
			if (list.Count % 3 == 1) {
				foreach (Tile tile in Tile.AllTiles) {
					var list2 = list.Clone();
					list2.Add(tile);
					if (FastCheckRon(list2)) {
						result.Add(tile);
					}
				}
			}
			return result;
		}

		private static bool FastCheckRon(ISortedTiles tiles) {
			return FastCheckRon七对子(tiles) || FastCheckRon国士无双(tiles) || FastCheckRon(tiles, false);
		}

		private static bool FastCheckRon(ISortedTiles tiles, bool hasPair) {
			if (tiles.Count == 0) return true;

			Tile t1, t2, t3;
			IEnumerator<Tile> itor = tiles.GetEnumerator();
			t1 = itor.Next();
			t2 = itor.Next();
			if (t1 == t2) {
				if (!hasPair) {
					// 检查雀头
					tiles.Remove(t1);
					tiles.Remove(t2);
					if (FastCheckRon(tiles, true)) return true;
					tiles.Add(t2);
					tiles.Add(t1);
				}

				if (tiles.Count >= 3) {
					// 检查刻子
					t3 = itor.Next();
					if (t2 == t3) {
						tiles.Remove(t1);
						tiles.Remove(t2);
						tiles.Remove(t3);
						if (FastCheckRon(tiles, hasPair)) return true;
						tiles.Add(t3);
						tiles.Add(t2);
						tiles.Add(t1);
					}
				}
			}
			itor.Dispose();

			// 检查顺子
			if (tiles.Count >= 3) {
				t2 = (t1 as NumberTile)?.Next;
				t3 = (t2 as NumberTile)?.Next;
				if (t3 != null && tiles.HasAllTiles(t2, t3)) {
					tiles.Remove(t1);
					tiles.Remove(t2);
					tiles.Remove(t3);
					if (FastCheckRon(tiles, hasPair)) return true;
					tiles.Add(t3);
					tiles.Add(t2);
					tiles.Add(t1);
				}
			}

			return false;
		}

		public static bool FastCheckRon七对子(ISortedTiles tiles) {
			return tiles.Count == 14
				&& tiles.GetDifferentEnumerator().All(tile => tiles.GetTileCount(tile) == 2);
		}

		private static bool FastCheckRon国士无双(ISortedTiles tiles) {
			return tiles.Count == 14 && tiles.HasAllTiles(
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
		}

		public static Tile[] CreateRandomTiles() {
			RandomTiles.Random(RandomTiles.Length);
			return RandomTiles;
		}

		public static Tile[] CreateRandomTiles(int count) {
			Tile[] output = new Tile[count];
			RandomTiles.Random(count);
			Array.Copy(RandomTiles, output, count);
			return output;
		}

		public static void Random<T>(this T[] array, int count) {
			int length = array.Length;
			for (int i = 0; i < count; i++) {
				int j = random.Next(i, length);
				T t = array[i];
				array[i] = array[j];
				array[j] = t;
			}
		}

		/// <summary>
		/// 求最小向听数
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static int ListenNumber(IEnumerable<Tile> tiles) {
			int best = 6;
			var list = new SortedTiles(tiles);
			ListenNumber七对子(list, ref best);
			ListenNumber国士无双(list, ref best);
			ListenNumber(list, list.Count / 3, ref best);
			return best;
		}

		public static int ListenNumber2(IEnumerable<Tile> tiles) {
			int best = 6;
			var list = new SortedTiles(tiles);
			ListenNumber七对子(list, ref best);
			ListenNumber国士无双(list, ref best);
			ListenNumber2(list, list.Count / 3, ref best);
			return best;
		}

		/// <summary>
		/// 求向听数
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="argsTuple">bytes[0]:(牌数-1)/3。bytes[1]:面子3数量。bytes[2]:面子2数量。bytes[3]:组成雀头的牌数(0~2)</param>
		/// <param name="best"></param>
		unsafe private static void ListenNumber(ISortedTiles tiles, int argsTuple, ref int best) {
			int N = ((byte*) &argsTuple)[0];
			int C3 = ((byte*) &argsTuple)[1];
			int C2 = ((byte*) &argsTuple)[2];
			int P = ((byte*) &argsTuple)[3];

			if (C3 + C2 > N || best == 0) return;

			if (P != 0) {
				if (tiles.Count == 0) {
					int num = ((N - C3) * 2) - C2 - P + 1;

					if (num < best) {
						best = num;
					}
					return;
				}
			} else if (tiles.Count == 0) {
				return;
			}

			Tile t1, t2, t3;
			using (var itor = tiles.GetEnumerator()) {
				t1 = itor.Next();
				if (tiles.Count > 1) {
					t2 = itor.Next();
					if (t1 == t2) {
						if (P == 0) {
							// P = 2
							tiles.Remove(t1);
							tiles.Remove(t2);
							ListenNumber(tiles, (argsTuple & 0x00FFFFFF) | 0x02000000, ref best);
							tiles.Add(t2);
							tiles.Add(t1);
						}

						if (tiles.Count >= 3 && t2 == (t3 = itor.Next())) {
							// C3 = C3 + 1
							tiles.Remove(t1);
							tiles.Remove(t2);
							tiles.Remove(t3);
							ListenNumber(tiles, argsTuple + 0x0100, ref best);
							tiles.Add(t3);
							tiles.Add(t2);
							tiles.Add(t1);
						}

						// C2 = C2 + 1
						tiles.Remove(t1);
						tiles.Remove(t2);
						ListenNumber(tiles, argsTuple + 0x010000, ref best);
						tiles.Add(t2);
						tiles.Add(t1);
					}

					NumberTile t = t1 as NumberTile;
					if (t != null) {
						if (t.Number <= 8) {
							if (tiles.DifferentNext(t1, out t2) && t.Next == t2) {
								t3 = (t2 as NumberTile).Next;
								if (t3 != null && tiles.Contains(t3)) {
									// C3 = C3 + 1
									tiles.Remove(t1);
									tiles.Remove(t2);
									tiles.Remove(t3);
									ListenNumber(tiles, argsTuple + 0x0100, ref best);
									tiles.Add(t3);
									tiles.Add(t2);
									tiles.Add(t1);
								}

								// C2 = C2 + 1
								tiles.Remove(t1);
								tiles.Remove(t2);
								ListenNumber(tiles, argsTuple + 0x010000, ref best);
								tiles.Add(t2);
								tiles.Add(t1);
							}

							if (t.Number <= 7 && t.Next.Next == t2) {
								// C2 = C2 + 1
								tiles.Remove(t1);
								tiles.Remove(t2);
								ListenNumber(tiles, argsTuple + 0x010000, ref best);
								tiles.Add(t2);
								tiles.Add(t1);
							}
						}
					}
				}
			}

			if (P == 0) {
				// P = 1
				tiles.Remove(t1);
				ListenNumber(tiles, (argsTuple & 0x00FFFFFF) | 0x01000000, ref best);
				tiles.Add(t1);
			}

			// 面子1，因为只需要参数N、C3、C2、P，所以不需要进行任何操作
			tiles.Remove(t1);
			ListenNumber(tiles, argsTuple, ref best);
			tiles.Add(t1);
		}

		unsafe private static void ListenNumber2(ISortedTiles tiles, int argsTuple, ref int best) {
			int N = ((byte*) &argsTuple)[0];
			int C3 = ((byte*) &argsTuple)[1];
			int C2 = ((byte*) &argsTuple)[2];
			int P = ((byte*) &argsTuple)[3];

			if (C3 + C2 > N || best == 0) return;

			if (P != 0) {
				if (tiles.Count == 0) {
					int num = ((N - C3) * 2) - C2 - P + 1;

					if (num < best) {
						best = num;
					}
					return;
				}
			} else if (tiles.Count == 0) {
				return;
			}

			Tile t1, t2, t3;
			using (var itor = tiles.GetEnumerator()) {
				t1 = itor.Next();
				if (tiles.Count > 1) {
					t2 = itor.Next();
					if (t1 == t2) {
						if (P == 0) {
							// 检查完整雀头
							tiles.Remove(t1);
							tiles.Remove(t2);
							ListenNumber2(tiles, (argsTuple & 0x00FFFFFF) | 0x02000000, ref best);
							tiles.Add(t2);
							tiles.Add(t1);
						}

						if (tiles.Count >= 3 && t2 == (t3 = itor.Next())) {
							// 检查完整刻子
							tiles.Remove(t1);
							tiles.Remove(t2);
							tiles.Remove(t3);
							ListenNumber2(tiles, argsTuple + 0x0100, ref best);
							tiles.Add(t3);
							tiles.Add(t2);
							tiles.Add(t1);
						}

						// 检查缺一刻子
						tiles.Remove(t1);
						tiles.Remove(t2);
						ListenNumber2(tiles, argsTuple + 0x010000, ref best);
						tiles.Add(t2);
						tiles.Add(t1);
					}

					NumberTile t = t1 as NumberTile;
					if (t != null) {
						if (t.Number <= 8) {
							if (tiles.DifferentNext(t1, out t2) && t.Next == t2) {
								t3 = (t2 as NumberTile).Next;
								if (t3 != null && tiles.Contains(t3)) {
									// 检查完整顺子
									tiles.Remove(t1);
									tiles.Remove(t2);
									tiles.Remove(t3);
									ListenNumber2(tiles, argsTuple + 0x0100, ref best);
									tiles.Add(t3);
									tiles.Add(t2);
									tiles.Add(t1);
								}

								// 检查缺一顺子
								tiles.Remove(t1);
								tiles.Remove(t2);
								ListenNumber2(tiles, argsTuple + 0x010000, ref best);
								tiles.Add(t2);
								tiles.Add(t1);
							}

							if (t.Number <= 7 && t.Next.Next == t2) {
								// 检查缺一顺子
								tiles.Remove(t1);
								tiles.Remove(t2);
								ListenNumber2(tiles, argsTuple + 0x010000, ref best);
								tiles.Add(t2);
								tiles.Add(t1);
							}
						}
					}
				}
			}

			if (P == 0) {
				// 检查缺一雀头
				tiles.Remove(t1);
				ListenNumber2(tiles, (argsTuple & 0x00FFFFFF) | 0x01000000, ref best);
				tiles.Add(t1);
			}

			// 检查缺二刻子或顺子
			tiles.Remove(t1);
			ListenNumber2(tiles, argsTuple, ref best);
			tiles.Add(t1);
		}

		unsafe private static void ListenNumber七对子(ISortedTiles tiles, ref int best) {
			if (tiles.Count == 13) {
				/* counter[0] : 缺失1张牌
				 * counter[1] : 完整
				 * counter[2] : 多出1张牌
				 * counter[3] : 多出2张牌
				 */
				byte* counter = stackalloc byte[4];
				foreach (var tile in tiles.GetDifferentEnumerator()) {
					int count = tiles.GetTileCount(tile);
					counter[count - 1]++;
				}

				int over = counter[2] + counter[3] * 2;
				int num = counter[0] < over ? over : (counter[0] + over) / 2;
				if (num < best) {
					best = num;
				}
			}
		}

		private static void ListenNumber国士无双(ISortedTiles tiles, ref int best) {
			if (tiles.Count == 13) {
				short terminalCount = 0;
				short pairFlag = 0;

				foreach (var tile in tiles.GetDifferentEnumerator()) {
					if (tile.IsTerminal) {
						if (pairFlag == 0 && tiles.GetTileCount(tile) > 1) {
							pairFlag = 1;
						}
						terminalCount++;
					}
				}

				int num = 13 - pairFlag - terminalCount;
				if (num < best) {
					best = num;
				}
			}
		}

		public static Tile[] Parse(string tilesName) {
			return tilesName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(name => Tile.Parse(name)).ToArray();
		}
	}
}
