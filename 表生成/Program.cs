using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Numerics;
using 日本麻将.表生成;

namespace 表生成 {
	class Program {
		static bool Valid(ulong value) => ((value >> 2) & ((value >> 1) | value) & 0x1111111111111111UL) == 0;

		static ulong Add(ulong x, ulong y) {
			ulong or = (x | y) & 0x8888888888888888UL;
			ulong sum = (x & 0x7777777777777777UL) + (y & 0x7777777777777777UL);
			return sum | or;
		}

		static ulong InsertSpace(ulong value, int bitIndex, int bits) {
			ulong mask = 0xFFFFFFFFFFFFFFFFUL << bitIndex;
			return ((value & mask) << bits) | (value & ~mask);
		}

		static void AppendJunko(HashSet<ulong> map, ulong oldValue) {
			/*
			                 .... ?x`1 $
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			  .... ?x`1 e001 1001 1001 $
			       .... ?y`1 1001 1001 $


			            .... ?x`2 0x`1 $
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			  .... ?x`2 e001 1y`1 1001 $


			            .... ?x`2 1x`1 $
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			       .... ?y`2 1y`1 1001 $
			

			       .... ?x`3 1x`2 1x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			       .... ?y`3 1y`2 1y`1 ....


			            .... ?x`2 0x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			       .... ?y`2 1001 1y`1 ....
			  .... ?x`2 e001 1001 1y`1 ....
			  .... ?y`2 1001 1001 ex`1 ....
		 .... ?x`2 e001 1001 1001 ex`1 ....
		 

			       .... ?x`3 0x`2 1x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			  .... ?x`3 e001 1y`2 1y`1 ....
			  

			       .... ?x`3 0x`2 0x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
		 .... ?x`3 e001 1y`2 1001 ey`1 ....


			       .... ?x`3 1x`2 0x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			  .... ?x`3 1y`2 1001 ey`1 ....


			                    ^ 0x`1 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			     ^ 0001 1001 1001 ex`1 ....
			          ^ 0001 1001 1y`1 ....


			               ^ 0x`1 0x`2 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			     ^ 0001 1y`1 1001 ex`2 ....


			               ^ 0x`1 ?x`2 ....
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			          ^ 0001 1y`1 1y`2 ....


			                    ^ 0x`1 $
			          + 0001 1001 1001
			  ---- ---- ---- ---- ----
			          ^ 0001 1y`1 1001 $
			 */

			const ulong mask = 0xFFF;
			const ulong junko = 0b0001_1001_1001;
			ulong newValue;

			newValue = (oldValue << 12) | junko;
			map.Add(newValue);
			newValue |= 0x800;
			map.Add(newValue);

			newValue = Add(oldValue << 8, junko);
			if (Valid(newValue)) { map.Add(newValue); }

			if ((oldValue & 0x70) != 0) {
				if ((oldValue & 8) == 0) {
					newValue = Add(InsertSpace(oldValue, 4, 4) << 4, junko);
					if (Valid(newValue)) { map.Add(newValue); }
					newValue |= 0x800;
					if (Valid(newValue)) { map.Add(newValue); }
				} else {
					newValue = Add(oldValue << 4, junko);
					if (Valid(newValue)) { map.Add(newValue); }
				}
			}

			int shift = 0;
			for (; (oldValue & (mask << shift)) != 0; shift += 4) {
				if ((oldValue & (0x70UL << shift)) != 0 && (oldValue & (0x700UL << shift)) != 0) {
					switch ((oldValue >> shift) & 0x88UL) {
						case 0x88:
							newValue = Add(oldValue, junko << shift);
							if (Valid(newValue)) { map.Add(newValue); }
							break;
						case 0x08:
							newValue = Add(InsertSpace(oldValue, shift + 8, 4), junko << shift);
							if (Valid(newValue)) { map.Add(newValue); }
							newValue |= 0x800UL << shift;
							if (Valid(newValue)) { map.Add(newValue); }
							break;
						case 0x00:
							newValue = Add(InsertSpace(InsertSpace(oldValue, shift + 4, 4), shift + 12, 4), junko << (shift + 4));
							if (Valid(newValue)) { map.Add(newValue); }
							var newValue2 = newValue | (8UL << shift);
							if (Valid(newValue2)) { map.Add(newValue2); }
							newValue |= 0x8000UL << shift;
							if (Valid(newValue)) { map.Add(newValue); }
							newValue |= 8UL << shift;
							if (Valid(newValue)) { map.Add(newValue); }
							break;
						case 0x80:
							newValue = Add(InsertSpace(oldValue, shift + 4, 4), junko << shift);
							if (Valid(newValue)) { map.Add(newValue); }
							newValue |= 8UL << shift;
							if (Valid(newValue)) { map.Add(newValue); }
							break;
					}
				}

				if ((oldValue & (0x70UL << shift)) != 0 && (oldValue & (8UL << shift)) == 0) {
					newValue = Add(InsertSpace(oldValue, shift + 4, 4), junko << shift);
					if (Valid(newValue)) { map.Add(newValue); }

					newValue = Add(InsertSpace(oldValue, shift + 4, 8), junko << shift);
					if (Valid(newValue)) { map.Add(newValue); }
					newValue |= 0x800UL << shift;
					if (Valid(newValue)) { map.Add(newValue); }

					newValue = Add(InsertSpace(oldValue, shift + 4, 8), junko << (shift + 4));
					if (Valid(newValue)) { map.Add(newValue); }
					newValue |= 0x8UL << shift;
					if (Valid(newValue)) { map.Add(newValue); }

					newValue = InsertSpace(oldValue, shift + 4, 12) | (junko << (shift + 4));
					map.Add(newValue);
					var newValue2 = newValue | (0x8UL << shift);
					map.Add(newValue2);
					newValue |= 0x8000UL << shift;
					map.Add(newValue);
					newValue |= 0x8UL << shift;
					map.Add(newValue);
				}
			}

			shift -= 4;
			newValue = oldValue | (junko << (shift + 4));
			map.Add(newValue);
			newValue |= 0x8UL << shift;
			map.Add(newValue);

			newValue = Add(oldValue, junko << shift);
			if (Valid(newValue)) { map.Add(newValue); }

			if (shift > 0) {
				shift -= 4;

				newValue = Add(oldValue, (junko << shift));
				if (Valid(newValue)) { map.Add(newValue); }

				if ((oldValue & (0x8UL << shift)) == 0) {
					newValue = Add(InsertSpace(oldValue, shift + 4, 4), junko << (shift + 4));
					if (Valid(newValue)) { map.Add(newValue); }
					newValue |= 8UL << shift;
					if (Valid(newValue)) { map.Add(newValue); }
				}
			}

			if ((oldValue & 0x70) == 0) {
				newValue = Add(oldValue << 4, junko);
				if (Valid(newValue)) { map.Add(newValue); }
			}
		}

