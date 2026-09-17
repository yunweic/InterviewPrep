public static class StacksDemo
{
    public static void Run()
    {
        //stack — backed by an array; Push/Pop/Peek/TryPush/TryPop/TryPeek are all O(1) amortized.
        // Unlike List/HashSet/Dictionary, Stack<T> has no Add method (only Push), so it does NOT
        // support a populated collection expression — `Stack<int> s = [1, 2, 3];` fails to compile.
        // `Stack<int> s = [];` (empty only) does work, as sugar for `new Stack<int>()`.
        //
        // To pre-populate, the constructor-from-sequence is cleanest: it pushes in enumeration
        // order, so the LAST element ends up on top.
        var stack = new Stack<int>([1, 2, 3]);
        Console.WriteLine($"Stack from constructor [1, 2, 3] -> Peek() (top): {stack.Peek()}");

        stack = new Stack<int>();
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

        // `Stack<int> s = [];` — the empty collection expression, equivalent to `new Stack<int>()`
        Stack<int> emptyStack = [];
        emptyStack.Push(1);
        Console.WriteLine($"Stack.Peek() after Push(1) on an empty-collection-expression stack: {emptyStack.Peek()}");
    }
}
