public static class SortingDemo
{
    public static void Run()
    {
        // Array.Sort — in-place, O(n log n), default ascending order for primitives
        var nums = new[] { 5, 3, 1, 4, 2 };
        Array.Sort(nums);
        Console.WriteLine($"Array.Sort (ascending): {string.Join(", ", nums)}");

        // custom comparer via a Comparison<T> lambda — descending order
        var nums2 = new[] { 5, 3, 1, 4, 2 };
        Array.Sort(nums2, (a, b) => b - a); // negative if a should come first
        Console.WriteLine($"Array.Sort descending via (a, b) => b - a: {string.Join(", ", nums2)}");

        // List<T>.Sort works the same way, in-place
        var numsList = new List<int> { 5, 3, 1, 4, 2 };
        numsList.Sort((a, b) => a - b);
        Console.WriteLine($"List.Sort ascending: {string.Join(", ", numsList)}");

        // the classic LeetCode pattern: sorting intervals (int[][]) by start index.
        // Array.Sort/List.Sort are NOT guaranteed stable — ties can reorder. Use LINQ's
        // OrderBy when tie-breaking order matters (OrderBy IS a stable sort).
        int[][] intervals = [[3, 5], [1, 4], [2, 6]];
        Array.Sort(intervals, (a, b) => a[0] - b[0]);
        Console.WriteLine($"Intervals sorted by start: {string.Join(", ", intervals.Select(iv => $"[{iv[0]},{iv[1]}]"))}");

        // LINQ OrderBy / OrderByDescending / ThenBy — doesn't sort in place, returns a new sequence.
        // Handy for multi-key sorts and when you want to keep the original array untouched.
        int[][] intervals2 = [[3, 5], [1, 4], [1, 2]];
        var sorted = intervals2
            .OrderBy(iv => iv[0])
            .ThenByDescending(iv => iv[1])
            .ToArray();
        Console.WriteLine($"OrderBy(start).ThenByDescending(end): {string.Join(", ", sorted.Select(iv => $"[{iv[0]},{iv[1]}]"))}");

        // sorting strings/objects with a key selector instead of a raw comparison
        var words = new List<string> { "banana", "kiwi", "apple" };
        words.Sort((a, b) => a.Length - b.Length); // shortest first
        Console.WriteLine($"Sort by length: {string.Join(", ", words)}");

        // gotcha: the (a, b) => a - b comparer can OVERFLOW on large values. int.MinValue - 1 wraps
        // around to int.MaxValue, so the sign comes out wrong and the sort silently misorders.
        // a.CompareTo(b) (or b.CompareTo(a) for descending) never overflows, so use it when values
        // can be near int.MinValue/MaxValue.
        int[] extremes = [int.MaxValue, -1, int.MinValue, 0];
        Array.Sort(extremes, (a, b) => a.CompareTo(b));
        Console.WriteLine($"Array.Sort with a.CompareTo(b) (overflow-safe): {string.Join(", ", extremes)}");

        // sort only a RANGE in place: Array.Sort(arr, startIndex, length), O(k log k) for k elements.
        // Example: "Next Permutation" sorts (or reverses) just the suffix.
        int[] partial = [9, 8, 5, 3, 1, 2];
        Array.Sort(partial, 2, 4); // sorts indices 2..5 only
        Console.WriteLine($"Array.Sort(arr, 2, 4) (sort indices 2..5 only): {string.Join(", ", partial)}");

        // Array.Sort(keys, items) sorts BOTH arrays by the keys array: two parallel arrays stay
        // aligned without building tuples (e.g. sort names by their scores)
        int[] scores = [90, 70, 85];
        string[] names = ["Ann", "Bob", "Cat"];
        Array.Sort(scores, names);
        Console.WriteLine($"Array.Sort(keys, items) — scores: {string.Join(", ", scores)}, names aligned: {string.Join(", ", names)}");
    }
}
