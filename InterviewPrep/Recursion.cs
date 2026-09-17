public static class RecursionDemo
{
    public static void Run()
    {
        // basic recursion — clear base case, then a recursive case that shrinks toward it.
        // Factorial: O(n) time, O(n) space (one stack frame per call until the base case).
        Console.WriteLine($"Factorial(5): {Factorial(5)}");

        // naive recursive Fibonacci: each call branches into two more calls, re-solving the
        // same overlapping subproblems repeatedly (Fib(n-2) is recomputed from both Fib(n-1)
        // and the outer call). That branching gives O(2^n) time despite only n stack frames deep.
        Console.WriteLine($"NaiveFibonacci(10): {NaiveFibonacci(10)}");

        // the blowup in practice: call counts roughly double per increment of n, tracking 2^n
        var callCounts = new List<(int N, long Calls)>();
        foreach (var n in new[] { 10, 20, 30 })
        {
            _naiveFibCalls = 0;
            NaiveFibonacciCounted(n);
            callCounts.Add((n, _naiveFibCalls));
        }
        Console.WriteLine(
            $"NaiveFibonacci call counts (exponential growth): {string.Join(", ", callCounts.Select(c => $"n={c.N}->{c.Calls} calls"))}");

        // top-down memoization: cache each n's result the first time it's computed, so every
        // later call for the same n is an O(1) dictionary lookup instead of a re-solve.
        // Fixes the time complexity back to O(n) (n distinct subproblems, each solved once),
        // at the cost of O(n) extra space for the cache (on top of the O(n) call stack).
        var memo = new Dictionary<int, long>();
        Console.WriteLine($"MemoizedFibonacci(40) [would be infeasible naively]: {MemoizedFibonacci(40, memo)}");
        Console.WriteLine($"Memo cache size after MemoizedFibonacci(40): {memo.Count}");

        // stack depth / stack-overflow risk: recursion depth is bounded by the runtime's call
        // stack (typically ~1MB default thread stack in .NET), not by available heap memory.
        // A LeetCode gotcha: a naive recursive traversal of a heavily skewed/degenerate
        // structure (e.g. a linked list or unbalanced tree of 10,000+ nodes) can blow the stack
        // even though an iterative version handling the same input is fine — recursion depth
        // there is O(n), not O(log n) like a balanced tree.
        var deepList = BuildLinkedList(10_000);
        var iterativeLength = IterativeLength(deepList);
        Console.WriteLine($"IterativeLength on a 10,000-node list (safe, O(1) extra space): {iterativeLength}");
        Console.WriteLine(
            "RecursiveLength on the same list would risk a StackOverflowException — " +
            "one stack frame per node, and StackOverflowException cannot be caught in .NET, " +
            "so prefer an iterative walk (or tail-recursion rewritten as a loop) for unbounded-depth inputs.");
    }

    private static long Factorial(int n) =>
        n <= 1 ? 1 : n * Factorial(n - 1); // base case: 0! = 1! = 1

    private static long NaiveFibonacci(int n) =>
        n <= 1 ? n : NaiveFibonacci(n - 1) + NaiveFibonacci(n - 2);

    private static long _naiveFibCalls;

    private static long NaiveFibonacciCounted(int n)
    {
        _naiveFibCalls++;
        return n <= 1 ? n : NaiveFibonacciCounted(n - 1) + NaiveFibonacciCounted(n - 2);
    }

    private static long MemoizedFibonacci(int n, Dictionary<int, long> memo)
    {
        if (n <= 1) return n;
        if (memo.TryGetValue(n, out var cached)) return cached; // O(1) hit instead of re-solving

        var result = MemoizedFibonacci(n - 1, memo) + MemoizedFibonacci(n - 2, memo);
        memo[n] = result;
        return result;
    }

    private static ListNode BuildLinkedList(int length)
    {
        ListNode? head = null;
        for (var i = length; i >= 1; i--)
            head = new ListNode(i, head);
        return head!;
    }

    private static int IterativeLength(ListNode? head)
    {
        var length = 0;
        for (var node = head; node != null; node = node.Next)
            length++;
        return length;
    }
}
