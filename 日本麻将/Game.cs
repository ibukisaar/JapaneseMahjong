using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace 日本麻将 {
	public abstract class Game {
		#region Static
		private static Game instance = null;

		public static Game Instance => instance ?? throw new InvalidOperationException($"{nameof(Game)}未创建任何默认实例。");

		static Game() {
			Load("默认规则.dll");
		}

		private static Game LoadGame(string gameName) {
			var types = Assembly.LoadFrom(gameName).GetTypes();

			var gameType = types.FirstOrDefault(t => t.IsSubclassOf(typeof(Game)));
			var game = gameType?.GetConstructor(Type.EmptyTypes)?.Invoke(null) as Game ?? new DefaultGame();

			var yakuTypes = Array.FindAll(types, t => t.IsSubclassOf(typeof(Yaku)));
			game.Yakus = new Yaku[yakuTypes.Length];
			for (int i = 0; i < yakuTypes.Length; i++) {
				game.Yakus[i] = yakuTypes[i].GetConstructor(Type.EmptyTypes).Invoke(null) as Yaku;
			}
			Array.Sort(game.Yakus);

			game.SpecialYakus = game.Yakus.OfType<SpecialYaku>().ToArray();

			var scoreSystemType = types.FirstOrDefault(t => t.IsSubclassOf(typeof(ScoreSystem)));
			game.ScoreSystem = scoreSystemType?.GetConstructor(Type.EmptyTypes)?.Invoke(null) as ScoreSystem ?? new ScoreSystem();
			return game;
		}

		public static void Load(string gameName) => instance = LoadGame(gameName);
		#endregion

		private Tile[] randomTiles;

		public Yaku[] Yakus { get; private set; }
		public SpecialYaku[] SpecialYakus { get; private set; }
		public abstract string Name { get; }
		public ScoreSystem ScoreSystem { get; private set; }

		public void BuildRandomTiles() {
			randomTiles = new Tile[34 * 4];
			for (int i = 0; i < 34; i++) {
				for (int j = 0; j < 4; j++) {
					randomTiles[i * 4 + j] = GetTile(GetTileId(i, j));
				}
			}
		}

		/// <summary>
		/// 获得牌的Id，Id不能为0。
		/// </summary>
		/// <param name="baseSortedIndex"></param>
		/// <param name="subIndex"></param>
		/// <returns></returns>
		internal protected abstract int GetTileId(int baseSortedIndex, int subIndex);

		public abstract Tile GetTile(int id);

		public Tile[] GetTiles(IEnumerable<BaseTile> baseTiles) {
			var counts = new int[BaseTile.AllTiles.Length];
			var result = new List<Tile>();
			foreach (var t in baseTiles) {
				if (counts[t.SortedIndex] >= 4)
					throw new ArgumentOutOfRangeException(nameof(baseTiles), $"[{t}]的数量超过4。");

				result.Add(GetTile(GetTileId(t.SortedIndex, counts[t.SortedIndex]++)));
			}
			return result.ToArray();
		}

		public Tile GetTile(BaseTile baseTile) => GetTile(GetTileId(baseTile.SortedIndex, 0));

		public virtual Tile[] GetRandomTiles(int count) {
			if (randomTiles == null) BuildRandomTiles();

			var result = new Tile[count];
			randomTiles.Random(count);
			Array.Copy(randomTiles, result, count);
			return result;
		}

		protected virtual void Reset(Tile tile) {
			tile.Owner = Wind.None;
			tile.State = TileState.牌山;
			tile.Dora = 0;
			tile.InDora = 0;
		}

		public override string ToString() => Name;

		public int Syanten(IEnumerable<BaseTile> tiles) {
			int result = 13;
			Syanten(new BaseTileCollection(tiles), ref result);
			return result;
		}

		internal void Syanten(IBaseTiles itiles, ref int result) {
			int best = 1 - itiles.Count % 3;
			if (itiles.Count == 13 || itiles.Count == 14) {
				foreach (var spYaku in SpecialYakus) {
					spYaku.Syanten(itiles, ref result);
					if (result == best) return;
				}
			}
			NormalSyanten(itiles.Sorted, ref result);
		}

		public virtual void NormalSyanten(SortedTilesEnumerator tiles, ref int result) {
			if (tiles.Count % 3 == 0) throw new ArgumentException("牌数量不能是3的倍数。", nameof(tiles));

			var args = new SyantenArgs {
				Result = result,
				MinValue = 1 - tiles.Count % 3
			};
			NormalSyantenCutPair(tiles.Tiles, tiles.Count, args);
			result = args.Result;
		}

		#region Private Syanten

		private class SyantenArgs {
			public int Result = 6;
			public int MaxUseTileCount = 0;
			public int MinValue = 0;
		}

		private static void NormalSyantenCutPair(int[] tiles, int count, SyantenArgs args) {
			if (args.Result == args.MinValue) return;
			for (int i = 0; i < 34; i++) {
				if (tiles[i] >= 2) {
					tiles[i] -= 2;
					NormalSyantenCut3(tiles, 0, count - 2, (count / 3) | 0x01000000, args);
					tiles[i] += 2;
				}
			}
			NormalSyantenCut3(tiles, 0, count, count / 3, args);
		}

		private static void NormalSyantenCut3(int[] tiles, int i, int remCount, int argsTuple, SyantenArgs args) {
			if (args.Result == args.MinValue) return;
			while (i < 34 && tiles[i] == 0) i++;
			if (i == 34) {
				NormalSyantenCut2(tiles, 0, remCount, argsTuple, args);
				return;
			}

			ref int curr = ref tiles[i];
			if (curr >= 3) {
				curr -= 3;
				NormalSyantenCut3(tiles, i, remCount - 3, argsTuple + 0x0100, args);
				curr += 3;
			}
			if (BaseTile.AllTiles[i] is NumberTile t && t.Number <= 7) {
				ref int next = ref tiles[i + 1], nextNext = ref tiles[i + 2];
				if (next > 0 && nextNext > 0) {
					curr--;
					next--;
					nextNext--;
					NormalSyantenCut3(tiles, i, remCount - 3, argsTuple + 0x0100, args);
					nextNext++;
					next++;
					curr++;
				}
			}

			NormalSyantenCut3(tiles, i + 1, remCount, argsTuple, args);
		}

		unsafe private static void NormalSyantenCut2(int[] tiles, int i, int remCount, int argsTuple, SyantenArgs args) {
			if (args.Result == args.MinValue) return;

			int N = ((byte*) &argsTuple)[0];
			int C3 = ((byte*) &argsTuple)[1];
			int C2 = ((byte*) &argsTuple)[2];
			int P = ((byte*) &argsTuple)[3];
			int useTileCount = C3 + (C3 + C2 + P) * 2;

			if (C3 + C2 > N) return;
			if (remCount < args.MaxUseTileCount - useTileCount) return;

			if (remCount == 0) {
				int num = (N - C3) * 2 - C2 - P;
				if (num < args.Result) args.Result = num;
				if (args.MaxUseTileCount < useTileCount) args.MaxUseTileCount = useTileCount;
				return;
			}
			while (tiles[i] == 0) i++;

			ref int curr = ref tiles[i];
			if (curr >= 2) {
				curr -= 2;
				NormalSyantenCut2(tiles, i, remCount - 2, argsTuple + 0x010000, args);
				curr += 2;
			}

			if (BaseTile.AllTiles[i] is NumberTile t) {
				ref int next = ref tiles[i + 1], nextNext = ref tiles[i + 2];
				if (t.Number <= 8 && next > 0) {
					curr--;
					next--;
					NormalSyantenCut2(tiles, i, remCount - 2, argsTuple + 0x010000, args);
					next++;
					curr++;
				}
				if (t.Number <= 7 && nextNext > 0) {
					curr--;
					nextNext--;
					NormalSyantenCut2(tiles, i, remCount - 2, argsTuple + 0x010000, args);
					nextNext++;
					curr++;
				}
			}

			var tempCurr = curr;
			curr = 0;
			NormalSyantenCut2(tiles, i + 1, remCount - tempCurr, argsTuple, args);
			curr = tempCurr;
		}

		#endregion

		public bool TestRon(IEnumerable<Tile> tiles, YakuEnvironment env = 0) {
			var itiles = new TileCollection(tiles);
			if (itiles.Count % 3 != 2)
				throw new ArgumentException("牌数量除以3的余数必须是2。", nameof(tiles));

			if (itiles.Count == 14) {
				var kindCounts = new int[4];
				foreach (var t in tiles) kindCounts[t.BaseTile.SortedLevel]++;

				foreach (var spYaku in SpecialYakus) {
					if (!spYaku.FilterTest(kindCounts, null, null)) continue;
					if (spYaku.TestRon(itiles, env)) return true;
				}
			}

			return TestNormalRon(itiles.BaseTiles.Sorted);
		}

		public virtual bool TestNormalRon(SortedTilesEnumerator tiles) {
			return TestNormalRon(tiles, false);
		}

		#region Private TestRon

		private static bool TestNormalRon(SortedTilesEnumerator tiles, bool hasPair) {
			if (tiles.Count == 0) return true;

			tiles.MoveIndex();

			if (tiles.Current >= 2) {
				if (!hasPair) {
					// 检查雀头
					tiles.Current -= 2;
					tiles.Count -= 2;
					if (TestNormalRon(tiles, true)) return true;
					tiles.Count += 2;
					tiles.Current += 2;
				}

				if (tiles.Current >= 3) {
					// 检查刻子
					tiles.Current -= 3;
					tiles.Count -= 3;
					if (TestNormalRon(tiles, hasPair)) return true;
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
				if (TestNormalRon(tiles, hasPair)) return true;
				tiles.Count += 3;
				tiles.NextNext++;
				tiles.Next++;
				tiles.Current++;
			}

			return false;
		}

		#endregion

		public virtual IReadOnlyList<AdvancedGroups> Analysis(IEnumerable<Tile> tiles, IEnumerable<Group> openGroups = null) {
			var ids = new SortedTileIdsEnumerator(tiles);
			if (ids.Count % 3 == 2) {
				Tile extra = tiles.Last();
				return Analysis(ids, extra, openGroups != null ? openGroups.ToArray() : Array.Empty<Group>());
			}
			return null;
		}

		#region Private Analysis

		private class AnalysisArgs {
			public Stack<Group> Groups = new Stack<Group>();
			public List<Group[]> Result = new List<Group[]>();
		}

		private IReadOnlyList<AdvancedGroups> Analysis(SortedTileIdsEnumerator tiles, Tile extra, Group[] openGroups) {
			var args = new AnalysisArgs();
			Analysis(args, tiles, false);
			if (args.Result.Count == 0) return null;
			var groups = args.Result.Select(gs => new SortedGroupSet(gs)).Distinct().Select(gs => gs.Groups);
			return groups.Select(gs => new AdvancedGroups(gs, extra, openGroups)).ToArray();
		}

		unsafe private void Analysis(AnalysisArgs args, SortedTileIdsEnumerator tiles, bool hasPair) {
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
					args.Groups.Push(new Pair(new[] { GetTile(t1), GetTile(t2) }));
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
					args.Groups.Push(new Pung(new[] { GetTile(t1), GetTile(t2), GetTile(t3) }));
					Analysis(args, tiles, hasPair);
					args.Groups.Pop();
					tiles.Current = (tiles.Current << 24) | temp;
					tiles.Count += 3;
				}
			}

			// 检查顺子
			if (tiles.Count >= 3) {
				int t1 = (int) (tiles.Current & 0xff);
				if (GetTile(t1).BaseTile is NumberTile t && t.Number <= 7) {
					int t2 = (int) (tiles.Next & 0xff);
					int t3 = (int) (tiles.NextNext & 0xff);
					if (t2 != 0 && t3 != 0) {
						tiles.Current >>= 8;
						tiles.Next >>= 8;
						tiles.NextNext >>= 8;
						tiles.Count -= 3;
						args.Groups.Push(new Junko(new[] { GetTile(t1), GetTile(t2), GetTile(t3) }));
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

		#endregion

		public Score GetScore(IEnumerable<Tile> tiles, IEnumerable<Group> openGroups, YakuEnvironment env) {
			var scores = GetScores(tiles, openGroups, env);
			var result = scores[0];
			for (int i = 1; i < scores.Count; i++) {
				if (ScoreSystem.Compare(result, scores[i]) < 0) result = scores[i];
			}
			return result;
		}

		#region Private GetScores

		private IReadOnlyList<Score> GetScores(IEnumerable<Tile> tiles, IEnumerable<Group> openGroups, YakuEnvironment env) {
			var envYakus = Yakus.Where(y => y.Type.HasFlag(YakuType.环境));
			var nonEnvYakus = Yakus.Where(y => !y.Type.HasFlag(YakuType.环境));
			var tempResult = new List<YakuValue>();
			foreach (var y in envYakus) {
				y.Test(tempResult, null, null, env);
			}

			var tileCollection = new TileCollection(tiles);
			var kindCountsFromTiles = new int[4];
			for (int i = 0; i < tileCollection.Count; i++) {
				kindCountsFromTiles[tileCollection[i].BaseTile.SortedLevel]++;
			}
			if (openGroups != null) {
				foreach (var g in openGroups) {
					if (!g.IsImportant) continue;
					foreach (var t in g.Tiles) {
						kindCountsFromTiles[t.BaseTile.SortedLevel]++;
					}
				}
			}

			void TryRemoveNormalYakus(List<YakuValue> result) {
				bool isFullYaku = result.Any(val => val.FullYaku > 0);
				if (isFullYaku) result.RemoveAll(y => y.FullYaku == 0);
			}

			var results = new List<Score>();
			var ags = Analysis(tiles, openGroups);
			if (ags != null) {
				foreach (var ag in ags) {
					var gss = ag.ToArray();
					var gs = gss[0];
					var (junkoCount, pungCount) = (gs.JunkoList.Length, gs.PungList.Length);
					var result = new List<YakuValue>(tempResult);

					var acceptYakus = nonEnvYakus
						.Where(y => y.FilterTest(junkoCount, pungCount) && y.FilterTest(kindCountsFromTiles, ag.KindCounts, ag.KindCountsWithoutPair));
					var yakus1 = acceptYakus.Where(y => !y.Type.HasFlag(YakuType.听牌形式));
					var yakus2 = acceptYakus.Where(y => y.Type.HasFlag(YakuType.听牌形式)).ToList();

					foreach (var y in yakus1) {
						y.Test(result, tileCollection, gs, env);
					}

					List<YakuValue> result2 = null;
					for (int i = 0; i < gss.Length; i++) {
						if (i == 0 || yakus2.Count > 0) {
							gs = gss[i];
							result2 = new List<YakuValue>(result);
							foreach (var y in yakus2) {
								y.Test(result2, tileCollection, gs, env);
							}
							TryRemoveNormalYakus(result2);
						}
						int fu = CalculateFu(result2, gs, env);
						results.Add(new Score(fu, result2));
					}
				}
			}

			// 形如11223344556677也算七对子，因此即使牌面能拆分成若干group也不能排除特殊役
			bool succ = false;
			foreach (var y in nonEnvYakus) {
				if (!y.FilterTest(0, 0)) continue;
				if (!y.FilterTest(kindCountsFromTiles, null, null)) continue;
				succ = (y.Test(tempResult, tileCollection, null, env) && y is SpecialYaku) || succ;
			}
			if (succ) {
				TryRemoveNormalYakus(tempResult);
				int fu = CalculateFu(tempResult, null, env);
				results.Add(new Score(fu, tempResult));
			}
			return results;
		}

		#endregion

		/// <summary>
		/// 计算<paramref name="group"/>的符数。
		/// <para>注意：该方法返回的符数未进行进位处理。</para>
		/// </summary>
		/// <param name="group"></param>
		/// <param name="env"></param>
		/// <returns></returns>
		protected virtual int CalculateFu(Group group, YakuEnvironment env) {
			if (group is Pair pair) {
				int fu = 0;
				if (group.Key is KanjiTile kanjiTile) {
					int val = (int) kanjiTile.Value;
					int val1 = ((int) env >> 6) & 0xf; // 自风
					int val2 = ((int) env >> 10) & 0xf; // 场风
					if (val == val1) fu += 2;
					if (val == val2) fu += 2;
					if (fu == 0 && kanjiTile.IsDragon) fu += 2;
				}
				if (group.Type == GroupType.和牌) fu += 2; // 单骑
				return fu;
			} else if (group.IsPung) {
				int addedWind = 1 << ((int) group.AddedWind - 1);
				int selfWind = (((int) env >> 6) & 0xf);
				bool isClosed = group.Type == GroupType.门清
					|| (group.Type == GroupType.和牌 && addedWind == selfWind);

				int fu = 2;
				if (group is Gan) fu *= 4;
				if (isClosed) fu *= 2;
				if (group.IsTerminal) fu *= 2;
				return fu;
			} else { //group is Junko
				var numTile = group.Key as NumberTile;
				if (group.Type == GroupType.和牌) {
					switch (group.AddedIndex) {
						case 0 when numTile.Number == 7: // 边张
						case 1: // 嵌张
						case 2 when numTile.Number == 1: // 边张
							return 2;
					}
				}
				return 0;
			}
		}

		/// <summary>
		/// 计算符数，并进行进位处理。
		/// </summary>
		/// <param name="yakuValues">表示当前成立的所有役，可能存在hook符数计算的役。</param>
		/// <param name="groups"></param>
		/// <param name="env"></param>
		/// <returns></returns>
		protected virtual int CalculateFu(IEnumerable<YakuValue> yakuValues, IGroups groups, YakuEnvironment env) {
			bool isFullYaku = yakuValues.Any(y => y.FullYaku > 0);
			if (isFullYaku) return 0;

			int fu = 20;
			fu += groups?.Sum(g => CalculateFu(g, env)) ?? 0; // 所有从面子获得的符
			if (env.HasFlag(YakuEnvironment.门前清) && !env.HasFlag(YakuEnvironment.自摸)) {
				fu += 10; // 门前清荣和
			}
			if (env.HasFlag(YakuEnvironment.自摸)) {
				fu += 2;
			}
			if (yakuValues.Any(y => y.Source.HookCalculateFu(ref fu, groups, env))) {
				return fu;
			}
			return (fu + 9) / 10 * 10; //进位
		}

		public SuggestResult Suggest(IEnumerable<BaseTile> tiles, int[] maxCounts = null) {
			var its = new BaseTileCollection(tiles);
			if (its.Count % 3 != 2) throw new ArgumentException("牌数量除以3的余数必须等于2。", nameof(tiles));

			if (maxCounts == null) {
				maxCounts = new int[34];
				for (int i = 0; i < 34; i++) maxCounts[i] = 4;
			}

			foreach (var t in its) {
				maxCounts[t.SortedIndex]--;
			}

			var best = its.Count - 1;
			Syanten(its, ref best);

			bool CanSwap(int[] counts, int index) {
				if (maxCounts[index] == 0) return false;

				var tile = BaseTile.AllTiles[index];
				if (tile is NumberTile t) {
					int start = Math.Max(t.SortedLevel * 9 + t.Number - 3, t.SortedLevel * 9);
					int end = Math.Min(t.SortedLevel * 9 + t.Number + 2, t.SortedLevel * 9 + 9);
					for (int i = start; i < end; i++) {
						if (counts[i] > 0) return true;
					}
					return false;
				} else {
					return counts[tile.SortedIndex] > 0;
				}
			}

			var suggestResult = new SuggestResult(best);
			BaseTile prev = null;
			if (best < 0) best = 0;
			var sortedCounts = its.Sorted.Tiles.Clone() as int[];
			var values = new List<(BaseTile, int)>(34);

			for (int srcIndex = 0; srcIndex < its.Count; srcIndex++) {
				if (its[srcIndex] == prev) continue;
				prev = its[srcIndex];
				var sortedIndex = its[srcIndex].SortedIndex;
				sortedCounts[sortedIndex]--;
				values.Clear();

				for (int dstIndex = 0; dstIndex < 34; dstIndex++) {
					// if (!CanSwap(sortedCounts, dstIndex)) continue;
					var dstTile = BaseTile.AllTiles[dstIndex];

					var old = its.Replace(srcIndex, dstTile);
					int result = best;
					Syanten(its, ref result);
					if (result < best) {
						values.Add((dstTile, maxCounts[dstIndex]));
					}
					its.Replace(srcIndex, old);
				}

				sortedCounts[sortedIndex]++;
				if (values.Count > 0) suggestResult.Add(new SuggestResult.SuggestItem(prev, values));
			}

			suggestResult.Sort();
			return suggestResult;
		}


	}
}
