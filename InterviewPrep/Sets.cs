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
    }
}
