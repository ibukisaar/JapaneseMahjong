using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	/// <summary>
	/// 表示役是否属于环境役或役满。
	/// </summary>
	[Flags]
	public enum YakuType {
		/// <summary>
		/// 没有特殊要求的役。
		/// </summary>
		None = 0,
		/// <summary>
		/// 指与牌面无关的役。
		/// </summary>
		环境 = 1,
		/// <summary>
		/// 指役满。如果某役满成立，所有一般役将失效。
		/// </summary>
		役满 = 2,
		/// <summary>
		/// 指对听牌形式有要求的役。典型例子为“平和”、“三暗刻”。
		/// <para>此标志位会将不同的听牌形式视为不同的groups，并分别进行Test。</para>
		/// </summary>
		听牌形式 = 4,
		/// <summary>
		/// 只计算Dora的役。
		/// </summary>
		Dora = 8,
	}
}
