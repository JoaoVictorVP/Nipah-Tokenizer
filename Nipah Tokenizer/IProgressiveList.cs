using System;
using System.Collections.Generic;
using System.Linq;

namespace NipahTokenizer
{
	public interface IProgressiveList<T>
	{
		int Pointer {get;}
		void Reset();
		void ResetBegin();
		void ResetAll();
		bool HasNext();
		T This();
		T Next();
		bool TryNext(out T item);
		bool TryNext(Predicate<T> ignoreThis, out T item);
		T Back();
		T Look_Next();
		T Look_Next(int plus);
		T Look_Back();
		void PointerNext();
		void PointerBack();

		void List(List<T> list);
		void CopyList(List<T> list);
		void Add(T item);
		void Remove(T item);

		T Next(Predicate<T> ignoreThis);
		T Next(List<T> toAdd);
		T NextUntil(Predicate<T> match);

		ProgressiveListInstance<T> Instantiate();
		ProgressiveListInstance<T> MakeInstance(Predicate<T> @while);
		List<T> List();

		bool ClosureCheck(Predicate<T> open, Predicate<T> close, int startsWith = 0);
	}
	public interface IPersistentList<TState>
	{
		TState GetState();
		void RestoreState(TState state);
	}
}
