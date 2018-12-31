using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public static class Huffman<T> where T : unmanaged {
		internal interface IWeight {
			long Weight { get; }
		}

		internal class Node : IWeight {
			public long Weight { get; }
			public IWeight Left { get; }
			public IWeight Right { get; }

			public Node(IWeight left, IWeight right) {
				Weight = left.Weight + right.Weight;
				Left = left;
				Right = right;
			}
		}

		internal class Terminal : IWeight {
			public T Value { get; }
			public long Weight { get; }

			public Terminal(T value, long weight) {
				Value = value;
				Weight = weight;
			}
		}

		internal static IWeight Build(IEnumerable<(T Key, long Count)> keys) {
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

			foreach (var (key, count) in keys) {
				AddNode(new Terminal(key, count));
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

			return nodes.First().Value[0];
		}

		internal static IReadOnlyList<(T Key, long Count, string Binary)> Build(IWeight root) {
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

			Output(root, new List<char>(24));
			return result;
		}

		internal static byte[] BuildTreeBinary(IWeight root) {
			var treeBinary = new List<bool>();

			void Output(Node node) {
				var left = node.Left as Node;
				var right = node.Right as Node;
				treeBinary.Add(left != null);
				treeBinary.Add(right != null);
				if (left != null) Output(left);
				if (right != null) Output(right);
			}

			Output(root as Node);

			var bitArray = new BitArray(treeBinary.ToArray());
			var result = new byte[(bitArray.Count + 7) >> 3];
			bitArray.CopyTo(result, 0);
			return result;
		}

		public static (long Bits, long Count, int MinBits, int MaxBits) GetInfo(IEnumerable<(T Key, long Count, string Binary)> table) {
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
