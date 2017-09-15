using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	public interface IBaseTiles : IReadOnlyList<BaseTile> {
		BaseTile[] BaseTiles { get; }
		SortedTilesEnumerator Sorted { get; }
	}
}
