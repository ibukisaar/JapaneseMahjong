using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public interface IGroups : IReadOnlyList<Group> {
		/// <summary>
		/// 雀头。
		/// </summary>
		Pair Pair { get; }
		/// <summary>
		/// 所有顺子。
		/// </summary>
		Junko[] JunkoList { get; }
		/// <summary>
		/// 所有刻子和杠子。
		/// </summary>
		Group[] PungList { get; }
		Pull[] PullList { get; }
	}
}