		static void AppendPung(HashSet<ulong> map, ulong oldValue) {
			/*
			                 .... ?x`1 $
			                    + 0003
			  ---- ---- ---- ---- ----
			            .... ?x`1 e003 $
			 
			  
			                 .... ?x`1 ....
			                    + 0003
			  ---- ---- ---- ---- ----
			                 .... ?y`1 ....


			            .... ?x`2 0x`1 ....
			                    + 0003
			  ---- ---- ---- ---- ----
			       .... ?x`2 e003 ex`1 ....


			                    ^ 0x`1 ....
			                    + 0003
			  ---- ---- ---- ---- ----
			               ^ 0003 ex`1 ....
			*/


			const ulong mask = 0xF;
			const ulong pung = 3UL;
			ulong newValue;

			newValue = (oldValue << 4) | pung;
			map.Add(newValue);
			newValue |= 8UL;
			map.Add(newValue);

			int shift = 0;
			for (; (oldValue & (mask << shift)) != 0; shift += 4) {
				newValue = Add(oldValue, pung << shift);
				if (Valid(newValue)) { map.Add(newValue); }

				if ((oldValue & (0x70UL << shift)) != 0 && (oldValue & (8UL << shift)) == 0) {
					newValue = InsertSpace(oldValue, shift + 4, 4) | (pung << (shift + 4));
					map.Add(newValue);
					var newValue2 = newValue | (8UL << shift);
					map.Add(newValue2);
					newValue |= 0x80UL << shift;
					map.Add(newValue);
					newValue |= 0x8UL << shift;
					map.Add(newValue);
				}
			}

			shift -= 4;
			newValue = oldValue | (pung << (shift + 4));
			map.Add(newValue);
			newValue |= 8UL << shift;
			map.Add(newValue);
		}

