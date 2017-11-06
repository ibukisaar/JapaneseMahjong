using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.表生成 {
	public sealed class HuffmanReader<T> : IDisposable where T : struct {
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

		private static readonly int KeyBytes = Marshal.SizeOf<T>();

		private const int ThresholdBits = 14;

		private Stream stream;
		private BinaryReader reader;
		private int maxBits;
		private INode[] table;

		public BinaryReader Reader => reader;

		public HuffmanReader(Stream stream) {
			if (!stream.CanSeek) throw new InvalidOperationException("只支持能重定向指针的流。");

			this.stream = stream;
			reader = new BinaryReader(stream);
			ReadTable();
		}

		unsafe private static T ReadKeyFromBuffer(byte[] buffer) {
			fixed (byte* p = buffer) {
				return Marshal.PtrToStructure<T>((IntPtr) p);
			}
		}

		private void ReadTable() {
			var tableLength = (int) reader.ReadUInt32();
			var table = new(T Key, int Bits, uint Binary)[tableLength];
			maxBits = 0;

			byte[] buffer = new byte[KeyBytes];
			for (int i = 0; i < tableLength; i++) {
				var binary = reader.ReadUInt32();
				int bits = (int) (binary >> 24);
				binary &= 0xFFFFFFU;
				reader.Read(buffer, 0, KeyBytes);
				var key = ReadKeyFromBuffer(buffer);

				maxBits = Math.Max(maxBits, bits);
				table[i] = (key, bits, binary);
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
						this.table[((uint) j << bits) | binary] = item;
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
					if (b >= 0) { buffer |= (ulong) b << bits; bits += 8; }
					b = reader.stream.ReadByte();
					if (b >= 0) { buffer |= (ulong) b << bits; bits += 8; }
					b = reader.stream.ReadByte();
					if (b >= 0) { buffer |= (ulong) b << bits; bits += 8; }
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
						var (value, keyBits) = GetValue(node, (uint) (buffer >> maxBits));
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
