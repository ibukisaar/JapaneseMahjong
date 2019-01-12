using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public sealed class HuffmanReader<T> : IDisposable where T : unmanaged {
		private interface INode { }

		private sealed class Terminal : INode {
			public T Value;
			public int Bits;

			public override string ToString()
				=> $"{Value}, Bits:{Bits}";
		}

		private sealed class Node : INode {
			public INode Child0;
			public INode Child1;
			public int TerminalCount;
			public int Depth;
		}

		private sealed class NodeArray : INode {
			public int Bits;
			public INode[] Nodes;

			public NodeArray(int bits) {
				Bits = bits;
				Nodes = new INode[1 << bits];
			}
		}

		private class FileCache {
			private BinaryReader reader;
			private byte[] buffer = new byte[4];
			private int dataLength = 0;

			public FileCache(BinaryReader reader) {
				this.reader = reader;
			}

			public byte ReadByte(int index) {
				if (index >= dataLength) {
					if (index >= buffer.Length) {
						var newBufferLength = Math.Max(buffer.Length * 2, index + 1);
						Array.Resize(ref buffer, newBufferLength);
					}

					var copyCount = index + 1 - dataLength;
					reader.Read(buffer, dataLength, copyCount);
					dataLength = index + 1;
				}
				return buffer[index];
			}
		}

		private static readonly int KeyBytes = Marshal.SizeOf<T>();

		private const int ThresholdBits = 14;

		private Stream stream;
		private BinaryReader reader;
		private FileCache fileCache;
		private NodeArray nodeArray;

		public BinaryReader Reader => reader;

		public HuffmanReader(Stream stream) {
			if (!stream.CanSeek) throw new InvalidOperationException("只支持能重定向指针的流。");

			this.stream = stream;
			reader = new BinaryReader(stream);
			fileCache = new FileCache(reader);

			CreateTable();
		}

		unsafe private static T ReadKeyFromBuffer(byte[] buffer) {
			fixed (byte* p = buffer) {
				return *(T*)p;
			}
		}

		private byte ReadTreePath(int index) {
			return (byte)((fileCache.ReadByte(index >> 2) >> ((index & 3) << 1)) & 3);
		}

		private void BuildTree(List<Terminal> terminals, ref int index, out INode node) {
			var newNode = new Node();
			node = newNode;
			var path = ReadTreePath(index++);
			var (leftCount, leftDepth) = SetNode(terminals, ref index, ref newNode.Child0, (path & 1) != 0);
			var (rightCount, rightDepth) = SetNode(terminals, ref index, ref newNode.Child1, (path & 2) != 0);
			newNode.TerminalCount = leftCount + rightCount;
			newNode.Depth = Math.Max(leftDepth + 1, rightDepth + 1);
		}

		private (int TerminalCount, int Depth) SetNode(List<Terminal> terminals, ref int index, ref INode node, bool isNonTerminal) {
			if (isNonTerminal) {
				BuildTree(terminals, ref index, out node);
				return ((node as Node).TerminalCount, (node as Node).Depth);
			} else {
				var terminal = new Terminal();
				terminals.Add(terminal);
				node = terminal;
				return (1, 0);
			}
		}

		private void FillBinary(List<uint> binaries, INode node, List<bool> binary) {
			switch (node) {
				case Node n:
					binary.Add(false);
					FillBinary(binaries, n.Child0, binary);
					binary[binary.Count - 1] = true;
					FillBinary(binaries, n.Child1, binary);
					binary.RemoveAt(binary.Count - 1);
					return;
				case Terminal t:
					uint value = 0;
					for (int shift = 0; shift < binary.Count; shift++) {
						if (binary[shift]) {
							value |= 1u << shift;
						}
					}
					binaries.Add(value);
					return;
			}
		}

		const int MaxArrayBits = 8;

		private static void BuildArrayTree(NodeArray nodeArray, INode node, int binary, int bits) {
			switch (node) {
				case Node n:
					if (bits < nodeArray.Bits) {
						BuildArrayTree(nodeArray, n.Child0, binary, bits + 1);
						BuildArrayTree(nodeArray, n.Child1, binary | (1 << bits), bits + 1);
					} else {
						nodeArray.Nodes[binary] = BuildArrayTree(n);
					}
					break;
				case Terminal t:
					t.Bits = bits;
					int paddingCount = 1 << (nodeArray.Bits - bits);
					for (int padding = 0; padding < paddingCount; padding++) {
						nodeArray.Nodes[(padding << bits) | binary] = node;
					}
					break;
			}
		}

		private static NodeArray BuildArrayTree(Node node) {
			NodeArray result;
			if (node.Depth <= MaxArrayBits) {
				result = new NodeArray(node.Depth);
			} else {
				result = new NodeArray(Math.Max((int)Math.Log(node.TerminalCount, 2), MaxArrayBits));
			}
			BuildArrayTree(result, node, 0, 0);
			return result;
		}

		private void CreateTable() {
			List<Terminal> terminals = new List<Terminal>();
			int index = 0;
			BuildTree(terminals, ref index, out var root);

			var keyBuffer = new byte[KeyBytes];
			for (int i = 0; i < terminals.Count; i++) {
				reader.Read(keyBuffer, 0, KeyBytes);
				terminals[i].Value = ReadKeyFromBuffer(keyBuffer);
			}

			nodeArray = BuildArrayTree(root as Node);
		}

		public IEnumerable<T> ReadValues() {
			return new Enumerator(this);
		}

		public void Dispose() {
			reader.Dispose();
			GC.SuppressFinalize(this);
		}

		~HuffmanReader() {
			Dispose();
		}

		private sealed class Enumerator : IEnumerable<T> {
			private HuffmanReader<T> reader;

			public int Count { get; }

			public Enumerator(HuffmanReader<T> reader) {
				this.reader = reader;
				Count = reader.reader.ReadInt32();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void FillBuffer(ref ulong buffer, ref int bits, int neededBits) {
				for (; bits < neededBits; bits += 8) {
					int b = reader.stream.ReadByte();
					if (b < 0) break;
					buffer |= (ulong)b << bits;
				}
			}

			private T Read(NodeArray nodeArray, ref ulong buffer, ref int bits) {
				FillBuffer(ref buffer, ref bits, nodeArray.Bits);
				int index = (int)buffer & (nodeArray.Nodes.Length - 1);
				switch (nodeArray.Nodes[index]) {
					case Terminal t:
						bits -= t.Bits;
						buffer >>= t.Bits;
						return t.Value;
					case NodeArray array:
						bits -= nodeArray.Bits;
						buffer >>= nodeArray.Bits;
						return Read(array, ref buffer, ref bits);
					default: throw new InvalidOperationException();
				}
			}

			public IEnumerator<T> GetEnumerator() {
				ulong buffer = 0;
				int bits = 0;
				for (int i = 0; i < Count; i++) {
					yield return Read(reader.nodeArray, ref buffer, ref bits);
				}
				if (bits >= 8) reader.stream.Seek(-(bits / 8), SeekOrigin.Current);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
	}
}
