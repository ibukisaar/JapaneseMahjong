using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 日本麻将 {
	class SBT<T> : IEnumerable<T> where T : IComparable<T> {
		private enum State {
			None, Left, Right
		}

		private enum Flags : uint {
			None = 0,
			Left_Left = 1, Left_Right = 2, Right_Left = 4, Right_Right = 8,
			Left = Left_Left | Left_Right, Right = Right_Left | Right_Right
		}

		private class Node {
			public int Size = 1;
			public Node Left = null;
			public Node Right = null;
			public T Value;

			public override string ToString() {
				return $"[Size: {Size}, Value: {Value}]";
			}
		}

		private Node root = null;

		public int Count { get { return Size(root); } }

		public SBT() { }

		public SBT(IEnumerable<T> values) {
			foreach (T value in values) {
				Add(value);
			}
		}

		public void Add(T value) {
			State state;
			Add(ref root, value, out state);
		}

		public bool Remove(T value) {
			return RemoveLocation(ref root, value);
		}

		public bool Contains(T value) {
			return Contains(root, value);
		}

		public int IndexOf(T value) {
			return IndexOf(root, value);
		}

		public T this[int index] {
			get {
				if (index >= 0 && index < Size(root)) {
					return At(root, index);
				} else {
					throw new ArgumentOutOfRangeException(nameof(index), $"下标越界，{nameof(index)} = {index}, Count = {Size(root)}。");
				}
			}
		}

		/// <summary>
		/// 下一个不同的值
		/// </summary>
		/// <param name="value"></param>
		/// <param name="nextValue"></param>
		/// <returns></returns>
		public bool DifferentNext(T value, out T nextValue) {
			bool @finally = false;
			return DifferentNext(root, value, ref @finally, out nextValue);
		}

		/// <summary>
		/// 上一个不同的值
		/// </summary>
		/// <param name="value"></param>
		/// <param name="previousValue"></param>
		/// <returns></returns>
		public bool DifferentPrevious(T value, out T previousValue) {
			bool @finally = false;
			return DifferentPrevious(root, value, ref @finally, out previousValue);
		}

		private static bool DifferentNext(Node node, T value, ref bool @finally, out T nextValue) {
			if (node == null) {
				nextValue = default(T);
				return false;
			}

			int cmp = value.CompareTo(node.Value);
			if (cmp == 0) {
				node = node.Right;

				while (node != null && node.Value.CompareTo(value) == 0) {
					node = node.Right;
				}

				if (node != null) {
					while (node.Left != null && node.Left.Value.CompareTo(value) != 0) {
						node = node.Left;
					}

					nextValue = node.Value;
					@finally = true;
					return true;
				}
			} else if (cmp < 0) {
				if (!DifferentNext(node.Left, value, ref @finally, out nextValue)) {
					nextValue = node.Value;
					@finally = true;
					return true;
				} else if (@finally) {
					return true;
				}
			} else {
				return DifferentNext(node.Right, value, ref @finally, out nextValue);
			}

			nextValue = default(T);
			return false;
		}

		private static bool DifferentPrevious(Node node, T value, ref bool @finally, out T previousValue) {
			if (node == null) {
				previousValue = default(T);
				return false;
			}

			int cmp = value.CompareTo(node.Value);
			if (cmp == 0) {
				node = node.Left;

				while (node != null && node.Value.CompareTo(value) == 0) {
					node = node.Left;
				}

				if (node != null) {
					while (node.Right != null && node.Right.Value.CompareTo(value) != 0) {
						node = node.Right;
					}

					previousValue = node.Value;
					@finally = true;
					return true;
				}
			} else if (cmp < 0) {
				return DifferentPrevious(node.Left, value, ref @finally, out previousValue);
			} else {
				if (!DifferentPrevious(node.Right, value, ref @finally, out previousValue)) {
					previousValue = node.Value;
					@finally = true;
					return true;
				} else if (@finally) {
					return true;
				}
			}

			previousValue = default(T);
			return false;
		}

		private static T At(Node node, int index) {
			int size = Size(node.Left);
			if (index < size) {
				return At(node.Left, index);
			} else if (index == size) {
				return node.Value;
			} else {
				return At(node.Right, index - size - 1);
			}
		}

		private static int IndexOf(Node node, T value) {
			if (node == null) return -1;

			int cmp = value.CompareTo(node.Value);
			if (cmp == 0) {
				Node p = node;
				node = node.Left;

				while (node != null && node.Value.CompareTo(value) == 0) {
					p = node;
					node = node.Left;
				}

				return Size(p.Left);
			} else if (cmp < 0) {
				return IndexOf(node.Left, value);
			} else {
				return IndexOf(node.Right, value);
			}
		}

		private static void LeftRotote(ref Node node) {
			Node r = node.Right;
			node.Right = r.Left;
			r.Left = node;
			r.Size = node.Size;
			node.Size = Size(node.Left) + Size(node.Right) + 1;
			node = r;
		}

		private static void RightRotote(ref Node node) {
			Node l = node.Left;
			node.Left = l.Right;
			l.Right = node;
			l.Size = node.Size;
			node.Size = Size(node.Left) + Size(node.Right) + 1;
			node = l;
		}

		private static bool Contains(Node node, T value) {
			if (node == null) {
				return false;
			}

			int cmp = value.CompareTo(node.Value);
			if (cmp < 0) {
				return Contains(node.Left, value);
			} else if (cmp > 0) {
				return Contains(node.Right, value);
			} else {
				return true;
			}
		}

		private static void Add(ref Node node, T value, out State state) {
			if (node == null) {
				node = new Node() { Value = value };
				state = State.None;
			} else {
				State state2;
				int cmp = value.CompareTo(node.Value);
				if (cmp < 0) {
					Add(ref node.Left, value, out state2);
					node.Size++;
					state = State.Left;
					if (state2 == State.Left) {
						Maintain(ref node, Flags.Left_Left);
					} else if (state2 == State.Right) {
						Maintain(ref node, Flags.Left_Right);
					}
				} else {
					Add(ref node.Right, value, out state2);
					node.Size++;
					state = State.Right;
					if (state2 == State.Left) {
						Maintain(ref node, Flags.Right_Left);
					} else if (state2 == State.Right) {
						Maintain(ref node, Flags.Right_Right);
					}
				}
			}
		}

		private static void SwapFromRemoveLeft(ref Node node, ref Node other) {
			Node temp = other;
			other = other.Left;
			temp.Left = node.Left;
			temp.Right = node.Right;
			node = temp;
		}

		private static void SwapFromRemoveRight(ref Node node, ref Node other) {
			Node temp = other;
			other = other.Right;
			temp.Left = node.Left;
			temp.Right = node.Right;
			node = temp;
		}

		private static bool RemoveLocation(ref Node node, T value) {
			bool result = false;
			if (node == null) return false;

			int cmp = value.CompareTo(node.Value);
			if (cmp == 0) {
				Remove(ref node, value);
				result = true;
			} else {
				if (cmp < 0) {
					result = RemoveLocation(ref node.Left, value);
				} else {
					result = RemoveLocation(ref node.Right, value);
				}

				if (result) {
					node.Size--;
				}
			}

			return result;
		}

		private static void Remove(ref Node node, T value) {
			if (node.Right == null) {
				node = node.Left;
			} else if (node.Left == null) {
				node = node.Right;
			} else {
				int leftSize = Size(node.Left);
				int rightSize = Size(node.Right);
				if (leftSize > rightSize) {
					RemoveLeftWhileRight(ref node);
				} else {
					RemoveRightWhileLeft(ref node);
				}
				node.Size = leftSize + rightSize;
			}
		}

		private static void RemoveRightWhileLeft(ref Node node) {
			Node p = node;
			Node temp = node.Right;

			while (temp.Left != null) {
				p = temp;
				p.Size--;
				temp = temp.Left;
			}

			if (p != node) {
				SwapFromRemoveRight(ref node, ref p.Left);
			} else {
				temp.Left = node.Left;
				node = temp;
			}
		}

		private static void RemoveLeftWhileRight(ref Node node) {
			Node p = node;
			Node temp = node.Left;

			while (temp.Right != null) {
				p = temp;
				p.Size--;
				temp = temp.Right;
			}

			if (p != node) {
				SwapFromRemoveLeft(ref node, ref p.Right);
			} else {
				temp.Right = node.Right;
				node = temp;
			}
		}

		private static int Size(Node node) => node != null ? node.Size : 0;

		private static void Maintain(ref Node node, Flags flags) {
			if (node == null) return;

			if ((flags & Flags.Left) != Flags.None) {
				if ((flags & Flags.Left_Left) != Flags.None && Size(node.Left?.Left) > Size(node.Right)) {
					RightRotote(ref node);
					Maintain(ref node.Right, Flags.Right);
					Maintain(ref node, Flags.Right_Left);
				} else if ((flags & Flags.Left_Right) != Flags.None && Size(node.Left?.Right) > Size(node.Right)) {
					LeftRotote(ref node.Left);
					RightRotote(ref node);
					Maintain(ref node.Right, Flags.Right);
					Maintain(ref node.Left, Flags.Left);
				} else {
					return;
				}
			} else {
				if ((flags & Flags.Right_Right) != Flags.None && Size(node.Right?.Right) > Size(node.Left)) {
					LeftRotote(ref node);
					Maintain(ref node.Left, Flags.Left);
					Maintain(ref node, Flags.Left_Right);
				} else if ((flags & Flags.Right_Left) != Flags.None && Size(node.Right?.Left) > Size(node.Left)) {
					RightRotote(ref node.Right);
					LeftRotote(ref node);
					Maintain(ref node.Left, Flags.Left);
					Maintain(ref node.Right, Flags.Right);
				} else {
					return;
				}
			}
		}

		private static bool Check(Node node) {
			if (node == null) return true;

			bool result =
				Size(node.Left?.Left) <= Size(node.Right) &&
				Size(node.Left?.Right) <= Size(node.Right) &&
				Size(node.Right?.Left) <= Size(node.Left) &&
				Size(node.Right?.Right) <= Size(node.Left) &&
				Check(node.Left) && Check(node.Right);

			return result;
		}

		public bool Check() {
			return Check(root);
		}

		private static bool CheckSize(Node node) {
			if (node == null) return true;

			return node.Size == Size(node.Left) + Size(node.Right) + 1 && CheckSize(node.Left) && CheckSize(node.Right);
		}

		public bool CheckSize() {
			return CheckSize(root);
		}

		private static IEnumerable<T> GetEnumerator(Node node) {
			if (node != null) {
				foreach (var vlaue in GetEnumerator(node.Left))
					yield return vlaue;
				yield return node.Value;
				foreach (var value in GetEnumerator(node.Right))
					yield return value;
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return GetEnumerator(root).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		/// <summary>
		/// 快速克隆一个相同的SBT
		/// </summary>
		/// <returns></returns>
		public SBT<T> Clone() {
			SBT<T> other = new SBT<T>();
			Clone(root, ref other.root);
			return other;
		}

		private static void Clone(Node node, ref Node newNode) {
			if (node == null) return;

			newNode = new Node { Size = node.Size, Value = node.Value };
			Clone(node.Left, ref newNode.Left);
			Clone(node.Right, ref newNode.Right);
		}
	}
}
