using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ona.App.Model
{
	public static class EnumerableExtensions
	{
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

			object IEnumerator.Current => Current;

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
}
