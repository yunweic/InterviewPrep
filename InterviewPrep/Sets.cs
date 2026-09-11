public static class SetsDemo
{
    public static void Run()
    {
        // create a set — HashSet<T> is a hash table like Dictionary, just without the value
        var mySet = new HashSet<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"HashSet: {string.Join(", ", mySet)}");
        mySet.Add(6); // O(1) average
        Console.WriteLine($"HashSet after Add(6): {string.Join(", ", mySet)}");

        // HashSet<T>.Contains — O(1) average, vs List<T>.Contains which is O(n).
        // This is the single biggest "make it fast enough" move on LeetCode: swap a List you're
        // only calling Contains() on for a HashSet, and an O(n^2) scan becomes O(n).
        Console.WriteLine($"mySet.Contains(3): {mySet.Contains(3)}");

        mySet.Remove(6); // O(1) average
        Console.WriteLine($"HashSet after Remove(6): {string.Join(", ", mySet)}");
    }
}
