public static class ArraysDemo
{
    public static void Run()
    {
        // collection expressions `[ ]` (C# 12+) are the modern, preferred way to initialize an
        // array. They need an explicit target type though (int[], string[]) — `[1, 2, 3]` alone
        // has no inherent type for the compiler to infer, so `var numArray = [1, 2, 3];` won't compile.
        int[] numArray = [1, 2, 3, 4, 5];
        Console.WriteLine($"Number array (collection expression): {string.Join(", ", numArray)}");

        string[] strArray = ["a", "b", "c", "d", "e"];
        Console.WriteLine($"String array (collection expression): {string.Join(", ", strArray)}");

        // the older `new[] { }` syntax still works, and its one real advantage over a collection
        // expression is that it works with `var` — the compiler infers the element type from the
        // values, so you don't have to spell out `int[]`/`string[]` yourself.
        var numArray2 = new[] { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Number array (new[] + var): {string.Join(", ", numArray2)}");

        // `return [];` — a method's declared return type IS a target type, so a collection expression
        // works directly in a return statement (the classic LeetCode "no answer found" case). For an
        // empty array, `[]` compiles to Array.Empty<T>() — a cached singleton, so no allocation, unlike
        // `new int[0]`. The same works for List<T>, IList<T>, IList<IList<int>>, int[][], etc.
        int[] noPair = FindPair([1, 2, 3], 100);
        int[] pair = FindPair([1, 2, 3], 5);
        Console.WriteLine($"FindPair (returns [] when not found): Length={noPair.Length}; found: [{string.Join(", ", pair)}]");
        Console.WriteLine($"Empty [] reuses Array.Empty<int>() (no allocation): {ReferenceEquals(noPair, Array.Empty<int>())}");

        IList<IList<int>> emptyNested = EmptyNested();
        Console.WriteLine($"EmptyNested() returning [] as IList<IList<int>>: Count={emptyNested.Count}");

        // slicing with ranges (C# 8): arr[start..end] is start-inclusive, end-EXCLUSIVE, and ^k means
        // "k from the end" (^1 is the last element). On an array, a range COPIES into a new array,
        // O(k) for k elements; use arr.AsSpan(start, length) for a no-copy view.
        var middle = strArray[1..^1];
        Console.WriteLine($"strArray[1..^1] (drop first and last): {string.Join(", ", middle)}");

        var tail = strArray[3..];
        Console.WriteLine($"strArray[3..] (index 3 to end): {string.Join(", ", tail)}");

        // repeating a value: new string(c, n) builds a string, and Array.Fill sets every slot of an
        // existing array, both O(n). Array.Fill is the go-to for DP arrays that need a sentinel start
        // value (-1 = "not computed yet", int.MaxValue = "unreachable") instead of the default 0.
        var repeatedStr = new string('a', 9);
        Console.WriteLine($"new string('a', 9): {repeatedStr}");

        var memo = new int[5];
        Array.Fill(memo, -1);
        Console.WriteLine($"new int[5] + Array.Fill(memo, -1): {string.Join(", ", memo)}");

        // fixed-length array where every element is default-initialized (0 for int, null for
        // reference types) — common when you need a DP array/output buffer of a known size upfront
        var zeroedArray = new int[2];
        Console.WriteLine($"new int[2] (default-initialized): {string.Join(", ", zeroedArray)}");

        // Length vs Count: arrays expose Length only (no Count) — it's a field baked into the array
        // at creation time, so reading it is O(1) and there's nothing to dynamically recompute (arrays
        // are fixed-size). Resizable BCL collections (List<T>, Dictionary<K,V>, HashSet<T>, etc.) expose
        // Count instead — see ListsDemo for how Count differs from LINQ's Count() extension method.
        Console.WriteLine($"numArray.Length: {numArray.Length}");

        // quick copy — three equivalent one-liners, all O(n) (each copies every element into new storage):
        var arrayClone = (int[])numArray.Clone(); // Clone() returns object, so it needs a cast
        var arrayToArray = numArray.ToArray(); // LINQ — no cast needed
        int[] arraySpread = [.. numArray]; // collection-expression spread (C# 12+)
        Console.WriteLine($"Copies — Clone(): {string.Join(", ", arrayClone)}, ToArray(): {string.Join(", ", arrayToArray)}, [..spread]: {string.Join(", ", arraySpread)}");

        // gotcha: all three are SHALLOW copies — fine here since int is a value type, but for an
        // array of reference-type elements the copy would hold the SAME object references as the
        // original, so mutating a shared element through either array affects both.
        arrayClone[0] = 999;
        Console.WriteLine($"Mutating arrayClone[0] leaves numArray untouched: numArray[0]={numArray[0]}, arrayClone[0]={arrayClone[0]}");

        // reverse IN PLACE — O(n). Arrays have no instance Reverse(), so use the static Array.Reverse.
        int[] reverseArray = [1, 2, 3, 4, 5];
        Array.Reverse(reverseArray);
        Console.WriteLine($"Array after Array.Reverse(arr) (in place): {string.Join(", ", reverseArray)}");

        // gotcha: arr.Reverse() compiles, but since arrays have no instance Reverse() it binds to
        // LINQ's Enumerable.Reverse, which returns a NEW sequence and leaves the array untouched.
        // Discard the result and nothing happens — silently. (List<T> is the opposite: there,
        // list.Reverse() IS the in-place instance method — see ListsDemo.)
        int[] linqReverseArray = [1, 2, 3, 4, 5];
        var reversedArrayCopy = linqReverseArray.Reverse().ToArray(); // O(n) copy
        Console.WriteLine($"arr.Reverse().ToArray() (LINQ copy): {string.Join(", ", reversedArrayCopy)}, original unchanged: {string.Join(", ", linqReverseArray)}");

        // finding by VALUE — all O(n) linear scans. IndexOf/LastIndexOf return -1 when missing.
        // arr.Contains(x) is LINQ's Enumerable.Contains (arrays have no instance Contains).
        int[] findArray = [4, 7, 2, 7, 9];
        Console.WriteLine($"Array.IndexOf(arr, 7): {Array.IndexOf(findArray, 7)}, Array.LastIndexOf(arr, 7): {Array.LastIndexOf(findArray, 7)}, Array.IndexOf(arr, 100): {Array.IndexOf(findArray, 100)}, arr.Contains(9): {findArray.Contains(9)}");

        // finding by PREDICATE — static Array.* methods take a Predicate<T> lambda, all O(n)
        Console.WriteLine($"Array.FindIndex(arr, x => x > 5): {Array.FindIndex(findArray, x => x > 5)}, Array.FindLastIndex(arr, x => x > 5): {Array.FindLastIndex(findArray, x => x > 5)}");
        Console.WriteLine($"Array.FindAll(arr, x => x % 2 == 1): [{string.Join(", ", Array.FindAll(findArray, x => x % 2 == 1))}]");
        Console.WriteLine($"Array.Exists(arr, x => x > 8): {Array.Exists(findArray, x => x > 8)}, Array.TrueForAll(arr, x => x > 0): {Array.TrueForAll(findArray, x => x > 0)}");

        // gotcha: Array.Find / Array.FindLast return default(T) when nothing matches. For int that's
        // 0, which looks like a real value. Use FindIndex (-1 is clearly "missing") or Exists when
        // 0 could be a valid element.
        Console.WriteLine($"Array.Find(arr, x => x > 5): {Array.Find(findArray, x => x > 5)}, Array.Find(arr, x => x > 100) (no match -> default 0): {Array.Find(findArray, x => x > 100)}");

        // Array.BinarySearch — O(log n), but the array MUST be sorted first (else the result is
        // meaningless). Found -> the index. Missing -> a negative number whose bitwise complement
        // (~result) is the insertion point, the index where the value would go to keep it sorted
        // (the same idea as Python's bisect_left / C++'s lower_bound).
        int[] sortedForSearch = [1, 3, 5, 7, 9];
        var foundAt = Array.BinarySearch(sortedForSearch, 7);
        var missingResult = Array.BinarySearch(sortedForSearch, 6);
        Console.WriteLine($"Array.BinarySearch(sorted, 7): {foundAt}; BinarySearch(sorted, 6): {missingResult}, ~result (insertion point): {~missingResult}");

        // gotcha: with DUPLICATES, BinarySearch returns the index of ANY matching element, not
        // necessarily the first one, so it is NOT a drop-in lower_bound. For "first index >= x"
        // (LC 34, Find First and Last Position) write the binary search loop yourself.
        int[] withDuplicates = [1, 2, 2, 2, 2, 2, 3];
        Console.WriteLine($"Array.BinarySearch([1, 2, 2, 2, 2, 2, 3], 2): {Array.BinarySearch(withDuplicates, 2)} (first 2 is at index 1)");
    }

    // Two Sum-style helper: `return [i, j];` builds a new int[] from the return type,
    // and `return [];` is the empty-array fallback
    private static int[] FindPair(int[] nums, int target)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] + nums[j] == target)
                {
                    return [i, j];
                }
            }
        }

        return [];
    }

    // [] also works for interface return types. The compiler picks a concrete type that
    // implements the interface (here a List<IList<int>>)
    private static IList<IList<int>> EmptyNested() => [];
}
