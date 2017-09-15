using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	static class Mahjong {
		readonly static BaseTile[] RandomTiles = new BaseTile[BaseTile.AllTiles.Length * 4];
		readonly static BaseTile[] RandomWanTiles = new BaseTile[9 * 4];
		readonly static Random random = new Random();

		static Mahjong() {
			for (int i = 0; i < 4; i++) {
				Array.Copy(BaseTile.AllTiles, 0, RandomTiles, i * BaseTile.AllTiles.Length, BaseTile.AllTiles.Length);
				Array.Copy(BaseTile.AllTiles, 0, RandomWanTiles, i * 9, 9);
			}
		}

		private class AnalysisArgs {
			public Game Game;
			public Stack<Group> Groups = new Stack<Group>();
			public List<Group[]> Result = new List<Group[]>();
		}

		/*

		/// <summary>
		/// 获得所有和牌组合
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static List<Group[]> Analysis(Game game, IEnumerable<Tile> tiles) {
			var ids = new SortedTileIdsEnumerator(tiles);
			if (ids.Count % 3 == 2) {
				var args = new AnalysisArgs { Game = game };
				Tile extra = tiles.Last();
				Analysis(args, ids, extra);
				return args.Result;
			}
			return null;
		}

		private static void Analysis(AnalysisArgs args, SortedTileIdsEnumerator tiles, Tile extra) {
			Analysis(args, tiles, false);
			var groups = args.Result.Select(gs => new SortedGroupSet(gs)).Distinct().Select(gs => gs.Groups);
			args.Result = new List<Group[]>();
			foreach (var gs in groups) {
				Analysis(args.Result, gs, extra);
			}
		}

		private static void Analysis(List<Group[]> result, Group[] groups, Tile extra) {
			var indexes = new Dictionary<(int SortedLevel, int TileIndex), int>();
			(int, int) extraIndex = (0, 0);
			for (int i = 0; i < groups.Length; i++) {
				for (int j = 0; j < groups[i].Tiles.Length; j++) {
					if (groups[i].Tiles[j].BaseTile == extra.BaseTile) {
						if (groups[i].Tiles[j].Id == extra.Id) extraIndex = (i, j);
						if (!indexes.ContainsKey((groups[i].SortedLevel, j))) {
							indexes.Add((groups[i].SortedLevel, j), i);
						}
						break;
					}
				}
			}

			foreach (var kv in indexes) {
				var tempGroups = new Group[groups.Length];
				var tempTiles = groups.Select(g => g.Tiles.Clone() as Tile[]).ToArray();
				var (groupIndex, tileIndex) = extraIndex;
				if (groupIndex != kv.Value || tileIndex != kv.Key.TileIndex) {
					Tile[] ts1 = tempTiles[groupIndex], ts2 = tempTiles[kv.Value];
					var t = ts1[tileIndex];
					ts1[tileIndex] = ts2[kv.Key.TileIndex];
					ts2[kv.Key.TileIndex] = t;
				}

				for (int i = 0; i < tempGroups.Length; i++) {
					if (i != kv.Value) {
						tempGroups[i] = groups[i].Remodeling(tempTiles[i], groups[i].Type, groups[i].AddedIndex);
					} else {
						tempGroups[i] = groups[i].Remodeling(tempTiles[i], GroupType.和牌, kv.Key.TileIndex);
					}
				}

				result.Add(tempGroups);
			}
		}

		unsafe private static void Analysis(AnalysisArgs args, SortedTileIdsEnumerator tiles, bool hasPair) {
			if (tiles.Count == 0) {
				args.Result.Add(args.Groups.ToArray());
				return;
			}

			tiles.MoveIndex();

			if ((tiles.Current & 0xff00) != 0) {
				if (!hasPair) {
					// 检查雀头
					uint temp = tiles.Current & 0xffff;
					int t1 = ((byte*) &temp)[0], t2 = ((byte*) &temp)[1];
					tiles.Current >>= 16;
					tiles.Count -= 2;
					args.Groups.Push(new Pair(new[] { args.Game.AllTiles[t1], args.Game.AllTiles[t2] }));
					Analysis(args, tiles, true);
					args.Groups.Pop();
					tiles.Current = (tiles.Current << 16) | temp;
					tiles.Count += 2;
				}

				if ((tiles.Current & 0xff0000) != 0) {
					// 检查刻子
					uint temp = tiles.Current & 0xffffff;
					int t1 = ((byte*) &temp)[0], t2 = ((byte*) &temp)[1], t3 = ((byte*) &temp)[2];
					tiles.Current >>= 24;
					tiles.Count -= 3;
					args.Groups.Push(new Pung(new[] { args.Game.AllTiles[t1], args.Game.AllTiles[t2], args.Game.AllTiles[t3] }));
					Analysis(args, tiles, hasPair);
					args.Groups.Pop();
					tiles.Current = (tiles.Current << 24) | temp;
					tiles.Count += 3;
				}
			}

			// 检查顺子
			if (tiles.Count >= 3) {
				int t1 = (int) (tiles.Current & 0xff);
				if (args.Game.AllTiles[t1].BaseTile is NumberTile t && t.Number <= 7) {
					int t2 = (int) (tiles.Next & 0xff);
					int t3 = (int) (tiles.NextNext & 0xff);
					if (t2 != 0 && t3 != 0) {
						tiles.Current >>= 8;
						tiles.Next >>= 8;
						tiles.NextNext >>= 8;
						tiles.Count -= 3;
						args.Groups.Push(new Junko(new[] { args.Game.AllTiles[t1], args.Game.AllTiles[t2], args.Game.AllTiles[t3] }));
						Analysis(args, tiles, hasPair);
						args.Groups.Pop();
						tiles.Current = (tiles.Current << 8) | (uint) t1;
						tiles.Next = (tiles.Next << 8) | (uint) t2;
						tiles.NextNext = (tiles.NextNext << 8) | (uint) t3;
						tiles.Count += 3;
					}
				}
			}
		}

		*/

		/// <summary>
		/// 快速检查是否和牌
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static bool FastCheckRon(IEnumerable<BaseTile> tiles) {
			var list = new SortedTilesEnumerator(tiles);
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
		public static List<BaseTile> AllReadyHand(IEnumerable<BaseTile> tiles) {
			var etor = new SortedTilesEnumerator(tiles);
			List<BaseTile> result = new List<BaseTile>();
			if (etor.Count % 3 == 1) {
				foreach (BaseTile tile in BaseTile.AllTiles) {
					var etor2 = etor.Clone();
					etor2.Tiles[tile.SortedIndex]++;
					etor2.Count++;
					if (FastCheckRon(etor2)) {
						result.Add(tile);
					}
				}
			}
			return result;
		}

		private static bool FastCheckRon(SortedTilesEnumerator tiles) {
			return FastCheckRon七对子(tiles) || FastCheckRon国士无双(tiles) || FastCheckRon(tiles, false);
		}

		private static bool FastCheckRon(SortedTilesEnumerator tiles, bool hasPair) {
			if (tiles.Count == 0) return true;

			tiles.MoveIndex();

			if (tiles.Current >= 2) {
				if (!hasPair) {
					// 检查雀头
					tiles.Current -= 2;
					tiles.Count -= 2;
					if (FastCheckRon(tiles, true)) return true;
					tiles.Count += 2;
					tiles.Current += 2;
				}

				if (tiles.Current >= 3) {
					// 检查刻子
					tiles.Current -= 3;
					tiles.Count -= 3;
					if (FastCheckRon(tiles, hasPair)) return true;
					tiles.Count += 3;
					tiles.Current += 3;
				}
			}

			if (BaseTile.AllTiles[tiles.Index] is NumberTile t && t.Number <= 7 && tiles.Next > 0 && tiles.NextNext > 0) {
				// 检查顺子
				tiles.Current--;
				tiles.Next--;
				tiles.NextNext--;
				tiles.Count -= 3;
				if (FastCheckRon(tiles, hasPair)) return true;
				tiles.Count += 3;
				tiles.NextNext++;
				tiles.Next++;
				tiles.Current++;
			}

			return false;
		}

		public static bool FastCheckRon七对子(SortedTilesEnumerator tiles) {
			return tiles.Count == 14 && tiles.Tiles.All(cnt => cnt == 0 || cnt == 2);
		}

		private static bool FastCheckRon国士无双(SortedTilesEnumerator tiles) {
			return tiles.Count == 14 && tiles.PerfectMatch(
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

		public static BaseTile[] CreateRandomTiles() {
			RandomTiles.Random(RandomTiles.Length);
			return RandomTiles;
		}

		public static BaseTile[] CreateRandomTiles(int count) {
			BaseTile[] output = new BaseTile[count];
			RandomTiles.Random(count);
			Array.Copy(RandomTiles, output, count);
			return output;
		}

		public static BaseTile[] CreateRandomWanTiles(int count) {
			BaseTile[] output = new BaseTile[count];
			RandomWanTiles.Random(count);
			Array.Copy(RandomWanTiles, output, count);
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
		/// 求向听数
		/// </summary>
		/// <param name="tiles"></param>
		/// <returns></returns>
		public static int ListenNumber(IEnumerable<BaseTile> tiles) {
			int best = 6;
			var list = new SortedTilesEnumerator(tiles);
			ListenNumber七对子(list, ref best);
			ListenNumber国士无双(list, ref best);
			ListenNumber(list, list.Count / 3, ref best);
			return best;
		}

		public static int ListenNumber2(IEnumerable<BaseTile> tiles) {
			int best = 6;
			var sortedTiles = new SortedTilesEnumerator(tiles);
			if (sortedTiles.Count % 3 == 0) throw new ArgumentException($"{nameof(tiles)}的数量必须不能被3整除。即N%3==1或N%3==2。", nameof(tiles));

			ListenNumber七对子(sortedTiles, ref best);
			//ListenNumber国士无双(sortedTiles, ref best);
			//var args = new ListenNumberArgs { Best = best, Beta = /*sortedTiles.Count % 3 - 1*/0 };
			// ListenNumber(sortedTiles, sortedTiles.Count / 3, args);
			// return args.Best;
			return best;
		}

		private static void ListenNumber(SortedTilesEnumerator tiles, int n, ref int best) {
			if (best == 0) return;
			for (int i = 0; i < 34; i++) {
				if (tiles.Tiles[i] >= 2) {
					tiles.Tiles[i] -= 2;
					ListenNumberCut3(tiles, n | 0x01000000, ref best);
					tiles.Tiles[i] += 2;
				}
			}
			ListenNumberCut3(tiles, n, ref best);
		}

		private static void ListenNumberCut3(SortedTilesEnumerator tiles, int argsTuple, ref int best) {
			tiles.MoveIndex();
			if (tiles.Index == 34) {
				tiles.Index = 0;
				ListenNumberCut2(tiles, argsTuple, ref best);
				return;
			}

			if (tiles.Current >= 3) {
				tiles.Current -= 3;
				ListenNumberCut3(tiles, argsTuple + 0x0100, ref best);
				tiles.Current += 3;
			}
			if (BaseTile.AllTiles[tiles.Index] is NumberTile t && t.Number <= 7) {
				if (tiles.Next > 0 && tiles.NextNext > 0) {
					tiles.Current--;
					tiles.Next--;
					tiles.NextNext--;
					ListenNumberCut3(tiles, argsTuple + 0x0100, ref best);
					tiles.NextNext++;
					tiles.Next++;
					tiles.Current++;
				}
			}

			tiles.Index++;
			ListenNumberCut3(tiles, argsTuple, ref best);
		}

		unsafe private static void ListenNumberCut2(SortedTilesEnumerator tiles, int argsTuple, ref int best) {
			int N = ((byte*) &argsTuple)[0];
			int C3 = ((byte*) &argsTuple)[1];
			int C2 = ((byte*) &argsTuple)[2];
			int P = ((byte*) &argsTuple)[3];
			if (best == 0 || C3 + C2 > N) return;

			tiles.MoveIndex();
			if (tiles.Index == 34) {
				int num = (N - C3) * 2 - C2 - P;
				if (num < best) best = num;
				return;
			}

			if (tiles.Current >= 2) {
				tiles.Current -= 2;
				ListenNumberCut2(tiles, argsTuple + 0x010000, ref best);
				tiles.Current += 2;
			}

			if (BaseTile.AllTiles[tiles.Index] is NumberTile t) {
				if (t.Number <= 8 && tiles.Next > 0) {
					tiles.Current--;
					tiles.Next--;
					ListenNumberCut2(tiles, argsTuple + 0x010000, ref best);
					tiles.Next++;
					tiles.Current++;
				}
				if (t.Number <= 7 && tiles.NextNext > 0) {
					tiles.Current--;
					tiles.NextNext--;
					ListenNumberCut2(tiles, argsTuple + 0x010000, ref best);
					tiles.NextNext++;
					tiles.Current++;
				}
			}

			tiles.Index++;
			ListenNumberCut2(tiles, argsTuple, ref best);
		}

		private class ListenNumberArgs {
			public int Best = 6;
			public int MaxUseTileCount = 0;
			public int Beta = 0;
		}

		private static void ListenNumber(SortedTilesEnumerator tiles, int n, ListenNumberArgs args) {
			if (args.Best <= 0) return;
			for (int i = 0; i < 34; i++) {
				if (tiles.Tiles[i] >= 2) {
					tiles.Tiles[i] -= 2;
					ListenNumberCut3(tiles, tiles.Count - 2, n | 0x01000000, args);
					tiles.Tiles[i] += 2;
				}
			}
			ListenNumberCut3(tiles, tiles.Count, n, args);
		}

		private static void ListenNumberCut3(SortedTilesEnumerator tiles, int remCount, int argsTuple, ListenNumberArgs args) {
			tiles.MoveIndex();
			if (tiles.Index == 34) {
				tiles.Index = 0;
				ListenNumberCut2(tiles, remCount, argsTuple, args);
				return;
			}

			if (tiles.Current >= 3) {
				tiles.Current -= 3;
				ListenNumberCut3(tiles, remCount - 3, argsTuple + 0x0100, args);
				tiles.Current += 3;
			}
			if (BaseTile.AllTiles[tiles.Index] is NumberTile t && t.Number <= 7) {
				if (tiles.Next > 0 && tiles.NextNext > 0) {
					tiles.Current--;
					tiles.Next--;
					tiles.NextNext--;
					ListenNumberCut3(tiles, remCount - 3, argsTuple + 0x0100, args);
					tiles.NextNext++;
					tiles.Next++;
					tiles.Current++;
				}
			}

			tiles.Index++;
			ListenNumberCut3(tiles, remCount, argsTuple, args);
		}

		unsafe private static void ListenNumberCut2(SortedTilesEnumerator tiles, int remCount, int argsTuple, ListenNumberArgs args) {
			int N = ((byte*) &argsTuple)[0];
			int C3 = ((byte*) &argsTuple)[1];
			int C2 = ((byte*) &argsTuple)[2];
			int P = ((byte*) &argsTuple)[3];
			int useTileCount = C3 + (C3 + C2 + P) * 2;

			if (args.Best <= 0) return;
			if (C3 + C2 > N) return;
			if (remCount < args.MaxUseTileCount - useTileCount) return;

			if (remCount == 0) {
				int num = (N - C3) * 2 - C2 - P + args.Beta;
				if (num < args.Best) args.Best = num;
				if (args.MaxUseTileCount < useTileCount) args.MaxUseTileCount = useTileCount;
				return;
			}
			tiles.MoveIndex();

			if (tiles.Current >= 2) {
				tiles.Current -= 2;
				ListenNumberCut2(tiles, remCount - 2, argsTuple + 0x010000, args);
				tiles.Current += 2;
			}

			if (BaseTile.AllTiles[tiles.Index] is NumberTile t) {
				if (t.Number <= 8 && tiles.Next > 0) {
					tiles.Current--;
					tiles.Next--;
					ListenNumberCut2(tiles, remCount - 2, argsTuple + 0x010000, args);
					tiles.Next++;
					tiles.Current++;
				}
				if (t.Number <= 7 && tiles.NextNext > 0) {
					tiles.Current--;
					tiles.NextNext--;
					ListenNumberCut2(tiles, remCount - 2, argsTuple + 0x010000, args);
					tiles.NextNext++;
					tiles.Current++;
				}
			}

			var cur = tiles.Current;
			tiles.Current = 0;
			ListenNumberCut2(tiles, remCount - cur, argsTuple, args);
			tiles.Current = cur;
		}

		unsafe private static void ListenNumber七对子(SortedTilesEnumerator tiles, ref int best) {
			if (tiles.Count == 13 || tiles.Count == 14) {
				int beta = 1 - tiles.Count % 2;
				int c1 = 0, c2 = 0;
				for (int i = 0; i < 34; i++) {
					if (tiles.Tiles[i] >= 1) c1++; else continue;
					if (tiles.Tiles[i] >= 2) c2++;
				}

				int num = 6 - c2;
				if (c1 < 7) num += 7 - c1;
				if (num < best) {
					best = num;
				}
			}
		}

		private static void ListenNumber国士无双(SortedTilesEnumerator tiles, ref int best) {
			if (best <= 0) return;
			if (tiles.Count == 13 || tiles.Count == 14) {
				int terminalCount = 0;
				int pairFlag = 0;

				for (int i = 0; i < 34; i++) {
					if (tiles.Tiles[i] == 0) continue;

					BaseTile tile = BaseTile.AllTiles[i];
					if (!tile.IsTerminal) continue;

					if (pairFlag == 0 && tiles.Tiles[i] >= 2) {
						pairFlag = 1;
					}
					terminalCount++;
				}

				int num = 13 - pairFlag - terminalCount;
				if (num < best) {
					best = num;
				}
			}
		}

		/*
		private class SyantenArg {
			public int[] Tiles;
			public int BaseNumber;
			public int Length;
			public int MaxUseTileCount;
			public int N;
			public int C3;
			public int C2;
			public int P;
		}

		private static void GetNumberTileSyantenArgs(SyantenArg arg, int remCount) {
			if (arg.P == 0) {
				for (int i = 0; i < arg.Length; i++) {
					if (arg.Tiles[i] >= 2) {
						arg.Tiles[i] -= 2;
						GetNumberTileSyantenArgsCut3(arg, remCount - 2, 0, arg.C3, 1);
						arg.Tiles[i] += 2;
					}
				}
			}
			GetNumberTileSyantenArgsCut3(arg, remCount, 0, arg.C3, arg.P);
		}

		private static void GetNumberTileSyantenArgsCut3(SyantenArg arg, int remCount, int i, int C3, int P) {
			while (i < arg.Length && arg.Tiles[i] == 0) i++;
			if (i == arg.Length) {
				GetNumberTileSyantenArgsCut2(arg, remCount, 0, C3, arg.C2, P);
				return;
			}

			if (arg.Tiles[i] >= 3) {
				arg.Tiles[i] -= 3;
				GetNumberTileSyantenArgsCut3(arg, remCount - 3, i, C3 + 1, P);
				arg.Tiles[i] += 3;
			}
			if (i + arg.BaseNumber <= 7 && i + 2 < arg.Length && arg.Tiles[i + 1] > 0 && arg.Tiles[i + 2] > 0) {
				arg.Tiles[i]--;
				arg.Tiles[i + 1]--;
				arg.Tiles[i + 2]--;
				GetNumberTileSyantenArgsCut3(arg, remCount - 3, i, C3 + 1, P);
				arg.Tiles[i]++;
				arg.Tiles[i + 1]++;
				arg.Tiles[i + 2]++;
			}
			GetNumberTileSyantenArgsCut3(arg, remCount, i + 1, C3, P);
		}

		private static void GetNumberTileSyantenArgsCut2(SyantenArg arg, int remCount, int i, int C3, int C2, int P) {
			if (C3 + C2 > arg.N) return;
			int useTileCount = C3 + (C3 + C2 + P) * 2;
			if (remCount < arg.MaxUseTileCount - useTileCount) return;

			if (remCount == 0) {
				if (arg.MaxUseTileCount < useTileCount) {
					arg.MaxUseTileCount = useTileCount;
					arg.C3 = C3;
					arg.C2 = C2;
					arg.P = P;
				}
				return;
			}
			while (i < arg.Length && arg.Tiles[i] == 0) i++;

			if (arg.Tiles[i] >= 2) {
				arg.Tiles[i] -= 2;
				GetNumberTileSyantenArgsCut2(arg, remCount - 2, i, C3, C2 + 1, P);
				arg.Tiles[i] += 2;
			}
			if (i + arg.BaseNumber <= 8 && i + 1 < arg.Length && arg.Tiles[i + 1] > 0) {
				arg.Tiles[i]--;
				arg.Tiles[i + 1]--;
				GetNumberTileSyantenArgsCut2(arg, remCount - 2, i, C3, C2 + 1, P);
				arg.Tiles[i]++;
				arg.Tiles[i + 1]++;
			}
			if (i + arg.BaseNumber <= 7 && i + 2 < arg.Length && arg.Tiles[i + 2] > 0) {
				arg.Tiles[i]--;
				arg.Tiles[i + 2]--;
				GetNumberTileSyantenArgsCut2(arg, remCount - 2, i, C3, C2 + 1, P);
				arg.Tiles[i]++;
				arg.Tiles[i + 2]++;
			}
			var cur = arg.Tiles[i];
			arg.Tiles[i] = 0;
			GetNumberTileSyantenArgsCut2(arg, remCount - cur, i + 1, C3, C2, P);
			arg.Tiles[i] = cur;
		}

		private static void GetKanjiTileSyantenArgs(SyantenArg arg, int remCount) {
			for (int i = 0; i < arg.Length; i++) {
				if (arg.Tiles[i] >= 3) {
					arg.C3++;
					if (arg.C2 + arg.C3 > arg.N) arg.C2--;
				} else if (arg.Tiles[i] >= 2) {
					if (arg.P == 0) {
						arg.P = 1;
					} else {
						if (arg.C2 + arg.C3 < arg.N) arg.C2++;
					}
				}
			}
		}

		private static IEnumerable<SyantenArg> GetSyantenArgs(SortedTilesEnumerator tiles) {
			IEnumerable<SyantenArg> GetNumberArgs(int start, int end) {
				int GetArgLength() {
					while (start < end && tiles.Tiles[start] == 0) start++;
					if (start == end) return 0;
					int len = 1;
					while (true) {
						if (start + len >= end) return len;
						if (tiles.Tiles[start + len] > 0) {
							len++; continue;
						}
						if (start + len + 1 >= end) return len;
						if (tiles.Tiles[start + len + 1] > 0) {
							len += 2; continue;
						} else {
							return len;
						}
					}
				}

				while (true) {
					int length = GetArgLength();
					if (length == 0) yield break;
					var tempTiles = new int[length];
					Array.Copy(tiles.Tiles, start, tempTiles, 0, length);
					yield return new SyantenArg {
						BaseNumber = start % 9 + 1,
						Tiles = tempTiles,
						Length = length,
					};
					start += length;
				}
			}

			foreach (var arg in GetNumberArgs(0, 9)) yield return arg;
			foreach (var arg in GetNumberArgs(9, 18)) yield return arg;
			foreach (var arg in GetNumberArgs(18, 27)) yield return arg;
		}

		public static void Syanten(IEnumerable<BaseTile> tiles, ref int best) {
			var result = new List<(int C3, int C2, int P)>();
			var sortedTiles = new SortedTilesEnumerator(tiles);
			int N = sortedTiles.Count / 3, C3 = 0, C2 = 0, P = 0;
			int remCount;

			var kanjiTiles = new int[7];
			Array.Copy(sortedTiles.Tiles, 27, kanjiTiles, 0, 7);
			remCount = kanjiTiles.Sum();
			var kanjiArg = new SyantenArg { N = N };
			GetKanjiTileSyantenArgs(kanjiArg, remCount);
			result.Add((C3, C2, P) = (kanjiArg.C3, kanjiArg.C2, kanjiArg.P));

			foreach (var arg in GetSyantenArgs(sortedTiles)) {
				remCount = arg.Tiles.Sum();
				arg.N = N;
				arg.C3 = C3;
				arg.C2 = C2;
				arg.P = P;
				GetNumberTileSyantenArgs(arg, remCount);
				result.Add((C3, C2, P) = (arg.C3, arg.C2, arg.P));
			}

		}
		//*/

		public static BaseTile[] Parse(string tilesName) {
			return tilesName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(name => BaseTile.Parse(name)).ToArray();
		}
	}
}
