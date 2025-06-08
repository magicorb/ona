using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.Main.Model;

public static class EnumerableExtensions
{
	public static T Last<T>(this IReadOnlyList<T> list)
		=> list[list.Count - 1];

	public static IEnumerator<T> Memorize<T>(this IEnumerable<T> enumerable)
		=> new MemorizeEnumerator<T>(enumerable);

	class MemorizeEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> enumerator;
		private readonly List<T> enumerated;
		private int position = -1;
		
		public MemorizeEnumerator(IEnumerable<T> enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
			this.enumerated = new List<T>();
		}

		public T Current => this.enumerated[position];

#pragma warning disable CS8603 // Possible null reference return.
		object IEnumerator.Current => Current;
#pragma warning restore CS8603 // Possible null reference return.

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (this.position != this.enumerated.Count - 1)
			{
				position++;
				return true;
			}

			var result = this.enumerator.MoveNext();

			if (result)
			{
				this.enumerated.Add(this.enumerator.Current);
				position++;
			}

			return result;
		}

		public void Reset()
		{
			this.position = -1;
		}
	}
}
