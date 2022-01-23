using System;
using System.Collections.Generic;
using System.Linq;

namespace NipahTokenizer
{
	public class ProgressiveList<T> : IProgressiveList<T>, IPersistentList<int>
	{
		public int Count => items.Count;
		public T this[int index]
		{
			get
			{
				return items[index];
			}
			set
			{
				items[index] = value;
			}
		}
		List<T> items;
		int index;
		int begin;

		public List<T> List() => items;
		public void List(List<T> list) => items = list;
		public void CopyList(List<T> list)
		{
			items.Clear();
			items.AddRange(list);
			if(items.Count > 0 && index < 0)
				index = 0;
		}

		public void Clear()
		{
			this.index = -1;
			this.begin = 0;

			items.Clear();
		}

		public void UpdateBegin()
		{
			begin = index;
		}

		public int GetState() => index;
		public void RestoreState(int state) => index = state;

		public void Reset()
		{
			if(items == null || items.Count == 0)
				index = -1;
			else
				index = begin;
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

		public bool VerifyNexts(ProgressiveList<T> nexts)
		{
			T def = default(T);
			bool ret = true;
			int state = GetState();
			T item;
			while(!Equals(item = nexts.Next(), def))
			{
				var nitem = nexts.Next();
				if(Equals(nitem, def) || !Equals(item, nitem))
				{
					ret = false;
					break;
				}
			}
			RestoreState(state);
			return ret;
		}

		public bool ClosureCheck(Predicate<T> open, Predicate<T> close, int startsWith = 0)
		{
			var state = GetState();
			int closures = startsWith;
			T item;
			while(HasNext())
			{
				item = Next();
				if (open(item))
					closures++;
				else if (close(item))
					closures--;
			}
			RestoreState(state);
			return closures == 0;
		}

		public bool First()
		{
			return index == begin;
		}
		public bool First_Back()
		{
			return index - 1 == begin;
		}
		/// <summary>
		/// Return the token that pointer is in
		/// </summary>
		/// <returns>The this.</returns>
		public T This()
		{
			int index = this.index - 1;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}
		/// <summary>
		/// Return the last token of list
		/// </summary>
		/// <returns>The last.</returns>
		public T Last()
		{
			return items[items.Count - 1];
		}
		/// <summary>
		/// Return a token and advance the pointer
		/// </summary>
		/// <returns>The next.</returns>
		public T Next()
		{
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			PointerNext();
			return t;
		}
		public bool TryNext(out T item)
		{
			if(HasNext())
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
		/// <summary>
		/// Return a token and advance the pointer, and continue advancing 
		/// while predicate is match
		/// </summary>
		/// <returns>The next.</returns>
		/// <param name="ignoreThis">Ignore this.</param>
		public T Next(Predicate<T> ignoreThis)
		{
			begin:
			var value = Next();
			if(ignoreThis(value))
				goto begin;
			return value;
		}
		public T NextUntil(Predicate<T> match)
		{
			begin:
			var value = Next();
			if(!match(value))
				goto begin;
			return value;
		}
		/// <summary>
		/// Return a token and advance the pointer, storing the result in specified list
		/// </summary>
		/// <returns>The next.</returns>
		/// <param name="toAdd">To add.</param>
		public T Next(List<T> toAdd)
		{
			var next = Next();
			toAdd.Add(next);
			return next;
		}
		public bool LinkNewInstances = true;
		public ProgressiveListInstance<T> Instantiate()
		{
			return new ProgressiveListInstance<T>(this, LinkNewInstances);
		}
		public ProgressiveListInstance<T> MakeInstance(Predicate<T> @while)
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

		public T[] NextCount(int count)
		{
			T[] array = new T[count];
			for(int i = 0; i < count; i++)
				array[i] = Next();
			return array;
		}
		public T[] NextCount(int count, T[] array)
		{
			for(int i = 0; i < count; i++)
				array[i] = Next();
			return array;
		}
		public Null<T> NullableNext()
		{
			if(index <= -1 || index >= items.Count)
				return null;
			var t = items[index];
			PointerNext();
			return t;
		}
		public bool HasNext()
		{
			if(index <= -1 || index >= items.Count)
				return false;
			return true;
		}
		/// <summary>
		/// Return a token and backwards the pointer
		/// </summary>
		/// <returns>The back.</returns>
		public T Back()
		{
			if(index <= -1)
				return default(T);
			else if(index >= items.Count)
			{
				index = items.Count - 1;
				return items[index];
			}
			var t = items[index];
			PointerBack();
			return t;
		}
		/// <summary>
		/// Previews the next token without moving pointer
		/// </summary>
		/// <returns>The next.</returns>
		public T Preview_Next()
		{
			var index = this.index;
			index++;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}
		/// <summary>
		/// Previews the back token without moving pointer
		/// </summary>
		/// <returns>The back.</returns>
		public T Preview_Back()
		{
			var index = this.index;
			index--;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}
		public T Look_Next()
		{
			var index = this.index;
			//index += 2;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}
		public T Look_Next(int plus)
		{
			var index = this.index;
			index += plus;
			//index += 2;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}
		public T Look_Back()
		{
			var index = this.index;
			index -= 2;
			if(index <= -1 || index >= items.Count)
				return default(T);
			var t = items[index];
			return t;
		}

		/// <summary>
		/// Move's the pointer to the next index
		/// </summary>
		public void PointerNext() => index++;
		/// <summary>
		/// Move's the pointer to a back index
		/// </summary>
		public void PointerBack() => index--;

		public int Pointer => index;

		public ProgressiveList<T> SplitUntil(Predicate<T> predicate, 
		                                     bool includeTruth = true, bool removeSelf = true,
		                                    int startingIndex = 0)
		{
			int count = items.Count;
			var pl = new ProgressiveList<T>();
			for(var i = startingIndex; i < count; i++)
			{
				var item = items[i];
				bool truth = predicate(item);
				if(!truth || truth && includeTruth)
					pl.Add(item);
				if(truth)
					break;
			}
			if(removeSelf)
				items.RemoveAll(i => pl.items.Contains(i));
			return pl;
		}

		public void Add(T item)
		{
			items.Add(item);
			if(index <= -1)
				index = begin;
		}
		public void Remove(T item)
		{
			items.Remove(item);
			if(items.Count == 0)
				index = -1;
		}

		public ProgressiveList(List<T> items)
		{
			this.items = items;
			if(items == null || items.Count == 0)
				index = -1;
			else
				index = begin;
		}
		public ProgressiveList(int capacity)
		{
			this.items = new List<T>(capacity);
			if(items == null || items.Count == 0)
				index = -1;
			else
				index = begin;
		}
		public ProgressiveList()
		{
			items = new List<T>();
			index = -1;
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach(var item in items)
				yield return item;
		}

		public static explicit operator ProgressiveList<T>(List<T> lst)
		{
			return new ProgressiveList<T>(lst);
		}
		public static implicit operator ProgressiveList<T>(T[] array)
		{
			return new ProgressiveList<T>(new List<T>(array));
		}
	}
}
