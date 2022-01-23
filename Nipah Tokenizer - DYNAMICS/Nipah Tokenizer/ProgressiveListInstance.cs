using System;
using System.Collections.Generic;
using System.Linq;

namespace NipahTokenizer
{
	public struct ProgressiveListInstance<T> : IProgressiveList<T>
	{
		ProgressiveList<T> link;

		public void Fix() => link.RestoreState(pointer);

		public List<T> List() => items;
		public int Pointer => pointer;
		public int Max
		{
			get {
				return max;
			}
			set {
				max = value;
			}
		}
		List<T> items;
		int pointer;
		int begin;
		int max;

		public void List(List<T> list) => items = list;
		public void CopyList(List<T> list)
		{
			items.Clear();
			items.AddRange(list);
			if(items.Count > 0 && pointer < 0)
				pointer = 0;
		}

		public void Add(T item)
		{
			items.Add(item);
			if(pointer <= -1)
				pointer = begin;
		}
		public void Remove(T item)
		{
			items.Remove(item);
			if(items.Count == 0)
				pointer = -1;
		}

		public bool ClosureCheck(Predicate<T> open, Predicate<T> close, int startsWith = 0)
		{
			var state = pointer;
			int closures = startsWith;
			T item;
			while (HasNext())
			{
				item = Next();
				if (open(item))
					closures++;
				else if (close(item))
					closures--;
			}
			pointer = state;
			return closures == 0;
		}

		ProgressiveListInstance<T> IProgressiveList<T>.Instantiate()
		{
			return new ProgressiveListInstance<T>(this);
		}
		ProgressiveListInstance<T> IProgressiveList<T>.MakeInstance(Predicate<T> @while)
		{
			var inst = new ProgressiveListInstance<T>(this);
			T n;
			int max = inst.Pointer;
			while(inst.HasNext())
			{
				n = inst.Next();
				if(!@while(n))
					break;
				max++;
			}
			inst.Max = max;

			inst.Reset();
			return inst;
		}
		public void Reset()
		{
			if(items == null || items.Count == 0)
				pointer = -1;
			else
				pointer = begin;
		}
		public void ResetBegin()
		{
			begin = 0;
		}
		public int GetBegin() => begin;

		public void ResetAll()
		{
			Reset();
			ResetBegin();
		}

		public bool HasNext()
		{
			if(pointer <= -1 || pointer >= items.Count || (max > -1 && pointer >= max))
				return false;
			return true;
		}
		public T This()
		{
			var pointer = this.pointer - 1;
			if(pointer <= -1 || pointer >= items.Count || (max > -1 && pointer >= max))
				return default(T);
			var t = items[pointer];
			return t;
		}
		public T Next()
		{
			if(pointer <= -1 || pointer >= items.Count || (max > -1 && pointer >= max))
				return default(T);
			var t = items[pointer];
			PointerNext();
			return t;
		}
		public bool TryNext(out T item)
		{
			if (HasNext())
			{
				item = Next();
				return true;
			}
			item = default;
			return false;
		}
		public bool TryNext(Predicate<T> ignore, out T item)
		{
			if(HasNext())
			{
				item = Next(ignore);
				return true;
			}
			item = default;
			return false;
		}
		public T Next(Predicate<T> ignoreThis)
		{
			begin:
			var value = Next();
			if(ignoreThis(value))
				goto begin;
			return value;
		}
		public T Next(List<T> toAdd)
		{
			var next = Next();
			toAdd.Add(next);
			return next;
		}
		public T Back()
		{
			if(pointer <= -1)
				return default(T);
			else if(pointer >= items.Count || (max > -1 && pointer >= max))
			{
				pointer = items.Count - 1;
				return items[pointer];
			}
			var t = items[pointer];
			PointerBack();
			return t;
		}
		public T NextUntil(Predicate<T> match)
		{
			begin:
			var value = Next();
			if(!match(value))
				goto begin;
			return value;
		}
		public T Look_Next()
		{
			var index = this.pointer;
			//index += 2;
			if(index <= -1 || index >= items.Count || (max > -1 && pointer >= max))
				return default(T);
			var t = items[index];
			return t;
		}
		public T Look_Next(int plus)
		{
			var index = this.pointer;
			index += plus;
			//index += 2;
			if(index <= -1 || index >= items.Count || (max > -1 && pointer >= max))
				return default(T);
			var t = items[index];
			return t;
		}
		public T Look_Back()
		{
			var index = this.pointer;
			index -= 2;
			if(index <= -1 || index >= items.Count || (max > -1 && pointer >= max))
				return default(T);
			var t = items[index];
			return t;
		}
		public bool HasOnScope(Predicate<T> expect, Predicate<T> closure)
		{
			T cur;
			while(pointer < items.Count)
			{
				cur = Next();
				if(expect(cur))
					return true;
				if(closure(cur))
					return false;
			}
			return false;
		}

		public void PointerNext() => pointer++;
		public void PointerBack() => pointer--;

		internal ProgressiveListInstance(IProgressiveList<T> list, bool bind = true)
		{
			link = bind ? list as ProgressiveList<T> : null;
			items = list.List();
			pointer = list.Pointer;
			begin = list.Pointer;
			max = -1;
		}
	}
}
