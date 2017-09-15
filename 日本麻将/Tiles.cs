using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public static class Tiles {
		public readonly static BaseTile 一万 = new WanTile(1);
		public readonly static BaseTile 二万 = new WanTile(2);
		public readonly static BaseTile 三万 = new WanTile(3);
		public readonly static BaseTile 四万 = new WanTile(4);
		public readonly static BaseTile 五万 = new WanTile(5);
		public readonly static BaseTile 六万 = new WanTile(6);
		public readonly static BaseTile 七万 = new WanTile(7);
		public readonly static BaseTile 八万 = new WanTile(8);
		public readonly static BaseTile 九万 = new WanTile(9);
		public readonly static BaseTile 一饼 = new PinTile(1);
		public readonly static BaseTile 二饼 = new PinTile(2);
		public readonly static BaseTile 三饼 = new PinTile(3);
		public readonly static BaseTile 四饼 = new PinTile(4);
		public readonly static BaseTile 五饼 = new PinTile(5);
		public readonly static BaseTile 六饼 = new PinTile(6);
		public readonly static BaseTile 七饼 = new PinTile(7);
		public readonly static BaseTile 八饼 = new PinTile(8);
		public readonly static BaseTile 九饼 = new PinTile(9);
		public readonly static BaseTile 一索 = new SouTile(1);
		public readonly static BaseTile 二索 = new SouTile(2);
		public readonly static BaseTile 三索 = new SouTile(3);
		public readonly static BaseTile 四索 = new SouTile(4);
		public readonly static BaseTile 五索 = new SouTile(5);
		public readonly static BaseTile 六索 = new SouTile(6);
		public readonly static BaseTile 七索 = new SouTile(7);
		public readonly static BaseTile 八索 = new SouTile(8);
		public readonly static BaseTile 九索 = new SouTile(9);
		public readonly static BaseTile 东 = new KanjiTile(KanjiTile.Kanji.东);
		public readonly static BaseTile 南 = new KanjiTile(KanjiTile.Kanji.南);
		public readonly static BaseTile 西 = new KanjiTile(KanjiTile.Kanji.西);
		public readonly static BaseTile 北 = new KanjiTile(KanjiTile.Kanji.北);
		public readonly static BaseTile 白 = new KanjiTile(KanjiTile.Kanji.白);
		public readonly static BaseTile 发 = new KanjiTile(KanjiTile.Kanji.发);
		public readonly static BaseTile 中 = new KanjiTile(KanjiTile.Kanji.中);

	}
}
