using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	static class Ex {
		public static T Next<T>(this IEnumerator<T> itor) {
			itor.MoveNext();
			return itor.Current;
		}
	}
}
