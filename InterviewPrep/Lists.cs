public static class ListsDemo
{
    public static void Run()
    {
        // create a list of int — collection expression syntax (C# 12+) is the modern preferred
        // form: the type is named once, on the left, instead of twice (`new List<int> { }`).
        List<int> numList = [1, 2, 3, 4, 5];
        Console.WriteLine($"Number list: {string.Join(", ", numList)}");

        // the older `new List<int> { }` form still works and is exactly equivalent — you'll see
        // it constantly in existing code, so it's worth recognizing even if you write `[ ]` yourself
        var numList2 = new List<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Number list (new List<int> {{ }}): {string.Join(", ", numList2)}");

        // add all the number in the list — O(n), Sum() walks the whole sequence once
        var total = numList.Sum();
        Console.WriteLine($"Sum of number list: {total}");

        //note that you cannot do this:
        /*
        var testTotal = strArray.Sum();
        Console.WriteLine($"Sum of str array: {testTotal}");*/

        // create a list of string — collection expression again
        List<string> strList = ["a", "b", "c", "d", "e"];
        Console.WriteLine($"String list: {string.Join(", ", strList)}");
        Console.WriteLine($"String list joined with no separator: {string.Join("", strList)}");

        // add an element to list — O(1) amortized (occasional O(n) resize when capacity is exceeded)
        strList.Add("f");
        Console.WriteLine($"String list after Add(\"f\"): {string.Join(", ", strList)}");

        // List<T>.Contains — O(n), linear scan. This is the one to watch for in LeetCode:
        // repeated Contains() calls inside a loop turn an O(n) algorithm into O(n^2).
        Console.WriteLine($"strList.Contains(\"c\"): {strList.Contains("c")}");

        // remove an element from the list — O(n): linear search for the value, then shifts everything after it
        strList.Remove("f");
        Console.WriteLine($"String list after Remove(\"f\"): {string.Join(", ", strList)}");

        // access the last element on the list — O(1), List<T> is backed by an array
        Console.WriteLine($"Last element via strList[^1]: {strList[^1]}");

        // remove a list element at a certain index — O(n) in general (shifts trailing elements),
        // but O(1) here specifically because we're removing the last element (nothing to shift)
        strList.RemoveAt(strList.Count - 1);
        Console.WriteLine($"String list after RemoveAt(last index): {string.Join(", ", strList)}");

        // Length vs Count: List<T> (and Dictionary/HashSet/Queue/Stack) expose a Count PROPERTY, not
        // Length — arrays/strings use Length instead. Both are O(1): List<T> maintains Count as a field
        // that Add/Remove keep up to date, same idea as an array's Length field.
        Console.WriteLine($"strList.Count (property): {strList.Count}");

        // LINQ's Count() EXTENSION METHOD (Enumerable.Count()) is a different thing from the Count
        // property above, and it's evaluated DYNAMICALLY: at runtime it checks whether the source
        // implements ICollection<T> (List<T>/HashSet<T>/Dictionary<K,V>/etc. all do) and, if so, just
        // returns that type's O(1) Count property directly — no enumeration needed.
        Console.WriteLine($"strList.Count() (LINQ extension, but strList IS ICollection<T> so it takes the O(1) fast path): {strList.Count()}");

        // A lazy LINQ query like Where(...) returns a plain IEnumerable<T> with no ICollection<T>
        // behind it (nothing has counted anything yet), so Count() on THAT has no fast path available
        // and has to walk every element to count them — O(n), same cost as if you'd written the loop yourself.
        IEnumerable<int> lazyQuery = numList.Where(n => n > 1);
        Console.WriteLine($"lazyQuery.Count() (LINQ extension, no ICollection<T> backing -> must enumerate, O(n)): {lazyQuery.Count()}");

        // quick copy — three equivalent one-liners, all O(n):
        var listCopyCtor = new List<int>(numList); // constructor-from-sequence
        var listToList = numList.ToList(); // LINQ
        List<int> listSpread = [.. numList]; // collection-expression spread (C# 12+)
        Console.WriteLine($"Copies — ctor: {string.Join(", ", listCopyCtor)}, ToList(): {string.Join(", ", listToList)}, [..spread]: {string.Join(", ", listSpread)}");

        // all three are SHALLOW copies, same reference-vs-value-element caveat as arrays — mutating
        // the copy's own list structure (Add/Remove) never touches the original, though.
        listCopyCtor.Add(999);
        Console.WriteLine($"Mutating listCopyCtor doesn't affect numList: numList={string.Join(", ", numList)}, listCopyCtor={string.Join(", ", listCopyCtor)}");
    }
}
