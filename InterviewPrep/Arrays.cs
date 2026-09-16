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
    }
}
