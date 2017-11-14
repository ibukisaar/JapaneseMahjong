using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public static class MahjongTable {
		struct RebuildItem : IComparer<RebuildItem> {
			public static readonly IComparer<RebuildItem> Comparer = new RebuildItem();

			public ulong Value;
			public int Length;

			public RebuildItem(ulong value, int length) {
				Value = Math.Min(value, Flip(value, length));
				Length = length;
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

			public int Compare(RebuildItem x, RebuildItem y) {
				return x.Value.CompareTo(y.Value);
			}
		}

		private class SyantenArgs {
			public int N;
			public int Result = 13;
			public int MaxUseTileCount = 0;
		}

		public struct AnalysisResult {
			public byte JunkoCount;
			public byte Pair;
			public uint Groups;

			public AnalysisResult(byte pair, int junkoCount, uint junkos, int pungCount, uint pungs) {
				Pair = pair;
				JunkoCount = (byte) junkoCount;
				junkos = Sort(junkos, junkoCount);
				pungs = Sort(pungs, pungCount);
				Groups = (pungs << (junkoCount * 8)) | junkos;
			}

			unsafe public static AnalysisResult Build(int arithmetic) {
				arithmetic = Math.DivRem(arithmetic, 5, out int junkoCount);
				arithmetic = Math.DivRem(arithmetic, 15, out int pair);
				uint groups = 0;
				int i = 0;
				while (arithmetic != 0) {
					arithmetic = Math.DivRem(arithmetic, 15, out int group);
					((byte*) &groups)[i++] = (byte) group;
				}
				return new AnalysisResult { Pair = (byte) pair, JunkoCount = (byte) junkoCount, Groups = groups };
			}

			unsafe static uint Sort(uint value, int bytes) {
				for (int i = 0; i < bytes; i++) {
					ref byte x = ref ((byte*) &value)[i];
					for (int j = i + 1; j < bytes; j++) {
						ref byte y = ref ((byte*) &value)[j];
						if (x > y) (x, y) = (y, x);
					}
				}
				return value;
			}

			unsafe public uint GetArithmetic(bool end, out int bytes) {
				uint result = 0;
				var gs = Groups;
				for (int i = 3; i >= 0; i--) {
					if (((byte*) &gs)[i] == 0) continue;
					result = result * 15 + ((byte*) &gs)[i];
				}
				result = result * 15 + Pair;
				result = result * 5 + JunkoCount;
				result <<= 3;
				bytes = GetIntBytes(result);
				return result | ((uint) (bytes - 1) << 1) | (end ? 0U : 1U);
			}

			public override string ToString() {
				var buffer = new StringBuilder(30);
				var gs = Groups;

				buffer.Append("Pair ").Append(Pair);
				if (JunkoCount > 0) {
					buffer.Append(" Junko ");
					for (int junkoIndex = 0; junkoIndex < JunkoCount; junkoIndex++, gs >>= 8) {
						buffer.Append(gs & 0xFF).Append(' ');
					}
				} else {
					buffer.Append(' ');
				}

				if (gs != 0) {
					buffer.Append("Pung");
					for (; gs != 0; gs >>= 8) {
						buffer.Append(' ').Append(gs & 0xFF);
					}
				}

				return buffer.ToString();
			}
		}

		public struct Info {
			public int Syanten;
			public IReadOnlyList<AnalysisResult> Analysis;
		}


		static List<RebuildItem> cache = new List<RebuildItem>();
		static readonly ulong[] DiffTables = { 0x2C, 0x30, 0x54, 0x210, 0x21, 0x50, 0x20C, 0x84, 0x240, 0x1890, 0x18C, 0x45, 0x3BC, 0x18C0, 0x3F0, 0x17D, 0x20, 0x3C0, 0x630, 0x188C, 0x108, 0x24, 0x11DC, 0x1B00, 0x12690, 0x32D, 0x2CCC, 0x79, 0x49, 0x60, 0x181, 0x17C, };
		static Dictionary<ulong, int> DiffIndexMap = new Dictionary<ulong, int> { { 0x2C, 0 }, { 0x30, 1 }, { 0x54, 2 }, { 0x210, 3 }, { 0x21, 4 }, { 0x50, 5 }, { 0x20C, 6 }, { 0x84, 7 }, { 0x240, 8 }, { 0x1890, 9 }, { 0x18C, 10 }, { 0x45, 11 }, { 0x3BC, 12 }, { 0x18C0, 13 }, { 0x3F0, 14 }, { 0x17D, 15 }, { 0x20, 16 }, { 0x3C0, 17 }, { 0x630, 18 }, { 0x188C, 19 }, { 0x108, 20 }, { 0x24, 21 }, { 0x11DC, 22 }, { 0x1B00, 23 }, { 0x12690, 24 }, { 0x32D, 25 }, { 0x2CCC, 26 }, { 0x79, 27 }, { 0x49, 28 }, { 0x60, 29 }, { 0x181, 30 }, { 0x17C, 31 }, };


		[DebuggerStepThrough]
		static void Get(ulong value, int shift, out int continuous, out int count) {
			continuous = Math.DivRem((int) ((value >> shift) & 0xF), 5, out count);
		}

		[DebuggerStepThrough]
		static ulong Set(ulong value, int shift, int continuous, int count) {
			return (value & ~(0xFUL << shift)) | ((ulong) (continuous * 5 + count) << shift);
		}

		unsafe static int GetIntBytes(uint value) {
			if (((byte*) &value)[3] != 0) return 4;
			if (((byte*) &value)[2] != 0) return 3;
			if (((byte*) &value)[1] != 0) return 2;
			if (((byte*) &value)[0] != 0) return 1;
			return 0;
		}

		unsafe static int GetIntBytes(ulong value) {
			if (((byte*) &value)[7] != 0) return 8;
			if (((byte*) &value)[6] != 0) return 7;
			if (((byte*) &value)[5] != 0) return 6;
			if (((byte*) &value)[4] != 0) return 5;
			if (((byte*) &value)[3] != 0) return 4;
			if (((byte*) &value)[2] != 0) return 3;
			if (((byte*) &value)[1] != 0) return 2;
			if (((byte*) &value)[0] != 0) return 1;
			return 0;
		}

		public static string Str(ulong value) {
			var buffer = new StringBuilder();
			bool pass = false;
			for (int shift = 60; shift >= 0; shift -= 4) {
				if (pass) {
					var con = (value >> (shift + 2)) & 3;
					var count = (value >> shift) & 3;
					buffer.Append($"{(con == 0 ? " " : con == 1 ? " . " : " ** ")}{(char) (count + '1')}");
				}
				if (!pass && (value >> shift) == 0xF) pass = true;
			}
			return buffer.ToString();
		}

		unsafe public static ulong Arithmetic(ulong value) {
			ulong arithmetic = 0;
			int shift = 60;
			for (; (value >> shift) != 0xF; shift -= 4) ;
			shift -= 4;
			for (; shift >= 0; shift -= 4) {
				arithmetic = arithmetic * 12 + ((value >> shift) & 0xF);
			}
			return arithmetic;
		}

		static void Decomposition(List<int[]> result, Stack<int> stack, int sum) {
			if (sum == 0) {
				result.Add(stack.Reverse().ToArray());
			} else {
				for (int i = 1; i <= 4; i++) {
					if (sum >= i) {
						stack.Push(i);
						Decomposition(result, stack, sum - i);
						stack.Pop();
					}
				}
			}
		}

		static ulong BuildUInt64(int[] values) {
			ulong result = 0;
			int shift = 0;
			for (int i = 0; i < values.Length; i++, shift += 4) {
				result |= (ulong) (values[i] - 1) << shift;
			}
			result |= 0xFUL << shift;
			return result;
		}

		static List<int[]> Decomposition(int N) {
			List<int[]> result = new List<int[]>();
			Decomposition(result, new Stack<int>(), N);
			return result;
		}

		static IEnumerable<ulong> EnumValue(int shiftLevel, int shiftCount, ulong value, int continuous = 0) {
			if (shiftLevel >= shiftCount) {
				yield return value;
			} else {
				for (int i = 0; i < 3; i++) {
					if (i <= 1) {
						if (continuous + i + 1 > 8) continue;
						foreach (var result in EnumValue(shiftLevel + 1, shiftCount, value | ((ulong) i << (shiftLevel * 4 + 2)), continuous + i + 1)) {
							yield return result;
						}
					} else {
						foreach (var result in EnumValue(shiftLevel + 1, shiftCount, value | ((ulong) i << (shiftLevel * 4 + 2)))) {
							yield return result;
						}
					}
				}
			}
		}

		static IEnumerable<ulong> EnumValue(int N) {
			if (N % 3 != 2) throw new ArgumentException("N除以3的余数必须等于2");

			foreach (var list in Decomposition(N)) {
				var buildValue = BuildUInt64(list);
				buildValue |= 0b1000UL << ((list.Length - 1) * 4);
				foreach (var value in EnumValue(0, list.Length - 1, buildValue)) {
					if (Valid(value)) yield return value;
				}
			}
		}

		static bool Valid(ulong value) {
			int level = 0;
			int continuous = 0, tempContinuous = 1;
			for (int shift = 0; (value >> shift) != 0xF; shift += 4) {
				int singleContinuous = (int) (value >> (shift + 2)) & 3;
				if (singleContinuous < 2) {
					if (level >= 3) return false;
					tempContinuous += singleContinuous + 1;
				} else if (level < 3) {
					if (continuous + 2 + tempContinuous <= 9) {
						continuous += 2 + tempContinuous;
					} else {
						continuous = 0;
						tempContinuous = 1;
						level++;
					}
				}
			}
			return true;
		}

		static IReadOnlyList<AnalysisResult> Analysis(ulong value) {
			var result = new List<AnalysisResult>();
			AnalysisCutPair(result, value);
			return result;
		}

		static void AnalysisCutPair(List<AnalysisResult> result, ulong value) {
			for (int shift = 0; (value >> shift) != 0; shift += 4) {
				Get(value, shift, out int continuous, out int singleCount);
				if (singleCount >= 2) {
					AnalysisCut3(result, Set(value, shift, continuous, singleCount - 2), 0, (byte) ((shift >> 2) + 1), 0, 0, 0, 0);
				}
			}
		}

		static void AnalysisCut3(List<AnalysisResult> result, ulong value, int shift, byte pair, int junkoCount, uint junkos, int pungCount, uint pungs) {
			while ((value >> shift) != 0 && ((value >> shift) & 0xF) % 5 == 0) shift += 4;
			if ((value >> shift) == 0) {
				var newResult = new AnalysisResult(pair, junkoCount, junkos, pungCount, pungs);
				if (!result.Contains(newResult)) result.Add(newResult);
				return;
			}

			Get(value, shift, out int continuous, out int singleCount);
			if (singleCount >= 3) {
				var temp = Set(value, shift, continuous, singleCount - 3);
				AnalysisCut3(result, temp, shift, pair, junkoCount, junkos, pungCount + 1, (pungs << 8) | ((uint) (shift >> 2) + 1));
			}
			if (continuous == 0) {
				Get(value, shift + 4, out int continuous2, out int singleCount2);
				if (continuous2 == 0 && singleCount2 > 0) {
					Get(value, shift + 8, out int continuous3, out int singleCount3);
					if (singleCount3 > 0) {
						var temp = Set(value, shift, continuous, singleCount - 1);
						temp = Set(temp, shift + 4, continuous2, singleCount2 - 1);
						temp = Set(temp, shift + 8, continuous3, singleCount3 - 1);
						AnalysisCut3(result, temp, shift, pair, junkoCount + 1, (junkos << 8) | ((uint) (shift >> 2) + 2), pungCount, pungs);
					}
				}
			}
		}

		static void SyantenCutPair(ulong value, int count, SyantenArgs args) {
			for (int shift = 0; (value >> shift) != 0; shift += 4) {
				Get(value, shift, out int continuous, out int singleCount);
				if (singleCount >= 2) {
					SyantenCut3(Set(value, shift, continuous, singleCount - 2), 0, count - 2, 0, 0, 1, args);
				}
			}

			SyantenCut3(value, 0, count, 0, 0, 0, args);
		}

		static void SyantenCut3(ulong value, int shift, int remCount, int C3, int C2, int P, SyantenArgs args) {
			while ((value >> shift) != 0 && ((value >> shift) & 0xF) % 5 == 0) shift += 4;
			if ((value >> shift) == 0) {
				SyantenCut2(value, 0, remCount, C3, C2, P, args);
				return;
			}

			Get(value, shift, out int continuous, out int singleCount);
			if (singleCount >= 3) {
				SyantenCut3(Set(value, shift, continuous, singleCount - 3), shift + 4, remCount - 3, C3 + 1, C2, P, args);
			}
			if (continuous == 0) {
				Get(value, shift + 4, out int continuous2, out int singleCount2);
				if (continuous2 == 0 && singleCount2 > 0) {
					Get(value, shift + 8, out int continuous3, out int singleCount3);
					if (singleCount3 > 0) {
						var temp = Set(value, shift, continuous, singleCount - 1);
						temp = Set(temp, shift + 4, continuous2, singleCount2 - 1);
						temp = Set(temp, shift + 8, continuous3, singleCount3 - 1);
						SyantenCut3(temp, shift, remCount - 3, C3 + 1, C2, P, args);
					}
				}
			}

			SyantenCut3(value, shift + 4, remCount, C3, C2, P, args);
		}

		static void SyantenCut2(ulong value, int shift, int remCount, int C3, int C2, int P, SyantenArgs args) {
			if (args.Result == 0) return;

			if (C3 + C2 > args.N) return;
			int useTileCount = C3 + (C3 + C2 + P) * 2;
			if (remCount < args.MaxUseTileCount - useTileCount) return;

			if (remCount == 0) {
				int num = (args.N - C3) * 2 - C2 - P;
				if (num < args.Result) args.Result = num;
				if (args.MaxUseTileCount < useTileCount) args.MaxUseTileCount = useTileCount;
				return;
			}

			while (((value >> shift) & 0xF) % 5 == 0) shift += 4;
			Get(value, shift, out int continuous, out int singleCount);

			if (singleCount >= 2) {
				SyantenCut2(Set(value, shift, continuous, singleCount - 2), shift, remCount - 2, C3, C2 + 1, P, args);
			}

			if (continuous == 0) {
				Get(value, shift + 4, out int continuous2, out int singleCount2);
				if (singleCount2 > 0) {
					var temp = Set(value, shift, continuous, singleCount - 1);
					temp = Set(temp, shift + 4, continuous2, singleCount2 - 1);
					SyantenCut2(temp, shift, remCount - 2, C3, C2 + 1, P, args);
				}
				if (continuous2 == 0) {
					Get(value, shift + 8, out int continuous3, out int singleCount3);
					if (singleCount3 > 0) {
						var temp = Set(value, shift, continuous, singleCount - 1);
						temp = Set(temp, shift + 8, continuous3, singleCount3 - 1);
						SyantenCut2(temp, shift, remCount - 2, C3, C2 + 1, P, args);
					}
				}
			} else if (continuous == 1) {
				Get(value, shift + 4, out int continuous3, out int singleCount3);
				if (singleCount3 > 0) {
					var temp = Set(value, shift, continuous, singleCount - 1);
					temp = Set(temp, shift + 4, continuous3, singleCount3 - 1);
					SyantenCut2(temp, shift, remCount - 2, C3, C2 + 1, P, args);
				}
			}

			SyantenCut2(value, shift + 4, remCount - singleCount, C3, C2, P, args);
		}

		static bool RonCutPair(ulong value, int count) {
			for (int shift = 0; (value >> shift) != 0; shift += 4) {
				Get(value, shift, out int continuous, out int singleCount);
				if (singleCount >= 2) {
					if (RonCutPung(Set(value, shift, continuous, singleCount - 2), 0, count - 2)) return true;
				}
			}
			return false;
		}

		static bool RonCutPung(ulong value, int shift, int count) {
			if (count == 0) return true;

			while ((value >> shift) != 0 && ((value >> shift) & 0xF) % 5 == 0) shift += 4;
			if ((value >> shift) == 0) {
				return RonCutJunko(value, 0, count);
			}

			Get(value, shift, out int continuous, out int singleCount);
			if (singleCount >= 3) {
				var temp = Set(value, shift, continuous, singleCount - 3);
				if (RonCutPung(temp, shift, count - 3)) return true;
			}

			return RonCutPung(value, shift + 4, count);
		}

		static bool RonCutJunko(ulong value, int shift, int count) {
			if (count == 0) return true;

			while ((value >> shift) != 0 && ((value >> shift) & 0xF) % 5 == 0) shift += 4;

			Get(value, shift, out int continuous1, out int singleCount1);
			if (continuous1 == 0) {
				Get(value, shift + 4, out int continuous2, out int singleCount2);
				if (continuous2 == 0 && singleCount2 > 0) {
					Get(value, shift + 8, out int continuous3, out int singleCount3);
					if (singleCount3 > 0) {
						var temp = Set(value, shift, continuous1, singleCount1 - 1);
						temp = Set(temp, shift + 4, continuous2, singleCount2 - 1);
						temp = Set(temp, shift + 8, continuous3, singleCount3 - 1);
						return RonCutJunko(temp, shift, count - 3);
					}
				}
			}

			return false;
		}

		public static Info GetInfo(ulong value) {
			byte remCount = 0;
			int shift = 0;
			ulong newValue = 0;
			while (((value >> shift) & 0xF) != 0xF) {
				var continuous = ((value >> (shift + 2)) & 3);
				var count = ((value >> shift) & 3) + 1;
				remCount += (byte) count;
				newValue |= (continuous * 5 + count) << shift;
				shift += 4;
			}

			if (RonCutPair(newValue, remCount)) {
				return new Info { Syanten = -1, Analysis = Analysis(newValue) };
			}
			var args = new SyantenArgs { N = remCount / 3 };
			SyantenCutPair(newValue, remCount, args);
			return new Info { Syanten = args.Result };
		}

		public static ulong Rebuild(ulong value) {
			cache.Clear();

			int shift = 0;
			while (value != 0xF) {
				if (((value >> shift) & 0b1000) == 0) {
					shift += 4;
					continue;
				}
				shift += 4;
				cache.Add(new RebuildItem(value & ((1UL << shift) - 1), shift));
				value >>= shift;
				shift = 0;
			}
			cache.Sort(RebuildItem.Comparer);
			var count = cache.Count;
			for (int i = 0; i < count; i++) {
				value = (value << cache[i].Length) | cache[i].Value;
			}
			return value;
		}

		public static void EnumTiles(int N, HashSet<ulong> result) {
			foreach (var val in EnumValue(N)) {
				var value = Rebuild(val);
				var success = result.Add(value);
				if (success && result.Count % 100000 == 0)
					Console.WriteLine($"N={N}, 已生成数量：{result.Count}");
			}
			Console.WriteLine($"N={N}, 已生成数量：{result.Count}");
		}

		unsafe static void WriteInt(FileStream fileStream, uint value, int bytes, byte[] cache) {
			fixed (byte* p = cache) {
				*(uint*) p = value;
			}
			fileStream.Write(cache, 0, bytes);
		}

		unsafe static void WriteInt(FileStream fileStream, ulong value, int bytes, byte[] cache) {
			fixed (byte* p = cache) {
				*(ulong*) p = value;
			}
			fileStream.Write(cache, 0, bytes);
		}

		static void WriteAnalysis(FileStream file, IReadOnlyList<AnalysisResult> analysis, byte[] cache) {
			for (int i = 0; i < analysis.Count; i++) {
				bool end = i == analysis.Count - 1;
				var arithmetic = analysis[i].GetArithmetic(end, out int bytes);
				WriteInt(file, arithmetic, bytes, cache);
			}
		}

		static void WriteDiff(FileStream file, ulong diff, byte[] cache) {
			if (DiffIndexMap.TryGetValue(diff, out int index)) {
				file.WriteByte((byte) (index << 3));
			} else {
				diff <<= 3;
				int bytes = GetIntBytes(diff);
				if ((bytes & 7) == 0) throw new Exception();
				diff |= (uint) bytes;
				WriteInt(file, diff, bytes, cache);
			}
		}

		/*
			-1 Syanten: [count:4 bytes][arithmetic(0):8 bytes][infos...:1~4 bytes][diff(1):1~7 bytes or 1 byte index][infos...][diffs[infos...]...]
			 0 Syanten: [count:4 bytes][arithmetic(0):8 bytes][diff(1):1~7 bytes or 1 byte index][diffs...]
			 1 Syanten: ...
			 ...
			 8 Syanten: ...
		*/
		public static void Write(string filename) {
			var table = new HashSet<ulong>();
			for (int N = 2; N <= 14; N += 3)
				EnumTiles(N, table);

			var ronDict = new Dictionary<ulong, IReadOnlyList<AnalysisResult>>();
			var lists = new List<ulong>[9];
			for (int i = 0; i < lists.Length; i++) lists[i] = new List<ulong>();

			foreach (var value in table) {
				var info = GetInfo(value);
				if (info.Syanten == -1) {
					if (info.Analysis.Count == 0) {
						Console.WriteLine(Str(value));
						throw new Exception("BUG!!!");
					}
					ronDict.Add(Arithmetic(value), info.Analysis);
				} else {
					lists[info.Syanten].Add(Arithmetic(value));
				}
			}

			var sortedRonDict = new SortedDictionary<ulong, IReadOnlyList<AnalysisResult>>(ronDict);
			for (int i = 0; i < lists.Length; i++) lists[i].Sort();

			byte[] cache = new byte[8];
			using (var file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new BinaryWriter(file)) {
				writer.Write(sortedRonDict.Count);
				using (var enumer = sortedRonDict.GetEnumerator()) {
					enumer.MoveNext();
					ulong prev = enumer.Current.Key;
					writer.Write(prev);
					WriteAnalysis(file, enumer.Current.Value, cache);

					while (enumer.MoveNext()) {
						var (arithmetic, analysis) = (enumer.Current.Key, enumer.Current.Value);
						var diff = (arithmetic - prev);
						WriteDiff(file, diff, cache);
						WriteAnalysis(file, enumer.Current.Value, cache);
						prev = arithmetic;
					}
				}

				for (int i = 0; i < lists.Length; i++) {
					Console.WriteLine($"写入向听数表: {i}");
					var list = lists[i];
					writer.Write(list.Count);
					using (var enumer = list.GetEnumerator()) {
						enumer.MoveNext();
						ulong prev = enumer.Current;
						writer.Write(prev);

						while (enumer.MoveNext()) {
							var arithmetic = enumer.Current;
							var diff = (arithmetic - prev);
							WriteDiff(file, diff, cache);
							prev = arithmetic;
						}
					}
				}
			}
		}

		unsafe static AnalysisResult ReadAnalysis(FileStream fileStream, byte[] cache, out bool end) {
			fixed (byte* p = cache) {
				*(uint*) p = 0;
				var peek = fileStream.ReadByte();
				end = (peek & 1) == 0;
				int bytes = ((peek >> 1) & 3) + 1;
				cache[0] = (byte) peek;
				if (bytes > 1) fileStream.Read(cache, 1, bytes - 1);
				return AnalysisResult.Build((int) (*(uint*) p >> 3));
			}
		}

		public static ulong ArithmeticToUInt64(ulong arithmetic) {
			ulong value = 0;
			int shift = 0;
			for (; arithmetic != 0; shift += 4) {
				arithmetic = (ulong) Math.DivRem((long) arithmetic, 12, out long singlePair);
				value |= (ulong) singlePair << shift;
			}
			value |= 0xFUL << shift;
			return value;
		}

		unsafe static ulong ReadDiff(FileStream file, byte[] cache) {
			var peek = file.ReadByte();
			int bytes = peek & 7;
			if (bytes == 0) {
				int index = peek >> 3;
				return DiffTables[index];
			} else {
				fixed (byte* p = cache) {
					*(ulong*) p = 0;
					cache[0] = (byte) peek;
					if (bytes > 1) file.Read(cache, 1, bytes - 1);
					return *(ulong*) p >> 3;
				}
			}
		}

		public static IEnumerable<(ulong Value, Info info)> Read(string filename) {
			byte[] cache = new byte[8];
			using (var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new BinaryReader(file)) {
				var ronCount = reader.ReadInt32();
				ulong arithmetic = reader.ReadUInt64();
				var analysisList = new List<AnalysisResult>();
				while (true) {
					analysisList.Add(ReadAnalysis(file, cache, out bool end));
					if (end) break;
				}
				yield return (ArithmeticToUInt64(arithmetic), new Info { Syanten = -1, Analysis = analysisList.ToArray() });

				for (int i = 1; i < ronCount; i++) {
					arithmetic += ReadDiff(file, cache);
					analysisList.Clear();
					while (true) {
						analysisList.Add(ReadAnalysis(file, cache, out bool end));
						if (end) break;
					}
					yield return (ArithmeticToUInt64(arithmetic), new Info { Syanten = -1, Analysis = analysisList.ToArray() });
				}

				for (int syanten = 0; syanten <= 8; syanten++) {
					var count = reader.ReadInt32();
					arithmetic = reader.ReadUInt64();
					yield return (ArithmeticToUInt64(arithmetic), new Info { Syanten = syanten });

					for (int i = 1; i < count; i++) {
						arithmetic += ReadDiff(file, cache);
						yield return (ArithmeticToUInt64(arithmetic), new Info { Syanten = syanten });
					}
				}
			}
		}

		public static void Write2(string filename) {
			void GetDiff(Dictionary<ulong, int> result, IEnumerable<ulong> values) {
				using (var enumer = values.GetEnumerator()) {
					enumer.MoveNext();
					ulong prev = (enumer.Current);
					while (enumer.MoveNext()) {
						ulong curr = (enumer.Current);
						ulong diff = curr - prev;
						if (result.TryGetValue(diff, out int count)) {
							result[diff] = count + 1;
						} else {
							result[diff] = 1;
						}
						prev = curr;
					}
				}
			}

			var table = new HashSet<ulong>();
			for (int N = 2; N <= 14; N += 3)
				EnumTiles(N, table);

			var ronDict = new Dictionary<ulong, IReadOnlyList<AnalysisResult>>();
			var lists = new List<ulong>[9];
			for (int i = 0; i < lists.Length; i++) lists[i] = new List<ulong>();

			Console.WriteLine("开始计算向听数");
			foreach (var value in table) {
				var info = GetInfo(value);
				if (info.Syanten == -1) {
					if (info.Analysis.Count == 0) {
						Console.WriteLine(Str(value));
						throw new Exception("BUG!!!");
					}
					ronDict.Add(value, info.Analysis);
				} else {
					lists[info.Syanten].Add(value);
				}
			}

			Console.WriteLine("开始进行差分编码与哈夫曼编码");
			var diffTable = new Dictionary<ulong, int>();
			var sortedRonDict = new SortedDictionary<ulong, IReadOnlyList<AnalysisResult>>(ronDict);
			GetDiff(diffTable, sortedRonDict.Keys);
			if (diffTable.Values.Sum() != sortedRonDict.Count - 1) throw new InvalidOperationException($"{diffTable.Values.Sum()} != {sortedRonDict.Count - 1}");

			for (int i = 0; i < lists.Length; i++) {
				lists[i].Sort();
				GetDiff(diffTable, lists[i]);
			}

			using (var file = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			using (var writer = new HuffmanWriter<ulong>(file, diffTable.Select(kv => (kv.Key, Count: (long) kv.Value)))) {
				using (var enumer = sortedRonDict.Keys.GetEnumerator()) {
					enumer.MoveNext();
					ulong prev = enumer.Current;
					writer.Writer.Write(prev);
					using (var handler = writer.BeginWrite(sortedRonDict.Count - 1)) {
						while (enumer.MoveNext()) {
							ulong curr = enumer.Current;
							handler.Write(curr - prev);
							prev = curr;
						}
					}
				}

				byte[] cache = new byte[4];
				foreach (var info in sortedRonDict.Values) {
					WriteAnalysis(file, info, cache);
				}

				for (int i = 0; i <= 8; i++) {
					var list = lists[i];
					ulong prev = list[0];
					writer.Writer.Write(prev);
					using (var handler = writer.BeginWrite(list.Count - 1)) {
						for (int j = 1; j < list.Count; j++) {
							ulong curr = list[j];
							handler.Write(curr - prev);
							prev = curr;
						}
					}
				}
			}

			Console.WriteLine("写入完成");
		}

		public static IEnumerable<(ulong Value, Info Info)> Read2(string filename) {
			using (var file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new HuffmanReader<ulong>(file)) {
				var ronList = new List<ulong>(6000);

				var val = reader.Reader.ReadUInt64();
				ronList.Add(val);
				foreach (var diff in reader.ReadValues()) {
					ronList.Add(val += diff);
				}

				byte[] cache = new byte[4];
				var analysisList = new List<AnalysisResult>();
				for (int i = 0; i < ronList.Count; i++) {
					analysisList.Clear();
					while (true) {
						analysisList.Add(ReadAnalysis(file, cache, out var end));
						if (end) break;
					}
					yield return (ronList[i], new Info { Syanten = -1, Analysis = analysisList.ToArray() });
				}

				for (int syanten = 0; syanten <= 8; syanten++) {
					val = reader.Reader.ReadUInt64();
					yield return (val, new Info { Syanten = syanten });
					foreach (var diff in reader.ReadValues()) {
						yield return (val += diff, new Info { Syanten = syanten });
					}
				}
			}
		}
	}
}
