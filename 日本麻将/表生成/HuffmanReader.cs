using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public sealed class HuffmanReader<T> : IDisposable where T : unmanaged {
		private interface INode { }

		private sealed class Terminal : INode {
			public T Value;
			public int Bits;

			public Terminal(T value, int bits) {
				Value = value;
				Bits = bits;
			}

			public override string ToString()
				=> $"{Value}, Bits:{Bits}";
		}

		private sealed class Node : INode {
			public INode Child0;
			public INode Child1;
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
		private int maxBits;
		private INode[] table;
		private FileCache fileCache;
		private INode root = null;

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

		private void BuildTree(List<Terminal> terminals, ref INode node, ref int index, int bits = 0) {
			var newNode = new Node();
			node = newNode;
			var path = ReadTreePath(index++);
			SetNode(terminals, ref index, ref newNode.Child0, bits + 1, (path & 1) != 0);
			SetNode(terminals, ref index, ref newNode.Child1, bits + 1, (path & 2) != 0);
		}

		private void SetNode(List<Terminal> terminals, ref int index, ref INode node, int bits, bool isNonTerminal) {
			if (isNonTerminal) {
				BuildTree(terminals, ref node, ref index, bits);
			} else {
				var terminal = new Terminal(default, bits);
				terminals.Add(terminal);
				node = terminal;
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

		private void CreateTable() {
			List<Terminal> terminals = new List<Terminal>();
			int index = 0;
			BuildTree(terminals, ref root, ref index);

			var keyBuffer = new byte[KeyBytes];
			for (int i = 0; i < terminals.Count; i++) {
				reader.Read(keyBuffer, 0, KeyBytes);
				terminals[i].Value = ReadKeyFromBuffer(keyBuffer);
			}

			var binaries = new List<uint>();
			FillBinary(binaries, root, new List<bool>());

			var tableLength = binaries.Count;
			var table = new (T Key, int Bits, uint Binary)[tableLength];
			maxBits = 0;

			for (int i = 0; i < tableLength; i++) {
				maxBits = Math.Max(maxBits, terminals[i].Bits);
				table[i] = (terminals[i].Value, terminals[i].Bits, binaries[i]);
			}

			int indexesMaxBit = Math.Min(maxBits, ThresholdBits);
			var indexesCount = 1 << indexesMaxBit;
			uint mask = (1U << indexesMaxBit) - 1;
			this.table = new INode[indexesCount];
			for (int i = 0; i < tableLength; i++) {
				var (key, bits, binary) = table[i];
				var item = new Terminal(key, bits);
				if (bits <= indexesMaxBit) {
					int paddingCount = 1 << (indexesMaxBit - bits);
					for (int j = 0; j < paddingCount; j++) {
						this.table[((uint)j << bits) | binary] = item;
					}
				} else {
					ref INode root = ref this.table[binary & mask];
					CreateTree(ref root, binary >> indexesMaxBit, bits - indexesMaxBit, item);
				}
			}
		}

		private static void CreateTree(ref INode root, uint binary, int bits, Terminal value) {
			if (bits == 0) { root = value; return; }

			if (root == null) root = new Node();
			var node = root as Node;
			if ((binary & 1) == 0) {
				CreateTree(ref node.Child0, binary >> 1, bits - 1, value);
			} else {
				CreateTree(ref node.Child1, binary >> 1, bits - 1, value);
			}
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

			private void FillBuffer(ref ulong buffer, ref int bits) {
				if (bits < 24) {
					int b = reader.stream.ReadByte();
					if (b >= 0) { buffer |= (ulong)b << bits; bits += 8; }
					b = reader.stream.ReadByte();
					if (b >= 0) { buffer |= (ulong)b << bits; bits += 8; }
					b = reader.stream.ReadByte();
					if (b >= 0) { buffer |= (ulong)b << bits; bits += 8; }
				}
			}

			private static (T Value, int Bits) GetValue(INode root, uint binary) {
				if (root is Terminal t) { return (t.Value, t.Bits); }

				var node = root as Node;
				if ((binary & 1) == 0) {
					return GetValue(node.Child0, binary >> 1);
				} else {
					return GetValue(node.Child1, binary >> 1);
				}
			}

			public IEnumerator<T> GetEnumerator() {
				ulong buffer = 0;
				int bits = 0;
				int maxBits = Math.Min(reader.maxBits, ThresholdBits);
				uint mask = (1U << maxBits) - 1;
				for (int i = 0; i < Count; i++) {
					FillBuffer(ref buffer, ref bits);
					if (bits == 0) throw new InvalidOperationException();
					var index = buffer & mask;
					var node = reader.table[index];
					if (node is Terminal t) {
						yield return t.Value;
						buffer >>= t.Bits;
						bits -= t.Bits;
					} else {
						var (value, keyBits) = GetValue(node, (uint)(buffer >> maxBits));
						yield return value;
						buffer >>= keyBits;
						bits -= keyBits;
					}
				}
				if (bits >= 8) reader.stream.Seek(-(bits / 8), SeekOrigin.Current);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
	}
}
