using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	static class Tiles {
		public readonly static Tile 一万 = new WanTile(1);
		public readonly static Tile 二万 = new WanTile(2);
		public readonly static Tile 三万 = new WanTile(3);
		public readonly static Tile 四万 = new WanTile(4);
		public readonly static Tile 五万 = new WanTile(5);
		public readonly static Tile 六万 = new WanTile(6);
		public readonly static Tile 七万 = new WanTile(7);
		public readonly static Tile 八万 = new WanTile(8);
		public readonly static Tile 九万 = new WanTile(9);
		public readonly static Tile 一饼 = new PinTile(1);
		public readonly static Tile 二饼 = new PinTile(2);
		public readonly static Tile 三饼 = new PinTile(3);
		public readonly static Tile 四饼 = new PinTile(4);
		public readonly static Tile 五饼 = new PinTile(5);
		public readonly static Tile 六饼 = new PinTile(6);
		public readonly static Tile 七饼 = new PinTile(7);
		public readonly static Tile 八饼 = new PinTile(8);
		public readonly static Tile 九饼 = new PinTile(9);
		public readonly static Tile 一索 = new SouTile(1);
		public readonly static Tile 二索 = new SouTile(2);
		public readonly static Tile 三索 = new SouTile(3);
		public readonly static Tile 四索 = new SouTile(4);
		public readonly static Tile 五索 = new SouTile(5);
		public readonly static Tile 六索 = new SouTile(6);
		public readonly static Tile 七索 = new SouTile(7);
		public readonly static Tile 八索 = new SouTile(8);
		public readonly static Tile 九索 = new SouTile(9);
		public readonly static Tile 东 = new KanjiTile(KanjiTile.Kanji.东);
		public readonly static Tile 南 = new KanjiTile(KanjiTile.Kanji.南);
		public readonly static Tile 西 = new KanjiTile(KanjiTile.Kanji.西);
		public readonly static Tile 北 = new KanjiTile(KanjiTile.Kanji.北);
		public readonly static Tile 白 = new KanjiTile(KanjiTile.Kanji.白);
		public readonly static Tile 发 = new KanjiTile(KanjiTile.Kanji.发);
		public readonly static Tile 中 = new KanjiTile(KanjiTile.Kanji.中);
	}
}
