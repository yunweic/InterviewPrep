public static class DictionariesDemo
{
    public static void Run()
    {
        // create a dictionary — index-initializer syntax `[key] = value` (C# 6+) is the preferred
        // modern form: it reads like how you'll actually access the dictionary later (dict[1]),
        // and a duplicate key here just overwrites (last one wins) instead of throwing.
        var lookupTable = new Dictionary<int, string>
        {
            [1] = "a",
            [2] = "b",
        };
        Console.WriteLine($"Dictionary (key-value pairs): {string.Join(", ", lookupTable)}");

        // the older `{ key, value }` add-pair form still works — under the hood each pair calls
        // Add(), so a duplicate key here throws ArgumentException at initialization time instead
        // of silently overwriting. That can be a feature (catches accidental dupes) or a footgun,
        // depending on whether you expect your keys to be unique going in.
        var lookupTable2 = new Dictionary<int, string>
        {
            { 1, "a" },
            { 2, "b" },
        };
        Console.WriteLine($"Dictionary via {{ key, value }} add-pairs: {string.Join(", ", lookupTable2)}");

        // Note: there is no dictionary literal via `[ ]` collection expressions (C# 12+) —
        // `Dictionary<int, string> d = [1: "a"];` is NOT valid syntax in current C#, unlike
        // arrays/lists. Collection expressions don't (yet) cover key-value pairs.

        // Dictionary<K,V> is a hash table — indexer get/set, Add, TryAdd, ContainsKey, TryGetValue, and
        // Remove are all O(1) average case (O(n) worst case on hash collisions, which is rare in practice).

        // add or overwrite via indexer — O(1) average
        lookupTable[3] = "c";
        Console.WriteLine($"Dictionary after lookupTable[3] = \"c\": {string.Join(", ", lookupTable)}");

        // Add throws if the key already exists; indexer assignment does not — O(1) average
        lookupTable.Add(4, "d");
        Console.WriteLine($"Dictionary after Add(4, \"d\"): {string.Join(", ", lookupTable)}");

        // TryAdd is the non-throwing version of Add — O(1) average
        bool added = lookupTable.TryAdd(4, "z");
        Console.WriteLine($"TryAdd(4, \"z\") result = {added} (false, key 4 already exists, value unchanged)");

        // ContainsKey vs indexer access — indexing a missing key throws KeyNotFoundException — O(1) average
        Console.WriteLine($"ContainsKey(5): {lookupTable.ContainsKey(5)}");

        // TryGetValue is the safe way to read a possibly-missing key (avoids a ContainsKey + indexer double-lookup) — O(1) average
        bool found = lookupTable.TryGetValue(2, out string? foundValue);
        Console.WriteLine($"TryGetValue(2) result = {found}, value = {foundValue}");

        // GetValueOrDefault avoids the out-parameter entirely when you just want a fallback — O(1) average
        Console.WriteLine($"GetValueOrDefault(99, \"none\"): {lookupTable.GetValueOrDefault(99, "none")}");

        // remove a key — O(1) average
        lookupTable.Remove(4);
        Console.WriteLine($"Dictionary after Remove(4): {string.Join(", ", lookupTable)}");

        // iterate keys / values separately
        Console.WriteLine($"Keys: {string.Join(", ", lookupTable.Keys)}");
        Console.WriteLine($"Values: {string.Join(", ", lookupTable.Values)}");
        foreach (var key in lookupTable.Keys)
        {
            Console.WriteLine($"{key} = {lookupTable[key]}");
        }

        // the classic LeetCode frequency-counter pattern: GetValueOrDefault + indexer assignment
        var charCounts = new Dictionary<char, int>();
        foreach (var c in "banana")
            charCounts[c] = charCounts.GetValueOrDefault(c, 0) + 1;
        Console.WriteLine($"Frequency map of \"banana\": {string.Join(", ", charCounts)}");
    }
}
