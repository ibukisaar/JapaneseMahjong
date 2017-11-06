using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace 日本麻将.表生成 {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">成员变量中不能有class（包括间接的），否则将导致内存泄漏。</typeparam>
	public sealed class HuffmanWriter<T> : IDisposable where T : struct {
		private static readonly int KeyBytes = Marshal.SizeOf<T>();

		private bool freeze = false;
		private Stream stream;
		private BinaryWriter writer;
		private Dictionary<T, (int Bits, uint Binary)> table;

		public BinaryWriter Writer => !freeze ? writer : throw new InvalidOperationException($"必须先结束对{nameof(WriteHandler)}的访问。");

		public HuffmanWriter(Stream stream, IEnumerable<(T Key, long Count)> keys) {
			this.stream = stream;
			writer = new BinaryWriter(stream);
			var table = Huffman<T>.Build(keys);
			var (_, count, _, maxBits) = Huffman<T>.GetInfo(table);
			if (maxBits > 24) throw new InvalidOperationException("编码后最大比特数大于24，暂不支持。");
			WriteTable(table);
		}

		private static uint ToUInt32(string binary) {
			uint value = 0;
			for (int shift = 0; shift < binary.Length; shift++) {
				value |= (uint) ((binary[shift] - '0') << shift);
			}
			return value;
		}

		unsafe private static void WriteKeyToBuffer(T key, byte[] buffer) {
			fixed (byte* p = buffer) {
				Marshal.StructureToPtr(key, (IntPtr) p, false);
			}
		}

		private void WriteTable(List<(T Key, long Count, string Binary)> table) {
			this.table = new Dictionary<T, (int Bits, uint Binary)>(table.Count);
			writer.Write(table.Count);
			var buffer = new byte[KeyBytes];
			foreach (var item in table) {
				var bits = item.Binary.Length;
				var binary = ToUInt32(item.Binary);
				WriteKeyToBuffer(item.Key, buffer);
				this.table.Add(item.Key, (bits, binary));
				binary |= (uint) bits << 24;
				writer.Write(binary);
				writer.Write(buffer);
			}
		}

		public WriteHandler BeginWrite(int count) {
			return new WriteHandler(this, count);
		}

		public void Dispose() {
			writer.Dispose();
			GC.SuppressFinalize(this);
		}

		~HuffmanWriter() {
			Dispose();
		}

		public sealed class WriteHandler : IDisposable {
			private HuffmanWriter<T> writer;
			private ulong buffer = 0;
			private int offset = 0;
			private int currCount = 0;
			private int count;


			public WriteHandler(HuffmanWriter<T> writer, int count) {
				writer.freeze = true;
				this.writer = writer;
				this.count = count;
				writer.writer.Write(count);
			}

			private void Write(ref ulong buffer, ref int bitOffset, uint binary, int bits) {
				buffer |= (ulong) binary << bitOffset;
				bitOffset += bits;
				for (; bitOffset >= 8; bitOffset -= 8, buffer >>= 8) {
					writer.stream.WriteByte((byte) buffer);
				}
			}

			private void Flush(ulong buffer, int bitOffset) {
				if (currCount != count) throw new InvalidOperationException("写入的数量和预期不一致。");
				if (bitOffset > 0) {
					writer.stream.WriteByte((byte) buffer);
				}
			}

			public void Write(T value) {
				var (bits, binary) = writer.table[value];
				Write(ref buffer, ref offset, binary, bits);
				currCount++;
			}

			public void Dispose() {
				Flush(buffer, offset);
				writer.freeze = false;
			}
		}
	}
}