		static HashSet<ulong> Create(int N) {
			var result = new HashSet<ulong>();
			for (int junko = 0; junko <= N; junko++) {
				var map = new HashSet<ulong>() { 2/*pair*/ };
				for (int i = 0; i < N; i++) {
					var tmp = new HashSet<ulong>();
					foreach (var value in map) {
						if (i < junko)
							AppendJunko(tmp, value);
						else
							AppendPung(tmp, value);
					}
					foreach (var value in tmp) map.Add(value);
				}
				foreach (var value in map) result.Add(value);
			}
			return result;
		}

		static string Str(ulong value) {
			var buffer = new StringBuilder();
			const ulong mask = 0xFUL;
			bool pass = false;
			for (int shift = 60; shift >= 0; shift -= 4) {
				if (!pass && (value & (mask << shift)) != 0) pass = true;
				if (pass) {
					buffer.Append($"{(((value >> shift) & 0x8) == 0 ? " * " : " ")}{(char) (((value >> shift) & 7) + '0')}");
				}
			}
			return buffer.ToString();
		}

		static bool Ron(ulong value, bool hasPair = false) {
			while (value != 0 && (value & 7) == 0) value >>= 4;
			if (value == 0) return hasPair;

			if ((value & 7) >= 2) {
				if (!hasPair) {
					if (Ron(value - 2, true)) return true;
				}
				if ((value & 7) >= 3) {
					if (Ron(value - 3, hasPair)) return true;
				}
			}
			if ((value & 0x70) != 0 && (value & 0x700) != 0 && (value & 0x8) != 0 && (value & 0x80) != 0) {
				if (((value >> 4) & 7) == 1) value &= ~0x80UL;
				if (((value >> 8) & 7) == 1) value &= ~0x800UL;
				if (Ron(value - 0x111, hasPair)) return true;
			}

			return false;
		}

