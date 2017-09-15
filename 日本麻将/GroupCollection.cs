using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 日本麻将.Yakus;

namespace 日本麻将 {
	public sealed class GroupCollection : IGroups {
		private Group[] groups;

		public Group this[int index] => groups[index];

		public Pair Pair { get; }

		public Junko[] JunkoList { get; }

		public Group[] PungList { get; }

		public Pull[] PullList { get; }

		public int Count => groups.Length;

		public GroupCollection(IEnumerable<Group> groups) {
			this.groups = groups.Where(g => g.IsImportant).ToArray();
			Pair = groups.OfType<Pair>().First();
			JunkoList = groups.OfType<Junko>().ToArray();
			PungList = groups.Where(g => g.IsPung).ToArray();
			PullList = groups.OfType<Pull>().ToArray();
		}

		public IEnumerator<Group> GetEnumerator() {
			foreach (var g in groups) yield return g;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return groups.GetEnumerator();
		}

		public override string ToString()
			=> string.Concat(groups.Select(g => $"[{g}]"));
	}
}
