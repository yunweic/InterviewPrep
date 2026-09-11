public static class StacksDemo
{
    public static void Run()
    {
        //stack — backed by an array; Push/Pop/Peek/TryPush/TryPop/TryPeek are all O(1) amortized
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        foreach (var item in stack)
            Console.WriteLine($"Stack item (foreach, top-to-bottom order): {item}");

        Console.WriteLine($"Stack.Peek() (top item, not removed): {stack.Peek()}");
        var poppedItem = stack.Pop();
        Console.WriteLine($"Stack.Pop() (removed top item): {poppedItem}");

        stack.Clear();
        bool success = stack.TryPeek(out int value);
        Console.WriteLine($"TryPeak result = {success},  value = {value}");
        // value == default(int) == 0   (for reference types, it'd be null)

        bool popped = stack.TryPop(out int poppedValue);
        Console.WriteLine($"TryPop() result = {popped},  value = {poppedValue}");
        // poppedValue == default(int) == 0
    }
}
