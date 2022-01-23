using System;

namespace NipahTokenizer
{
	/// <summary>
	/// A nullable type (any)
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public struct Null<T>
	{
		public bool HasValue => hasValue;
		public T Value => boxed;
		T boxed;
		bool hasValue;

		public static implicit operator bool (Null<T> n) => n.hasValue;

		public static implicit operator T (Null<T> n) => n.boxed;
		public static implicit operator Null<T> (T toBox) => new Null<T>(toBox);
		public static implicit operator Null<T> (DBNull nul)
		{
			return new Null<T>(true);
		}

		public Null(T toBox)
		{
			boxed = toBox;
			hasValue = true;
		}
		public Null(bool nul)
		{
			boxed = default(T);
			hasValue = false;
		}

		public override string ToString()
		{
			if(hasValue)
				return boxed.ToString();
			return $"({nameof(T)} Null";
		}
	}
}
