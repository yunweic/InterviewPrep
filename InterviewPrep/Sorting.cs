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
    }
}
