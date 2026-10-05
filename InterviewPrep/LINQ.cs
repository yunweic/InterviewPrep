public static class LinqDemo
{
    public static void Run()
    {
        int[] nums = [5, 3, 8, 1, 9, 2, 8];

        // LINQ = extension methods on IEnumerable<T> (any array, List, HashSet, Dictionary, string...).
        // Two syntaxes compile to the SAME method calls. Method syntax is what you'll write in interviews;
        // query syntax is worth recognizing (it reads like SQL and shows up in older code).
        var evensSquaredMethod = nums.Where(x => x % 2 == 0).Select(x => x * x);
        var evensSquaredQuery = from x in nums
                                where x % 2 == 0
                                select x * x;
        Console.WriteLine($"Method syntax Where(even).Select(x*x): [{string.Join(", ", evensSquaredMethod)}], query syntax: [{string.Join(", ", evensSquaredQuery)}]");

        // filter / project — Where and Select are O(n), one pass
        Console.WriteLine($"Where(x => x > 4): [{string.Join(", ", nums.Where(x => x > 4))}]");
        Console.WriteLine($"Select(x => x * 10): [{string.Join(", ", nums.Select(x => x * 10))}]");

        // Select / Where also have an overload that passes the INDEX as a second lambda parameter
        Console.WriteLine($"Select((x, i) => $\"{{i}}:{{x}}\"): [{string.Join(", ", nums.Select((x, i) => $"{i}:{x}"))}]");

        // SelectMany flattens — one output sequence from many inner sequences. O(total number of elements).
        // e.g. flatten a jagged array / grid, or the neighbor lists of a graph.
        int[][] jagged = [[1, 2], [3], [4, 5, 6]];
        Console.WriteLine($"SelectMany(row => row) on [[1,2],[3],[4,5,6]]: [{string.Join(", ", jagged.SelectMany(row => row))}]");

        // checks — Any / All / Contains SHORT-CIRCUIT: they stop at the first element that decides the
        // answer. O(n) worst case, but often much less.
        Console.WriteLine($"Any(x => x > 8): {nums.Any(x => x > 8)}, All(x => x > 0): {nums.All(x => x > 0)}, Contains(4): {nums.Contains(4)}");

        // "is it empty?" — use Any(), not Count() > 0. On a plain IEnumerable (like a Where query),
        // Count() walks EVERYTHING, while Any() stops after the first element.
        // (on an array/List, just use Length / Count — the analyzer even warns about Any() there, CA1860)
        var bigOnes = nums.Where(x => x > 100);
        Console.WriteLine($"nums.Where(x => x > 100).Any(): {bigOnes.Any()}");

        // element access — the plain versions THROW when nothing matches (InvalidOperationException);
        // the OrDefault versions return default (0 / null) instead, same idea as the Try* methods.
        Console.WriteLine($"First(x => x > 4): {nums.First(x => x > 4)}, Last(x => x > 4): {nums.Last(x => x > 4)}");
        Console.WriteLine($"FirstOrDefault(x => x > 100): {nums.FirstOrDefault(x => x > 100)} (default int), FirstOrDefault(x => x > 100, -1): {nums.FirstOrDefault(x => x > 100, -1)} (custom default)");

        // Single — like First, but ALSO throws if MORE than one element matches. Use it to assert "exactly one".
        Console.WriteLine($"Single(x => x == 9): {nums.Single(x => x == 9)}");

        // ElementAt(i) / Last() are O(1) on arrays and Lists (LINQ checks for IList<T> at runtime and
        // uses the indexer — same "evaluated dynamically" idea as Count() in ListsDemo), but O(n) on a
        // plain IEnumerable like a Where query, which has no indexer and must be walked.
        var filtered = nums.Where(x => x > 2);
        Console.WriteLine($"filtered.ElementAt(2) (O(n) — walks the query): {filtered.ElementAt(2)}");

        // aggregates — all O(n). Sum / Max / Min are also in MathDemo.
        Console.WriteLine($"Sum(): {nums.Sum()}, Average(): {nums.Average():0.00}, Max(): {nums.Max()}, Min(): {nums.Min()}");

        // MaxBy / MinBy (.NET 6+) — return the ELEMENT with the largest/smallest key, not the key itself.
        // Before these existed you'd write OrderByDescending(...).First() — O(n log n) instead of O(n).
        string[] words = ["kiwi", "banana", "fig", "cherry"];
        Console.WriteLine($"words.MaxBy(w => w.Length): {words.MaxBy(w => w.Length)}, words.MinBy(w => w.Length): {words.MinBy(w => w.Length)}, vs Max(w => w.Length) (just the key): {words.Max(w => w.Length)}");

        // Aggregate — a general "fold": carries an accumulator through the sequence. O(n).
        // Sum / Max / etc. are special cases of it.
        var product = nums.Aggregate(1L, (acc, x) => acc * x); // seed 1L -> accumulates in long, avoids int overflow
        var xorAll = nums.Aggregate((acc, x) => acc ^ x); // no seed -> starts from the first element
        Console.WriteLine($"Aggregate(1L, (acc, x) => acc * x) (product): {product}, Aggregate((acc, x) => acc ^ x) (XOR of all): {xorAll}");

        // GroupBy — buckets elements by a key. O(n), backed by a hash table internally.
        // Classic: Group Anagrams (LC 49) — words with the same sorted letters share a group.
        string[] anagramWords = ["eat", "tea", "tan", "ate", "nat", "bat"];
        var anagramGroups = anagramWords.GroupBy(w => new string(w.Order().ToArray()));
        foreach (var group in anagramGroups)
            Console.WriteLine($"GroupBy(sorted letters) — key \"{group.Key}\": [{string.Join(", ", group)}]");

        // frequency map — three ways to count occurrences, all O(n):
        //   1) manual loop with GetValueOrDefault — what you'd write by default, works everywhere
        //   2) CountBy (.NET 9+) — one call, yields KeyValuePair<key, count>
        //   3) GroupBy + ToDictionary — the pre-.NET 9 LINQ way
        var freqManual = new Dictionary<int, int>();
        foreach (var x in nums)
            freqManual[x] = freqManual.GetValueOrDefault(x) + 1;
        var freqCountBy = nums.CountBy(x => x).ToDictionary();
        var freqGroupBy = nums.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        Console.WriteLine($"Frequency map — manual: {string.Join(", ", freqManual)} | CountBy: {string.Join(", ", freqCountBy)} | GroupBy+ToDictionary: {string.Join(", ", freqGroupBy)}");

        // ToDictionary(keySelector, valueSelector) — THROWS on a duplicate key, like Dictionary.Add
        var wordLengths = words.ToDictionary(w => w, w => w.Length);
        Console.WriteLine($"words.ToDictionary(w => w, w => w.Length): {string.Join(", ", wordLengths)}");

        // ToLookup — like a Dictionary<K, List<V>> built in one call, and a missing key returns an EMPTY
        // sequence instead of throwing. Read-only once built.
        var byLength = words.ToLookup(w => w.Length);
        Console.WriteLine($"words.ToLookup(w => w.Length) — [6]: [{string.Join(", ", byLength[6])}], [99] (missing key, no throw): [{string.Join(", ", byLength[99])}]");

        // set-style operations — O(n + m), each builds a HashSet internally. Results keep first-seen order.
        int[] other = [8, 9, 10];
        Console.WriteLine($"Distinct(): [{string.Join(", ", nums.Distinct())}]");
        Console.WriteLine($"Union([8,9,10]): [{string.Join(", ", nums.Union(other))}], Intersect: [{string.Join(", ", nums.Intersect(other))}], Except: [{string.Join(", ", nums.Except(other))}]");

        // DistinctBy (.NET 6+) — dedupe by a key, keeping the FIRST element seen for each key
        Console.WriteLine($"words.DistinctBy(w => w.Length): [{string.Join(", ", words.DistinctBy(w => w.Length))}]");

        // slicing — Skip / Take are O(skip + take) on a plain sequence. TakeWhile / SkipWhile stop at the
        // first element that FAILS the condition (unlike Where, which checks every element).
        Console.WriteLine($"Skip(2): [{string.Join(", ", nums.Skip(2))}], Take(3): [{string.Join(", ", nums.Take(3))}]");
        Console.WriteLine($"TakeWhile(x => x > 2): [{string.Join(", ", nums.TakeWhile(x => x > 2))}], SkipWhile(x => x > 2): [{string.Join(", ", nums.SkipWhile(x => x > 2))}]");

        // Chunk (.NET 6+) — split into arrays of size k (the last one may be shorter). O(n).
        Console.WriteLine($"Chunk(3): {string.Join(" ", nums.Chunk(3).Select(c => $"[{string.Join(", ", c)}]"))}");

        // Zip — pairs up elements from two sequences by position, stops at the SHORTER one. O(min(n, m)).
        // With no selector it yields tuples.
        string[] names = ["a", "b", "c"];
        Console.WriteLine($"names.Zip(nums): [{string.Join(", ", names.Zip(nums))}], names.Zip(nums, (n, x) => n + x): [{string.Join(", ", names.Zip(nums, (n, x) => n + x))}]");

        // generating sequences — Enumerable.Range(start, COUNT) (not an end index!) and Repeat
        Console.WriteLine($"Enumerable.Range(1, 5): [{string.Join(", ", Enumerable.Range(1, 5))}], Enumerable.Repeat(\"x\", 3): [{string.Join(", ", Enumerable.Repeat("x", 3))}]");

        // Range + Select is a quick way to build a 2D grid / dp table as a jagged array
        int[][] grid = Enumerable.Range(0, 3).Select(_ => new int[4]).ToArray();
        Console.WriteLine($"Enumerable.Range(0, 3).Select(_ => new int[4]).ToArray(): {grid.Length} rows x {grid[0].Length} cols");

        // ordering — OrderBy is O(n log n) and returns a NEW sequence (the source is untouched).
        // Full coverage (ThenBy, custom comparers, stability) lives in SortingDemo.
        Console.WriteLine($"OrderByDescending(x => x): [{string.Join(", ", nums.OrderByDescending(x => x))}], nums unchanged: [{string.Join(", ", nums)}]");

        // DEFERRED EXECUTION — the #1 LINQ gotcha. Where / Select / OrderBy etc. don't run when you write
        // them; they build a recipe that runs each time you ENUMERATE it (foreach, string.Join, Count()...).
        //   - changes to the source after building the query ARE seen by the query
        //   - enumerating twice does all the work twice
        // ToList() / ToArray() / ToDictionary() run it ONCE and store the result ("materialize").
        List<int> source = [1, 2, 3];
        var deferred = source.Where(x => x > 1);
        var materialized = source.Where(x => x > 1).ToList();
        source.Add(10);
        Console.WriteLine($"After source.Add(10) — deferred query sees it: [{string.Join(", ", deferred)}], ToList() snapshot doesn't: [{string.Join(", ", materialized)}]");

        var lambdaCalls = 0;
        var counted = source.Select(x => { lambdaCalls++; return x * 2; });
        _ = counted.Sum();
        _ = counted.Max();
        Console.WriteLine($"Deferred query enumerated twice (Sum then Max) -> lambda ran {lambdaCalls} times for {source.Count} elements");

        lambdaCalls = 0;
        var countedOnce = source.Select(x => { lambdaCalls++; return x * 2; }).ToList();
        _ = countedOnce.Sum();
        _ = countedOnce.Max();
        Console.WriteLine($"Materialized with ToList() first, then Sum and Max -> lambda ran {lambdaCalls} times for {source.Count} elements");

        // when NOT to use LINQ in an interview: hidden O(n) calls inside a loop. Each Count(pred) below
        // re-scans the whole array, so the loop is O(n^2). Build a frequency map once (O(n)) instead,
        // then each lookup is O(1).
        var slowCounts = nums.Select(x => nums.Count(y => y == x)); // O(n^2)
        var fastCounts = nums.Select(x => freqManual[x]); // O(n) total, map built above
        Console.WriteLine($"Count of each element — O(n^2) Count(pred) inside Select: [{string.Join(", ", slowCounts)}], O(n) via frequency map: [{string.Join(", ", fastCounts)}]");
    }
}
