using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	[Flags]
	public enum YakuEnvironment : uint {
		无 = 0,
		立直 = 1 << 0,
		自摸 = 1 << 1,
		一发 = 1 << 2,
		W立直 = 1 << 3,
		抢杠 = 1 << 4,
		/// <summary>
		/// 天和、地和、人和的可能性
		/// </summary>
		第一巡 = 1 << 5,
		自风东 = 1 << 6,
		自风南 = 1 << 7,
		自风西 = 1 << 8,
		自风北 = 1 << 9,
		场风东 = 1 << 10,
		场风南 = 1 << 11,
		场风西 = 1 << 12,
		场风北 = 1 << 13,
		/// <summary>
		/// 海底或河底
		/// </summary>
		海底 = 1 << 14,
		岭上开花 = 1 << 15,
		门前清 = 1 << 16
	}
}
