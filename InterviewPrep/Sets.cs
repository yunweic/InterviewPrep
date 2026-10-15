public static class SetsDemo
{
    public static void Run()
    {
        // create a set — HashSet<T> is a hash table like Dictionary, just without the value.
        // Collection expression syntax works fully here (unlike Stack/Queue below) since
        // HashSet<T> has a real Add method for the compiler to call.
        HashSet<int> mySet = [1, 2, 3, 4, 5];
        Console.WriteLine($"HashSet: {string.Join(", ", mySet)}");

        // the older `new HashSet<int> { }` form is exactly equivalent
        var mySet2 = new HashSet<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"HashSet (new HashSet<int> {{ }}): {string.Join(", ", mySet2)}");
        mySet.Add(6); // O(1) average
        Console.WriteLine($"HashSet after Add(6): {string.Join(", ", mySet)}");

        // HashSet<T>.Contains — O(1) average, vs List<T>.Contains which is O(n).
        // This is the single biggest "make it fast enough" move on LeetCode: swap a List you're
        // only calling Contains() on for a HashSet, and an O(n^2) scan becomes O(n).
        Console.WriteLine($"mySet.Contains(3): {mySet.Contains(3)}");
        Console.WriteLine($"mySet.Contains(99): {mySet.Contains(99)}");

        // Add returns bool — false if the element was already present (set is unchanged).
        // Duplicate check + insert in one O(1) call, no separate Contains needed
        // (e.g. "Contains Duplicate": `if (!seen.Add(x)) return true;`).
        bool addedNew = mySet.Add(7);
        bool addedDup = mySet.Add(7);
        Console.WriteLine($"mySet.Add(7) first time: {addedNew}, second time: {addedDup}");

        mySet.Remove(6); // O(1) average
        Console.WriteLine($"HashSet after Remove(6): {string.Join(", ", mySet)}");

        // Remove also returns bool — false if the element wasn't there (no exception thrown)
        bool removedMissing = mySet.Remove(99);
        Console.WriteLine($"mySet.Remove(99) (not present): {removedMissing}");

        // TryGetValue — O(1) average; returns the instance actually stored in the set.
        // Only interesting with a custom comparer, where "equal" values can still differ
        // (here: case-insensitive set, looking up "APPLE" gives back the stored "apple").
        var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "apple", "banana" };
        if (words.TryGetValue("APPLE", out string? stored))
        {
            Console.WriteLine($"words.TryGetValue(\"APPLE\"): found stored value \"{stored}\"");
        }

        // Count (not Length) — HashSet<T> implements ICollection<T>, so Count is an O(1) field read
        // (see ListsDemo for the full Count-property vs LINQ-Count()-extension explanation).
        Console.WriteLine($"mySet.Count: {mySet.Count}");

        // quick copy — O(n): constructor-from-sequence, LINQ ToHashSet(), or spread
        var setCopyCtor = new HashSet<int>(mySet);
        var setToHashSet = mySet.ToHashSet();
        HashSet<int> setSpread = [.. mySet];
        Console.WriteLine($"Copies — ctor: {string.Join(", ", setCopyCtor)}, ToHashSet(): {string.Join(", ", setToHashSet)}, [..spread]: {string.Join(", ", setSpread)}");

        // iterating — foreach is the only direct way, O(n) total. A HashSet has NO index:
        // `mySet[0]` doesn't compile, and there's no for-loop over it. mySet.ElementAt(i) (LINQ)
        // compiles, but it walks from the start every call, so it's O(i) per call, O(n^2) in a loop.
        // Order is NOT guaranteed: it often looks like insertion order for small int sets, but
        // don't rely on that. Use SortedSet (below) or sort a copy if order matters.
        foreach (var item in mySet)
        {
            Console.WriteLine($"foreach over mySet: {item}");
        }

        // the classic LeetCode set-iteration pattern, "Longest Consecutive Sequence": loop over the
        // set, and only start counting from numbers that begin a run (num - 1 not in the set).
        // Each number is visited by the inner while at most once overall, so it's O(n), not O(n^2).
        HashSet<int> consecutive = [100, 4, 200, 1, 3, 2];
        var longest = 0;
        foreach (var num in consecutive)
        {
            if (consecutive.Contains(num - 1))
            {
                continue; // not the start of a run, so a smaller number will count it
            }

            var length = 1;
            while (consecutive.Contains(num + length))
            {
                length++;
            }

            longest = Math.Max(longest, length);
        }
        Console.WriteLine($"Longest consecutive run in {{100, 4, 200, 1, 3, 2}} (foreach + Contains): {longest}");

        // gotcha: Add-ing to a set INSIDE its own foreach throws InvalidOperationException
        // ("Collection was modified"). Remove inside foreach does NOT throw on modern .NET (allowed
        // for HashSet/Dictionary since .NET Core 3.0+), but List<T>.Remove inside foreach still
        // throws, so the fixes below are the habits that are safe for every collection.
        HashSet<int> modifySet = [1, 2, 3];
        try
        {
            foreach (var x in modifySet)
            {
                modifySet.Add(x + 10);
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Add inside foreach threw InvalidOperationException: {ex.Message}");
        }

        // fix 1: RemoveWhere(predicate), which removes every match in one O(n) pass and returns the count removed
        HashSet<int> removeWhereSet = [1, 2, 3, 4, 5, 6];
        var removedCount = removeWhereSet.RemoveWhere(x => x % 2 == 0);
        Console.WriteLine($"RemoveWhere(x => x % 2 == 0) removed {removedCount}: {string.Join(", ", removeWhereSet)}");

        // fix 2: iterate over a COPY ([.. set] or set.ToList()) and modify the original. Costs an
        // O(n) copy, but it's the only option when you need to Add while iterating.
        HashSet<int> copyLoopSet = [1, 2, 3, 4, 5, 6];
        foreach (var x in copyLoopSet.ToList())
        {
            if (x % 2 == 0)
            {
                copyLoopSet.Remove(x);
            }
        }
        Console.WriteLine($"Remove while iterating a ToList() copy: {string.Join(", ", copyLoopSet)}");

        // SortedSet<T> — a balanced tree (Add/Remove/Contains O(log n), vs O(1) for HashSet), but
        // foreach is GUARANTEED to go in ascending order, and Min/Max are O(log n) reads.
        // .Reverse() iterates descending (the instance method on SortedSet, not a LINQ copy).
        SortedSet<int> sortedSet = [5, 1, 4, 2, 3];
        Console.WriteLine($"SortedSet foreach (ascending): {string.Join(", ", sortedSet)}, .Reverse(): {string.Join(", ", sortedSet.Reverse())}, Min: {sortedSet.Min}, Max: {sortedSet.Max}");
    }
}
