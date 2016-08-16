using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace 日本麻将 {
	static class Program {
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() {
			const int N = 500;

			Stopwatch sw = new Stopwatch();
			////Tile[] tiles = { new WanTile(2), new WanTile(1), new WanTile(1), new WanTile(1), new WanTile(2), new WanTile(3), new WanTile(4), new WanTile(5), new WanTile(6), new WanTile(7), new WanTile(8), new WanTile(9), new WanTile(9), new WanTile(9) };
			//Tile[] tiles = Mahjong.Parse("一万 一万 一万 一万 二万 二万 二万 二万 三万 三万 三万 三万 四万 四万");
			////Tile[] tiles = Mahjong.Parse("一万 一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 九万 九万");
			//sw.Restart();
			//List<Mahjong.IGroup[]> result = null;
			//for (int i = 0; i < N; i++)
			//	result = Mahjong.Analysis(tiles);
			//sw.Stop();
			//Console.WriteLine($"平均耗时：{TimeSpan.FromTicks(sw.ElapsedTicks / 1000)}");
			//Console.WriteLine($"返回数量：{result.Count}");
			//foreach (Mahjong.IGroup[] groups in result) {
			//	Console.Write($"[{groups[0]}]");
			//	for (int i = 1; i < groups.Length; i++) {
			//		Console.Write($", [{groups[i]}]");
			//	}
			//	Console.WriteLine();
			//}

			////Tile[] tiles2 = { new WanTile(1), new WanTile(1), new WanTile(1), new WanTile(2), new WanTile(3), new WanTile(4), new WanTile(5), new WanTile(6), new WanTile(7), new WanTile(8), new WanTile(9), new WanTile(9), new WanTile(9) };
			//Tile[] tiles2 = Mahjong.Parse("一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 九万 九万");

			//sw.Restart();
			//List<Tile> result2 = null;
			//for (int i = 0; i < N; i++)
			//	result2 = Mahjong.AllReadyHand(tiles2);
			//sw.Stop();
			//Console.WriteLine($"平均耗时：{TimeSpan.FromTicks(sw.ElapsedTicks / 1000)}");
			//foreach (Tile tile in result2) {
			//	Console.WriteLine(tile);
			//}
			//Console.WriteLine();

			//SortedTiles tiles3 = new SortedTiles(Mahjong.CreateRandomTiles(13));
			// Tile[] tiles3 = { new WanTile(1), new WanTile(2), new WanTile(3), new SouTile(1), new SouTile(3), new SouTile(5), new PinTile(2), new PinTile(4), new PinTile(8), new PinTile(9), new PinTile(9), new KanjiTile(KanjiTile.Kanji.南), new KanjiTile(KanjiTile.Kanji.南), };
			// Tile[] tiles3 = Mahjong.Parse("一万 三万 五万 六万 九万 九万 二饼 五饼 九饼 三索 七索 东 中");
			// Tile[] tiles3 = Mahjong.Parse("一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 九万 一索");
			// Tile[] tiles3 = Mahjong.Parse("一万 九万 一饼 一饼 九饼 一索 九索 南 北 白 白 发 中");
			//foreach (Tile tile in tiles3) {
			//	Console.Write($"{tile} ");
			//}
			//Console.WriteLine();
			//sw.Restart();
			//int listenNumber = 0;
			//for (int i = 0; i < N; i++)
			//	listenNumber = Mahjong.ListenNumber(tiles3);
			//sw.Stop();
			//Console.WriteLine($"平均耗时：{TimeSpan.FromTicks(sw.Elapsed.Ticks / N)}");
			//Console.WriteLine($"最小向听数：{listenNumber}");
			////Console.WriteLine($"SBT操作次数：{Mahjong.sbtCount}");
			////Console.WriteLine($"SBT操作耗时：{Mahjong.sw.Elapsed}");

			////Application.EnableVisualStyles();
			////Application.SetCompatibleTextRenderingDefault(false);
			////Application.Run(new Form1());

			////Tile[] tiles = Mahjong.Parse("一万 二万 一饼 一饼 九饼 一索 九索 南 北 白 白 发 中");
			////SortedTiles sortedTiles = new SortedTiles(tiles);
			////Console.WriteLine(sortedTiles.HasNextTile(tiles[0]));
			////Console.WriteLine(sortedTiles.DifferentNext(tiles[2]));

			////sortedTiles.Remove(tiles[2]);
			////sortedTiles.Add(tiles[0]);
			////Console.WriteLine("===");
			////foreach (var tile in sortedTiles) {
			////	Console.WriteLine(tile);
			////}

			//for (int m = 0; m < 100; m++) {
			//	SortedTiles tiles = new SortedTiles(Mahjong.CreateRandomTiles(13));
			//	// SortedTiles tiles = new SortedTiles(Mahjong.Parse("二万 五万 六万 七万 九万 三饼 五索 六索 九索   东   南   白   中"));
			//	foreach (Tile tile in tiles) {
			//		Console.Write(tile is KanjiTile ? $"  {tile} " : $"{tile} ");
			//	}

			//	sw.Restart();
			//	int listenNumber = 0;
			//	for (int i = 0; i < N; i++)
			//		listenNumber = Mahjong.ListenNumber(tiles);
			//	Console.Write($"| {listenNumber} | ");
			//	Console.WriteLine($"{TimeSpan.FromTicks(sw.ElapsedTicks / N)}");
			//	sw.Stop();
			//}

			// 三万 三万 三万 九饼 九饼 三索 三索 三索 六索   南   白   白   中
			//SortedTiles tiles = new SortedTiles(Mahjong.Parse("一万 一万 二万 二万 二万 三万 四万 五万 六万 七万 发 发 发"));
			//int listenNumber = Mahjong.ListenNumber(tiles);
			//Console.WriteLine(listenNumber);

			//for (int m = 0; m < 30; m++) {
			//	while (true) {
			//		SortedTiles tiles = new SortedTiles(Mahjong.CreateRandomTiles(13));
			//		int listenNumber = Mahjong.ListenNumber(tiles);
			//		if (listenNumber == 0) {
			//			foreach (Tile tile in tiles) {
			//				Console.Write(tile is KanjiTile ? $"  {tile} " : $"{tile} ");
			//			}
			//			Console.WriteLine();
			//			break;
			//		}
			//	}
			//}

			int r1 = 0, r2 = 0;
			while (r1 == r2) {
				SortedTiles tiles = new SortedTiles(Mahjong.CreateRandomTiles(13));
				r1 = Mahjong.ListenNumber(tiles);
				r2 = Mahjong.ListenNumber2(tiles);

				if (r1 != r2) {
					foreach (Tile tile in tiles) {
						Console.Write(tile is KanjiTile ? $"  {tile} " : $"{tile} ");
					}

					Console.WriteLine();
					Console.WriteLine($"{r1}, {r2}");
				}
			}
		}
	}
}
