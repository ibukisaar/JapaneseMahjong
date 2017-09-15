using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将.Yakus {
	/// <summary>
	/// 所有役的基类。
	/// </summary>
	public abstract class Yaku : IComparable<Yaku> {
		/// <summary>
		/// 该役的顺序编号。影响显示顺序及比较顺序。
		/// </summary>
		public abstract int OrderIndex { get; }
		/// <summary>
		/// 表示役的类型，参考<see cref="YakuType"/>。
		/// </summary>
		public abstract YakuType Type { get; }
		/// <summary>
		/// 第一层过滤器，判断顺子数量和刻子数量。返回true则通过，false则不通过。
		/// <para>对于特殊牌型，<paramref name="junkoCount"/>和<paramref name="pungCount"/>都等于0。</para>
		/// </summary>
		/// <param name="junkoCount">指groups的顺子数量</param>
		/// <param name="pungCount">指groups刻子数量</param>
		/// <returns></returns>
		internal protected virtual bool FilterTest(int junkoCount, int pungCount) => true;
		/// <summary>
		/// 第二层过滤器，判断牌种类（万[0],饼[1],索[2],字[3]）的数量。后2个参数所统计的对象是groups。
		/// <para>返回true则通过，false则不通过。</para>
		/// </summary>
		/// <param name="kindCountsFromTiles">对tiles的统计结果。</param>
		/// <param name="kindCounts">包含雀头的统计结果，换言之是5个group的统计结果。</param>
		/// <param name="kindCountsWithoutPair">不包含雀头，换言之是4个group的统计结果。</param>
		/// <returns></returns>
		internal protected virtual bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) => true;
		/// <summary>
		/// 测试并将役名与番数添加至返回集合中。返回false表示不匹配该类负责的任何役。
		/// </summary>
		/// <param name="result">可修改的返回集合</param>
		/// <param name="tiles">当前手牌</param>
		/// <param name="groups">包含雀头、顺子、刻子、杠子。如果是国士无双或七对子，该值为null</param>
		/// <param name="env">环境flags</param>
		/// <returns></returns>
		internal protected abstract bool Test(ICollection<YakuValue> result, ITiles tiles, IGroups groups, YakuEnvironment env);

		/// <summary>
		/// 拦截符数计算。若该方法返回true，则后续不会对符数进行任何计算。
		/// </summary>
		/// <param name="fu">符数。</param>
		/// <param name="groups"></param>
		/// <param name="env"></param>
		/// <returns></returns>
		internal protected virtual bool HookCalculateFu(ref int fu, IGroups groups, YakuEnvironment env) => false;

		public int CompareTo(Yaku other) => OrderIndex - other.OrderIndex;

		protected static bool SelfWindEquals(Wind wind, YakuEnvironment env) {
			if (wind == Wind.None) throw new ArgumentException($"不可以为{nameof(Wind)}.{nameof(Wind.None)}", nameof(wind));
			int val1 = ((int) env >> 6) & 0xf;
			int val2 = 1 << ((int) wind - 1);
			return val1 == val2;
		}
	}
}
