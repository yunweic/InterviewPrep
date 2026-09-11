public static class ArraysDemo
{
    public static void Run()
    {
        // create a number array
        var numArray = new[] { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Number array: {string.Join(", ", numArray)}");

        // create a string array
        var strArray = new[] { "a", "b", "c", "d", "e" };
        Console.WriteLine($"String array: {string.Join(", ", strArray)}");

        var subset = strArray[1..^1];
        Console.WriteLine($"Subset: {string.Join(", ", subset)}");

        var subset2 = strArray[3..];
        Console.WriteLine($"Subset: {string.Join(", ", subset2)}");

        var repeatedStr = new string('a', 9);
        Console.WriteLine($"Repeated string ('a' x 9): {repeatedStr}");
    }
}
