using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	/// <summary>
	/// 无法通过Groups（雀头、顺子、刻子、杠子）判断的役。
	/// <para>注：添加一个新的特殊役，您需要亲自实现求向听数的方法。</para>
	/// </summary>
	public abstract class SpecialYaku : Yaku {
		protected internal sealed override bool FilterTest(int junkoCount, int pungCount) => junkoCount + pungCount == 0;

		protected internal sealed override bool FilterTest(int[] kindCountsFromTiles, int[] kindCounts, int[] kindCountsWithoutPair) {
			return FilterTest(kindCountsFromTiles);
		}

		protected virtual bool FilterTest(int[] kindCountsFromTiles) => true;

		/// <summary>
		/// 求向听数。
		/// <para>返回-1，表示已和牌。</para>
		/// </summary>
		/// <param name="tiles">要计算的牌集合。</param>
		/// <param name="result">通过此参数返回，同时此参数也表示其他役最小的向听数结果。如果该役的向听数比原先大，则不应该更新该值。</param>
		internal protected abstract void Syanten(IBaseTiles tiles, ref int result);

		internal protected abstract bool TestRon(ITiles tiles, YakuEnvironment env);
	}
}
