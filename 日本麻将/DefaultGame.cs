using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将.表生成;

namespace 日本麻将 {
	public class DefaultGame : Game {
		private static readonly Dictionary<ulong, MahjongTable.Info> mahjongTable = new Dictionary<ulong, MahjongTable.Info>(1300000);

		static DefaultGame() {
			foreach (var (value, info) in MahjongTable.Read2("mahjong2.table")) {
				mahjongTable.Add(value, info);
			}
		}



		public Tile[] AllTiles { get; }

		public override string Name => "默认规则";

		internal protected override int GetTileId(int baseSortedIndex, int subIndex) => baseSortedIndex * 4 + subIndex + 1;

		public override Tile GetTile(int id) => AllTiles[id - 1];

		public DefaultGame() {
			var tiles = new Tile[BaseTile.AllTiles.Length * 4];
			for (int i = 0; i < BaseTile.AllTiles.Length; i++) {
				for (int j = 0; j < 4; j++) {
					tiles[4 * i + j] = new Tile(BaseTile.AllTiles[i], GetTileId(i, j));
				}
			}
			this.AllTiles = tiles;
		}

		private static ulong BuildKey(SortedTilesEnumerator tiles) {
			ulong result = 0xF;
			int prevIndex = -3;
			for (int i = 0; i < 34; i++) {
				if (i % 9 == 0 || i >= 27) prevIndex = -3;
				if (tiles.Tiles[i] == 0) continue;
				int diffIndex = i - prevIndex - 1;
				result = (result << 4) | (diffIndex >= 2 ? 8U : (uint) diffIndex << 2);
				result |= (byte) (tiles.Tiles[i] - 1);
				prevIndex = i;
			}
			result = MahjongTable.Rebuild(result);
			return result;
		}

		public override bool TestNormalRon(SortedTilesEnumerator tiles) {
			if (mahjongTable.TryGetValue(BuildKey(tiles), out var info)) {
				return info.Syanten == -1;
			} else {
				throw new NotImplementedException("mahjong.table中不存在的项。可能是牌数除以3余数不等于2，或者BuildKey方法存在错误。");
			}
		}

		public override void NormalSyanten(SortedTilesEnumerator tiles, ref int result) {
			if (mahjongTable.TryGetValue(BuildKey(tiles), out var info)) {
				if (info.Syanten < result) result = info.Syanten;
			} else {
				throw new NotImplementedException("mahjong.table中不存在的项。可能是牌数除以3余数不等于2，或者BuildKey方法存在错误。");
			}
		}

		struct Key : IComparer<Key> {
			public static readonly IComparer<Key> Comparer = new Key();

			public ulong Value;
			public int Bits;
			public ArraySegment<List<Tile>> Tiles;

			public Key(ulong value, int bits, ArraySegment<List<Tile>> tiles) {
				var flip = Flip(value, bits);
				if (flip < value) {
					value = flip;
					Array.Reverse(tiles.Array, tiles.Offset, tiles.Count);
				}
				Value = value;
				Bits = bits;
				Tiles = tiles;
			}

			static ulong Flip(ulong value, int length) {
				ulong temp = 0b10UL << (length - 2);
				int maxShift = length - 4;
				int maxShift2 = length - 8;
				for (int shift = 0; shift < length; shift += 4) {
					temp |= ((value >> shift) & 0b0011UL) << (maxShift - shift);
					if (shift < length - 4) {
						temp |= ((value >> shift) & 0b1100UL) << (maxShift2 - shift);
					}
				}
				return temp;
			}

			public int Compare(Key x, Key y) {
				return x.Value.CompareTo(y.Value);
			}
		}

		private static List<Key> BuildKey(IEnumerable<Tile> tiles, out Tile extra) {
			List<Tile>[] tilesMap = new List<Tile>[34];
			int listCount = 0;
			extra = null;
			foreach (var t in tiles) {
				ref List<Tile> list = ref tilesMap[t.BaseTile.SortedIndex];
				if (list == null) {
					list = new List<Tile>() { t };
					listCount++;
				} else {
					list.Add(t);
				}
				extra = t;
			}

			var template = new List<Tile>[listCount];
			for (int i = 0, listIndex = 0; i < 34; i++) {
				if (tilesMap[i] == null) continue;
				template[listIndex++] = tilesMap[i];
			}

			var result = new List<Key>();
			int start = 0, length = 0;
			int prevIndex = -3;
			ulong value = 0;
			int shift = 0;
			int diffIndex;
			for (int i = 0; i <= 34; i++) {
				if (i % 9 == 0 || i >= 27) prevIndex = -3;
				if (i < 34 && tilesMap[i] == null) continue;
				diffIndex = i - prevIndex - 1;
				if (diffIndex >= 2) {
					value |= (2UL << shift) >> 2;
					if (value != 0) {
						result.Add(new Key(value, shift, new ArraySegment<List<Tile>>(template, start, length)));
						start += length;
						length = 0;
						value = 0;
						shift = 0;
					}
				} else {
					value |= (ulong) diffIndex << (shift - 2);
				}
				if (i == 34) continue;
				value |= (ulong) (tilesMap[i].Count - 1) << shift;
				length++;
				shift += 4;
				prevIndex = i;
			}

			result.Sort(Key.Comparer);
			return result;
		}

		private static ulong BuildKey(IReadOnlyList<Key> keys) {
			ulong value = 0xF;
			for (int i = 0; i < keys.Count; i++) {
				value = (value << keys[i].Bits) | keys[i].Value;
			}
			return value;
		}

		public override IReadOnlyList<AdvancedGroups> Analysis(IEnumerable<Tile> tiles, IEnumerable<Group> openGroups = null) {
			var keys = BuildKey(tiles, out var extra);
			var value = BuildKey(keys);
			if (mahjongTable.TryGetValue(value, out var info)) {
				if (info.Syanten >= 0) return null;
			} else {
				throw new NotImplementedException("mahjong.table中不存在的项。可能是牌数除以3余数不等于2，或者BuildKey方法存在错误。");
			}


			var result = new List<AdvancedGroups>();
			keys.Reverse();
			var template = keys.SelectMany(key => key.Tiles).ToList();
			var openGroupArray = openGroups?.ToArray() ?? Array.Empty<Group>();
			var groups = new List<Group>(5);
			var indexes = new int[template.Count];
			for (int i = 0; i < info.Analysis.Count; i++) {
				if (i > 0) Array.Clear(indexes, 0, indexes.Length);
				var analysis = info.Analysis[i];
				groups.Clear();

				int pairIndex = analysis.Pair - 1;
				groups.Add(new Pair(new[] { template[pairIndex][indexes[pairIndex]++], template[pairIndex][indexes[pairIndex]++] }));
				var groupIds = analysis.Groups;
				for (int junkoIndex = 0; junkoIndex < analysis.JunkoCount; junkoIndex++, groupIds >>= 8) {
					var index = (int) (groupIds & 0xFF);
					groups.Add(new Junko(new[] {
						template[index - 2][indexes[index - 2]++],
						template[index - 1][indexes[index - 1]++],
						template[index - 0][indexes[index - 0]++],
					}));
				}
				for (; groupIds != 0; groupIds >>= 8) {
					var index = (int) (groupIds & 0xFF) - 1;
					groups.Add(new Pung(new[] {
						template[index][indexes[index]++],
						template[index][indexes[index]++],
						template[index][indexes[index]++],
					}));
				}
				result.Add(new AdvancedGroups(groups.ToArray(), extra, openGroupArray));
			}
			return result;
		}
	}
}
