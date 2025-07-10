using System.Collections;

namespace Ona.Main.Model;

public static class EnumerableExtensions
{
    public static T Last<T>(this IReadOnlyList<T> list)
        => list[list.Count - 1];

    public static bool TryLast<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T result)
    {
        result = default!;
        var found = false;

        foreach (var item in source)
        {
            if (predicate(item))
            {
                result = item;
                found = true;
            }
        }

        return found;
    }
}
