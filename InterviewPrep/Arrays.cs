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

        // create a new array
        var subset = strArray[1..^1];
        Console.WriteLine($"Subset: {string.Join(", ", subset)}");

        var subset2 = strArray[3..];
        Console.WriteLine($"Subset: {string.Join(", ", subset2)}");

        var repeatedStr = new string('a', 9);
        Console.WriteLine($"Repeated string ('a' x 9): {repeatedStr}");

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
    }
}
