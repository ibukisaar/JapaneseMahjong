using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public static class Huffman<T> {
		private abstract class IWeight {
			public abstract long Weight { get; }
		}

		private class Node : IWeight {
			public override long Weight { get; }
			public IWeight Left { get; }
			public IWeight Right { get; }

			public Node(IWeight left, IWeight right) {
				Weight = left.Weight + right.Weight;
				Left = left;
				Right = right;
			}
		}

		private class Terminal : IWeight {
			public T Value { get; }
			public override long Weight { get; }

			public Terminal(T value, long weight) {
				Value = value;
				Weight = weight;
			}
		}

		public static List<(T Key, long Count, string Binary)> Build(IEnumerable<(T Key, long Count)> keys) {
			var nodes = new SortedDictionary<long, List<IWeight>>();

			void AddNode(IWeight node) {
				if (nodes.TryGetValue(node.Weight, out var list)) {
					list.Add(node);
				} else {
					nodes.Add(node.Weight, new List<IWeight> { node });
				}
			}

			void RemoveNode(IWeight node) {
				if (nodes.TryGetValue(node.Weight, out var list)) {
					if (list.Count >= 2) {
						list.Remove(node);
					} else {
						nodes.Remove(node.Weight);
					}
				}
			}

			foreach (var key in keys) {
				AddNode(new Terminal(key.Key, key.Count));
			}

			while (true) {
				IWeight left, right;
				using (var enumer = nodes.GetEnumerator()) {
					enumer.MoveNext();
					var list = enumer.Current.Value;
					left = list[0];
					if (list.Count >= 2) {
						right = list[1];
					} else {
						if (enumer.MoveNext()) {
							right = enumer.Current.Value[0];
						} else {
							break;
						}
					}
				}

				RemoveNode(left);
				RemoveNode(right);
				AddNode(new Node(left, right));
			}

			var root = nodes.First().Value[0];
			var result = new List<(T Key, long Count, string Binary)>();

			void Output(IWeight node, List<char> binary) {
				switch (node) {
					case Terminal t:
						result.Add((t.Value, t.Weight, new string(binary.ToArray())));
						return;
					case Node n:
						binary.Add('0');
						Output(n.Left, binary);
						binary[binary.Count - 1] = '1';
						Output(n.Right, binary);
						binary.RemoveAt(binary.Count - 1);
						return;
					default: throw new InvalidOperationException();
				}
			}

			Output(root, new List<char>(16));
			return result;
		}

		public static (long Bits, long Count, int MinBits, int MaxBits) GetInfo(IReadOnlyList<(T Key, long Count, string Binary)> table) {
			long bits = 0;
			long totalCount = 0;
			int maxBits = 0, minBits = int.MaxValue;
			foreach (var (_, count, binary) in table) {
				var length = binary.Length;
				bits += length * count;
				totalCount += count;
				maxBits = Math.Max(maxBits, length);
				minBits = Math.Min(minBits, length);
			}
			return (bits, totalCount, minBits, maxBits);
		}
	}
}
