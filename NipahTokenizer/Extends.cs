using System;
using System.Reflection;
using System.Collections.Generic;

namespace NipahTokenizer
{
	static class Extends
	{
		public static bool Member(this object dyn, string func)
		{
			if(dyn is IDictionary<string, dynamic>)
				return ((IDictionary<string, dynamic>)dyn).ContainsKey(func);

			return dyn.GetType().GetMember(func, BindingFlags.Default|BindingFlags.GetField
			                               |BindingFlags.GetProperty|BindingFlags.InvokeMethod
			                               |BindingFlags.Public|BindingFlags.Static
			                               |BindingFlags.Instance) != null;
		}
	}
	public static class Extends_String
	{
		public static bool wholeInt(this string str)
		{
			foreach(var c in str)
				if(!char.IsDigit(c))
					return false;
			return true;
		}
		public static bool wholeFloat(this string str)
		{
			foreach(var c in str)
				if(!char.IsDigit(c) || c == '.')
					return false;
			return true;
		}
		public static bool isBool(this string str)
		{
			return str == "true" || str == "false";
		}
		public static bool isId(this string str)
		{
			return Token.IsValidIdentifier(str);
		}
		public static bool isOperator(this string str)
		{
			return str == "*" || str == "/" || str == "+" || str == "-";
		}
	}
	public static class Extends_Array
	{
		/* Comparers */
		public static bool Contains<T>(this T[] array, T item)
		{
			foreach(var itm in array)
				if(Equals(itm, item))
					return true;
			return false;
		}
		public static bool Exists<T>(this T[] array, Predicate<T> predicate)
		{
			return Array.Exists(array, predicate);
		}
		public static int IndexOf<T>(this T[] array, T item)
		{
			return Array.IndexOf(array, item);
		}
		public static T Find<T>(this T[] array, Predicate<T> predicate)
		{
			return Array.Find(array, predicate);
		}
		public static void Return<T>(this List<T> list) => Lists<T>.Return(list);
		public static void Return<T>(this T[] array) => Arrays<T>.Return(array);
		public static void Return<T>(this DynamicArray<T> array) => DynamicArrays<T>.Return(array);
		public static void Return<T>(this ProgressiveList<T> array) => 
		ProgressiveLists<T>.Return(array);
		public static List<T> ilnRemoveAll<T>(this List<T> list, Predicate<T> predicate)
		{
			list.RemoveAll(predicate);
			return list;
		}
	}
	public static class Arrays<T>
	{
		static readonly Dictionary<int, Queue<T[]>> pools = new Dictionary<int, Queue<T[]>>();
		static Queue<T[]> getPool(int size)
		{
			Queue<T[]> pool;
			if(!pools.TryGetValue(size, out pool))
				pool = pools[size] = new Queue<T[]>(32);
			return pool;
		}
		public static T[] Get(int size)
		{
			var pool = getPool(size);
			if(pool.Count > 0)
				return pool.Dequeue();
			return new T[size];

		}
		public static void Return(T[] array)
		{
			var pool = getPool(array.Length);
			Array.Clear(array, 0, array.Length);
			pool.Enqueue(array);
		}
	}
	public static class DynamicArrays<T>
	{
		static readonly Queue<DynamicArray<T>> pool = new Queue<DynamicArray<T>>();

		public static DynamicArray<T> Get(bool andReturn = false, int defSize = 1)
		{
			DynamicArray<T> list;
			if(pool.Count == 0)
			{
				for(int i = 0; i < defSize; i++)
					pool.Enqueue(new DynamicArray<T>(32));
				list = new DynamicArray<T>(32);
			}
			else
				list = pool.Dequeue();

			if(andReturn)
				pool.Enqueue(list);

			list.Clear();
			return list;
		}
		public static void Return(DynamicArray<T> list)
		{
			list.Clear();
			pool.Enqueue(list);
		}
	}
	public static class Lists<T>
	{
		static readonly Queue<List<T>> pool = new Queue<List<T>>();

		public static List<T> Get(int defSize = 32)
		{
			List<T> list;
			if(pool.Count == 0)
			{
				list = new List<T>(defSize);
			}
			else
				list = pool.Dequeue();

			return list;
		}
		public static void Return(List<T> list)
		{
			list.Clear();
			pool.Enqueue(list);
		}
	}
	public static class ProgressiveLists<T>
	{
		static readonly Queue<ProgressiveList<T>> pool = new Queue<ProgressiveList<T>>();

		public static ProgressiveList<T> Get(bool andReturn = false, int defSize = 1)
		{
			ProgressiveList<T> list;
			if(pool.Count == 0)
			{
				for(int i = 0; i < defSize; i++)
					pool.Enqueue(new ProgressiveList<T>(32));
				list = new ProgressiveList<T>(32);
			}
			else
				list = pool.Dequeue();

			if(andReturn)
				pool.Enqueue(list);

			list.Clear();
			return list;
		}
		public static void Return(ProgressiveList<T> list)
		{
			list.Clear();
			pool.Enqueue(list);
		}
	}
}
