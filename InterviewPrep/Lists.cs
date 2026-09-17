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
    }
}
