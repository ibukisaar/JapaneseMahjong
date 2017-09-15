using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	public sealed class YakuValue {
		public Yaku Source { get; }
		/// <summary>
		/// 显示的名字。
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// 役满倍数，为0表示非役满。
		/// </summary>
		public int FullYaku { get; }
		/// <summary>
		/// 番数，如果为役满则忽略此值。
		/// </summary>
		public int FanValue { get; }
		/// <summary>
		/// Dora产生的番数。
		/// </summary>
		public int DoraValue { get; }

		private YakuValue(Yaku source, string name, int fanValue, int fullYaku, int dora) {
			Source = source;
			Name = name;
			FanValue = fanValue;
			FullYaku = fullYaku;
			DoraValue = dora;
		}

		public override string ToString()
			=> FullYaku != 0 ? $"{Name}[{FullYaku}倍役满]" : $"{Name}[{FanValue}]";

		public override int GetHashCode() => Name.GetHashCode();

		public override bool Equals(object obj) => obj is YakuValue other && Name == other.Name;

		/// <summary>
		/// 构建一个役满。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fullYaku"></param>
		/// <returns></returns>
		public static YakuValue FromFullYaku(Yaku source, string name, int fullYaku) => new YakuValue(source, name, 0, fullYaku, 0);
		/// <summary>
		/// 构建一个普通役。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fanValue"></param>
		/// <returns></returns>
		public static YakuValue FromFanValue(Yaku source, string name, int fanValue, int dora = 0) => new YakuValue(source, name, fanValue, 0, dora);

		/// <summary>
		/// 构建一个用于移除的键值。
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static YakuValue BuildKey(string name) => new YakuValue(null, name, 0, 0, 0);
	}
}