		unsafe static void Main(string[] args) {
			// WriteTable(@"Z:\mahjong.table");

			//var dict = new Dictionary<ulong, int>(4104364);
			//foreach (var (value, syanten) in ReadTable(@"Z:\mahjong.table")) {
			//	dict.Add(value, (sbyte) syanten);
			//}

			//var field = dict.GetType().GetField("buckets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			//var val = field.GetValue(dict) as int[];



			//const int N = 1674319;
			//int[] counts = new int[N];
			//ulong min = ulong.MaxValue, max = ulong.MinValue;
			//foreach (var value in ReadOriginalTable(@"Z:\mahjong.table")) {
			//	counts[(((value / 10)) ^ 0xC0FFEE_EEFF0CUL) % N]++;
			//}

			//int maxCnt = int.MinValue;
			//for (int i = 0; i < N; i++) {
			//	maxCnt = Math.Max(maxCnt, counts[i]);
			//}
			//Console.WriteLine($"{maxCnt}");

			//var list = new List<ulong>(4104364);
			//foreach (var value in ReadOriginalTable(@"Z:\mahjong.table")) {
			//	list.Add(value / 10);
			//}
			//list.Sort();

			//SortedDictionary<ulong, int> diffDict = new SortedDictionary<ulong, int>();
			//for (int i = 1; i < list.Count; i++) {
			//	if (diffDict.TryGetValue(list[i] - list[i - 1], out var value)) {
			//		diffDict[list[i] - list[i - 1]] = value + 1;
			//	} else {
			//		diffDict[list[i] - list[i - 1]] = 1;
			//	}
			//}

			//var diffs = diffDict.OrderByDescending(kv => kv.Value).ToArray();
			//var diffList = new List<KeyValuePair<ulong, int>>();
			//int count = 0;
			//for (int i = 0, j = 0; i < 32; j++) {
			//	if (diffs[j].Key >= 32) { diffList.Add(diffs[j]); i++; }
			//	count += diffs[j].Value;
			//}
			//Console.WriteLine(count);
			//int index = 0;
			//foreach (var d in diffList) {
			//	Console.Write($"{{ 0x{d.Key:X}, {index++} }},");
			//}


			//while (true) ;

			//foreach (var (value, syanten) in ReadTable(@"Z:\mahjong.table")) {
			//	var otherValue = Flip(value, GetLength(value));
			//	var syanten2 = Syanten(otherValue);
			//	if (syanten2 != syanten) {
			//		Console.WriteLine(Str2(value));
			//		Console.WriteLine(Str2(otherValue));
			//		Console.WriteLine(syanten);
			//		Console.WriteLine(syanten2);
			//		syanten2 = Syanten(otherValue);
			//		break;
			//	}
			//}

			// WriteTable(@"Z:\mahjong.table");
			//var dict = new Dictionary<ulong, int>(1292059); // 1292059
			//foreach (var (value, syanten) in ReadTable(@"Z:\mahjong.table")) {
			//	dict.Add(value, syanten);
			//}

			//var field = dict.GetType().GetField("buckets", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			//var val = field.GetValue(dict) as int[];

			//var table = new HashSet<ulong>();
			//EnumTiles(14, table);

			//foreach (var value in table) {
			//	var info = GetInfo(value);
			//	if (info.Syanten == -1) {
			//		Console.WriteLine(Str2(value));
			//		Console.WriteLine(string.Join(" ; ", info.Analysis));
			//	}
			//}


			// WriteTable2(@"Z:\mahjong.table");
			//Console.WriteLine(Str2(0x0000000fa0000305));
			//Console.WriteLine(string.Join(", ", GetInfo(0x0000000fa0000305).Analysis));
			//var a = GetInfo(0x0000000fa0000305).Analysis[0];
			//uint val = a.GetArithmetic(true, out int bytes);
			//var ana = AnalysisResult.Build((int) (val >> 3));



			//void GetDiff(Dictionary<ulong, int> result, List<ulong> values) {
			//	ulong prev = values[0];
			//	int length = values.Count;
			//	for (int i = 1; i < length; i++) {
			//		ulong diff = values[i] - prev;
			//		if (result.TryGetValue(diff, out int count)) {
			//			result[diff] = count + 1;
			//		} else {
			//			result[diff] = 1;
			//		}
			//		prev = values[i];
			//	}
			//}

			//var hash = new HashSet<ulong>();
			//for (int n = 2; n <= 14; n += 3) {
			//	MahjongTable.EnumTiles(n, hash);
			//}

			//var lists = new List<ulong>[10];
			//for (int i = 0; i < lists.Length; i++) lists[i] = new List<ulong>();

			//foreach (var value in hash) {
			//	var info = MahjongTable.GetInfo(value);
			//	lists[info.Syanten + 1].Add(value);
			//}

			//Dictionary<ulong, int> diffDict = new Dictionary<ulong, int>();
			//for (int i = 0; i < lists.Length; i++) {
			//	lists[i].Sort();
			//	GetDiff(diffDict, lists[i]);
			//}

			//var alists = new List<ulong>[10];
			//for (int i = 0; i < alists.Length; i++) {
			//	alists[i] = lists[i].ConvertAll(val => MahjongTable.Arithmetic(val));
			//}

			//Dictionary<ulong, int> diffDict2 = new Dictionary<ulong, int>();
			//for (int i = 0; i < alists.Length; i++) {
			//	alists[i].Sort();
			//	GetDiff(diffDict2, alists[i]);
			//}

			//var huffman = Huffman<ulong>.Build(diffDict.Select(kv => (kv.Key, (long) kv.Value)).ToArray());
			//Console.WriteLine(Huffman<ulong>.GetInfo(huffman));
			//var huffman2 = Huffman<ulong>.Build(diffDict2.Select(kv => (kv.Key, (long) kv.Value)).ToArray());
			//Console.WriteLine(Huffman<ulong>.GetInfo(huffman2));

			MahjongTable.Write2(@"mahjong2.table");

			//foreach (var (val, info) in MahjongTable.Read2(@"Z:\mahjong2.table")) {
			//	if (info.Syanten == -1) {
			//		Console.WriteLine(MahjongTable.Str(val));
			//		Console.WriteLine(string.Join("; ", info.Analysis));
			//	}
			//}

			//var values = new List<(ulong Value, long Count)>();
			//for (int i = 1; i < 20; i++) {
			//	values.Add(((ulong) i, i));
			//}

			//var info = Huffman<ulong>.Build(values);
			//foreach (var i in info) {
			//	Console.WriteLine($"{i.Key} => '{i.Binary}'");
			//}

			//byte[] data = null;
			//using (var mem = new MemoryStream()) {
			//	using (var writer = new HuffmanWriter<ulong>(mem, values))
			//	using (var handler = writer.BeginWrite((int) values.Sum(p => p.Count))) {
			//		for (int i = 0; i < values.Count; i++) {
			//			for (int j = 0; j < values[i].Count; j++) {
			//				handler.Write(values[i].Value);
			//			}
			//		}
			//	}
			//	data = mem.ToArray();
			//}

			//using (var mem = new MemoryStream(data))
			//using (var reader = new HuffmanReader<ulong>(mem)) {
			//	foreach (var val in reader.ReadValues()) {
			//		Console.WriteLine(val);
			//	}
			//}
		}
	}
}
