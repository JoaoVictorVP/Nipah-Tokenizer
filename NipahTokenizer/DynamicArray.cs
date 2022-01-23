using System;
using System.Collections.Generic;
using System.Linq;
using Arr = System.Array;

/// <summary>
/// <b>Pt-BR</b><br/>
/// COMENTÁRIO
/// <br/><b>En-US</b><br/>
/// COMMENTARY
/// </summary>
static class sample { }

namespace NipahTokenizer
{
	/// <summary>
	/// <b>Pt-BR</b><br/>
	/// Uma matrix dinamicamente escalável, que diferentemente de uma lista, não duplica sua capacidade quando demasiados elementos são adicionados a ela
	/// <br/><b>En-US</b><br/>
	/// A dynamic scalable array, that, differently from a list, not double this capacity when it is reached
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DynamicArray<T>
	{
		T[] array;
		int count;

		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Número de elementos presentes na array
		/// <br/><b>En-US</b><br/>
		/// Number of elements present in the array
		/// </summary>
		public int Count => count;
		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Referência atual ao objeto array interno
		/// <br/><b>En-US</b><br/>
		/// Current internal array object
		/// </summary>
		public T[] Array => array;
		public T this[int index]
		{
			get {
				return array[index];
			}
			set {
				array[index] = value;
			}
		}

		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Realiza uma conversão de tipos entre os elementos dessa array
		/// <br/><b>En-US</b><br/>
		/// Cast elements from this array, into another new array
		/// </summary>
		public TNew[] Cast<TNew>()
		{
			TNew[] of = new TNew[count];
			for(int i = 0; i < count; i++)
				of[i] = (TNew)(object)array[i];
			return of;
		}
		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Converte os elementos dessa array, em outra nova array, realizando uma conversão 'As'
		/// <br/><b>En-US</b><br/>
		/// Cast the elements of this array, into another new array, using the 'As' casting
		/// </summary>
		public TNew[] As<TNew>() where TNew : class
		{
			TNew[] of = new TNew[count];
			for(int i = 0; i < count; i++)
				of[i] = array[i] as TNew;
			return of;
		}

		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Itera entre todos os elementos da array, chamando 'iterator' a cada vez
		/// <br/><b>En-US</b><br/>
		/// Iterates over each element of this array, calling 'iterator' on every element
		/// </summary>
		public void ForEach(Action<T> iterator) => Arr.ForEach(array, iterator);

		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Adiciona um novo elemento para esta array
		/// <br/><b>En-US</b><br/>
		/// Adds a new element to this array
		/// </summary>
		public void Add(T item)
		{
			count++;
			Arr.Resize(ref array, count);
			array[count - 1] = item;
		}
		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Remove um elemento desta array
		/// <br/><b>En-US</b><br/>
		/// Remove's an element from this array
		/// </summary>
		public void Remove(T item)
		{
			count--;
			var lst = array[count];
			Arr.Resize(ref array, count);
			if(Equals(lst, item))
				return;
			int toRemove = Arr.IndexOf(array, item);
			if(toRemove > -1) {
				for(int i = toRemove + 1; i < count; i++) {
					array[i] = array[i - 1];
				}
				array[count - 1] = lst;
			}
		}
		/// <summary>
		/// <b>Pt-BR</b><br/>
		/// Limpa todos os elementos dessa array
		/// <br/><b>En-US</b><br/>
		/// Clear all the element from this array
		/// </summary>
		public void Clear()
		{
			count = 0;
			Arr.Resize(ref array, count);
		}


		public int IndexOf(T item) => Arr.IndexOf(array, item);
		public bool Contains(T item) => Arr.IndexOf(array, item) > -1;
		public bool Exists(Predicate<T> predicate) => Arr.Exists(array, predicate);

		public IEnumerator<T> GetEnumerator()
		{
			for(int i = 0; i < count; i++)
				yield return array[i];
		}

		public DynamicArray()
		{
			array = new T[0];
		}
		public DynamicArray(int capacity)
		{
			array = new T[capacity];
		}
		public DynamicArray(T[] src)
		{
			count = src.Length;
			array = new T[count];
			Arr.Copy(src, array, count);
		}
		public DynamicArray(List<T> src)
		{
			count = src.Count;
			array = src.ToArray();
		}
	}
}
