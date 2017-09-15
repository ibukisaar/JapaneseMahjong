using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using 日本麻将.Yakus;

namespace 日本麻将 {
	static class Program {
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main() {
			const int N = 50;

			Stopwatch sw = new Stopwatch();
			////Tile[] tiles = { new WanTile(2), new WanTile(1), new WanTile(1), new WanTile(1), new WanTile(2), new WanTile(3), new WanTile(4), new WanTile(5), new WanTile(6), new WanTile(7), new WanTile(8), new WanTile(9), new WanTile(9), new WanTile(9) };
			//var game = new TenhouGame();
			//var tiles = game.GetTiles(Mahjong.Parse("一万 一万 一万 一万 二万 二万 二万 三万 三万 三万 三万 四万 四万 二万"));
			////var tiles = game.GetTiles(Mahjong.Parse("一万 一万 一万 一万 二万 三万 五万 六万 七万 八万 九万 九万 九万 四万 "));
			//////var tiles = game.GetTiles(Mahjong.Parse("一万 一万 二万 三万 一万"));
			////sw.Restart();
			//List<Group[]> result = null;
			////for (int i = 0; i < N; i++)
			//result = Mahjong.Analysis(game, tiles);
			////sw.Stop();
			////Console.WriteLine($"平均耗时：{TimeSpan.FromTicks(sw.ElapsedTicks / N)}");
			//Console.WriteLine($"返回数量：{result.Count}");
			//foreach (Group[] groups in result) {
			//	Console.Write($"[{groups[0]}]");
			//	for (int i = 1; i < groups.Length; i++) {
			//		Console.Write($", [{groups[i]}]");
			//	}
			//	Console.WriteLine();
			//}

			////Tile[] tiles2 = { new WanTile(1), new WanTile(1), new WanTile(1), new WanTile(2), new WanTile(3), new WanTile(4), new WanTile(5), new WanTile(6), new WanTile(7), new WanTile(8), new WanTile(9), new WanTile(9), new WanTile(9) };
			//var tiles2 = Mahjong.Parse("一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 九万 九万");

			//sw.Restart();
			//List<BaseTile> result2 = null;
			//for (int i = 0; i < N; i++)
			//	result2 = Mahjong.AllReadyHand(tiles2);
			//sw.Stop();
			//Console.WriteLine($"平均耗时：{TimeSpan.FromTicks(sw.ElapsedTicks / N)}");
			//foreach (var tile in result2) {
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
			//var tiles = Mahjong.Parse("一万 一万 二万 二万 二万 三万 四万 五万 六万 七万 中 发 发");
			//var tiles = Mahjong.Parse("一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 九万 白");
			//int listenNumber = 0;
			//sw.Restart();
			//for (int i = 0; i < N; i++) {
			//	listenNumber = Mahjong.ListenNumber(tiles);
			//}
			//sw.Stop();
			//Console.WriteLine(listenNumber);
			//Console.WriteLine(sw.Elapsed);
			//Console.WriteLine(TimeSpan.FromTicks(sw.Elapsed.Ticks / N));


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

			//for (int k = 0; k < 300; k++) {
			//	var tiles = new SortedTilesEnumerator(Mahjong.CreateRandomTiles(14));
			//	int r1 = 0, r2 = 0;
			//	TimeSpan t1 = TimeSpan.Zero, t2 = TimeSpan.Zero;

			//	sw.Restart();
			//	r1 = Mahjong.ListenNumber2(tiles) + 1;
			//	sw.Stop();
			//	t1 = sw.Elapsed;

			//	for (int i = 0; i < r1; i++) Console.Write('☆');
			//	for (int i = r1; i < 7; i++) Console.Write('★');
			//	Console.Write($"{t1} ");
			//	foreach (var t in tiles) Console.Write($"{t}");
			//	Console.WriteLine();

			//	// if (r1 >= 3) break;
			//}

			//int c1 = 0, c2 = 0;
			//TimeSpan tt1 = TimeSpan.Zero, tt2 = TimeSpan.Zero;
			//while (true) {
			//	int r1 = 0, r2 = 0;
			//	TimeSpan t1, t2;
			//	//var tiles = new SortedTilesEnumerator(Mahjong.CreateRandomTiles(14));
			//	var tiles = Mahjong.Parse("一万 一万 一万 二万 三万 四万 五万 六万 七万 八万 九万 一索 一索 一索");
			//	//var tiles = Mahjong.Parse("一万 一万 一万 一万");
			//	sw.Restart();
			//	r1 = Mahjong.ListenNumber(tiles);
			//	sw.Stop();
			//	t1 = sw.Elapsed;
			//	sw.Restart();
			//	r2 = Mahjong.ListenNumber2(tiles);
			//	sw.Stop();
			//	t2 = sw.Elapsed;

			//	tt1 += t1;
			//	tt2 += t2;

			//	if (r1 != r2) {
			//		Console.WriteLine($"{r1},{r2}");
			//		Console.WriteLine(string.Join<BaseTile>("", tiles));
			//		Console.WriteLine(TimeSpan.FromTicks(t1.Ticks / N));
			//		Console.WriteLine(TimeSpan.FromTicks(t2.Ticks / N));
			//		break;
			//	} else {
			//		//if (t1 < t2) c1++; else c2++;
			//		//Console.Write($"\r{c1},{c2}");
			//		//Console.Write($"\r{tt1},{tt2}");

			//		Console.WriteLine($"{r1},{r2}");
			//		Console.WriteLine(string.Join<BaseTile>("", tiles));
			//		Console.WriteLine(TimeSpan.FromTicks(t1.Ticks / N));
			//		Console.WriteLine(TimeSpan.FromTicks(t2.Ticks / N));
			//		break;
			//	}
			//}


			var game = Game.Instance;
			//var tiles = Mahjong.Parse("七万 八万 九万 五饼 五饼 七饼 八饼 一索 二索 三索 七索 八索 九索 东");
			//var tiles = Mahjong.Parse(" 一万 一万 一万 二万 二万 二万 二万 三万 三万 三万 三万 四万 四万 一万");

			//Console.WriteLine(string.Join("", tiles.Select(t => t.BaseTile).OrderBy(t => t)));
			//Tile last = tiles[tiles.Length - 1];
			//last.Owner = Wind.东;
			//var a = game.Analysis(tiles);
			//var score = game.GetScore(tiles, null, YakuEnvironment.门前清 | YakuEnvironment.自摸 | YakuEnvironment.场风东 | YakuEnvironment.自风东);
			//var basePoint = game.ScoreSystem.GetBasePoint(score);
			//var point = game.ScoreSystem.GetPoint(basePoint, 2);


			//var tiles = game.GetTiles(Mahjong.Parse(" 二万 二万 三万 三万 四万 四万 五万 五万 六万 六万 七万 七万 八万 八万"));
			//var tiles = game.GetTiles(Mahjong.Parse(" 一万 一万 一万 二万 二万 二万 二万 三万 三万 三万 三万 四万 四万 一万"));
			//var tiles = game.GetTiles(Mahjong.Parse(" 一万 一万 一万 二万 二万 二万 三万 三万 三万 四万 四万 四万 五万 五万"));
			//var tiles = game.GetTiles(Mahjong.Parse("东 东 东 南 南 南 西 西 西 北 北 北 发 发"));
			//var tiles = game.GetTiles(Mahjong.Parse("东 东 南 南 西 西 北 北 白 白 发 发 中 中"));

			var tiles = new SortedTilesEnumerator(game.GetRandomTiles(14).Select(t => t.BaseTile));
			//var tiles = BaseTile.ParseSuffixExpr("1112345678999m1m");
			Console.WriteLine(string.Concat<BaseTile>(tiles));
			Console.WriteLine(BaseTile.ToSuffixExpr(tiles));
			SuggestResult suggest = null;
			sw.Restart();
			for (int i = 0; i < N; i++) suggest = game.Suggest(tiles);
			sw.Stop();
			Console.WriteLine(TimeSpan.FromTicks(sw.Elapsed.Ticks / N));
			Console.WriteLine(suggest);
			Console.WriteLine(string.Join(Environment.NewLine, suggest.Values));

			var tiles2 = game.GetTiles(tiles).ToArray();
			tiles2.Last().Owner = Wind.东;
			if (game.TestRon(tiles2)) {
				var a = game.Analysis(tiles2, new[] { new Pull(game.GetTile(BaseTile.Parse("北"))) });
				var score = game.GetScore(tiles2, new[] { new Pull(game.GetTile(BaseTile.Parse("北"))) }, YakuEnvironment.门前清 | YakuEnvironment.自摸);
				Console.WriteLine(score);
				Console.WriteLine();
			}
		}
	}
}
