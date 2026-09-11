public static class HeapsDemo
{
    public static void Run()
    {
        // PriorityQueue<TElement, TPriority> — a MIN-heap by default: Dequeue always returns
        // the element with the LOWEST priority first. Enqueue/Dequeue/Peek are all O(log n);
        // there's no built-in TPriority-only overload for value types like int — you enqueue
        // (element, priority) pairs, and often element == priority for simple cases.
        var minHeap = new PriorityQueue<int, int>();
        minHeap.Enqueue(5, 5);
        minHeap.Enqueue(1, 1);
        minHeap.Enqueue(3, 3);
        minHeap.Enqueue(2, 2);

        Console.WriteLine($"Peek() (lowest priority, not removed): {minHeap.Peek()}");

        var order = new List<int>();
        while (minHeap.Count > 0)
            order.Add(minHeap.Dequeue());
        Console.WriteLine($"Dequeue order (ascending): {string.Join(", ", order)}");

        // TryDequeue is the non-throwing version, same idea as List/Stack/Queue's TryX methods
        var empty = new PriorityQueue<int, int>();
        bool got = empty.TryDequeue(out int elementResult, out int priorityResult);
        Console.WriteLine($"TryDequeue on empty heap: {got} (element={elementResult}, priority={priorityResult})");

        // MAX-heap: PriorityQueue has no built-in max-heap mode. Two common workarounds:
        // 1) negate the priority on the way in
        var maxHeapByNegation = new PriorityQueue<int, int>();
        foreach (var n in new[] { 5, 1, 3, 2 })
            maxHeapByNegation.Enqueue(n, -n);
        Console.WriteLine($"Max-heap via negated priority — first Dequeue (largest): {maxHeapByNegation.Dequeue()}");

        // 2) supply a custom IComparer<TPriority> that reverses the comparison
        var maxHeapByComparer = new PriorityQueue<int, int>(Comparer<int>.Create((a, b) => b - a));
        foreach (var n in new[] { 5, 1, 3, 2 })
            maxHeapByComparer.Enqueue(n, n);
        Console.WriteLine($"Max-heap via reversed comparer — first Dequeue (largest): {maxHeapByComparer.Dequeue()}");

        // classic use case: k-th largest element in a stream — keep a min-heap of size k;
        // if it grows past k, evict the smallest. The heap's Peek() is always the k-th largest so far.
        var kthLargestHeap = new PriorityQueue<int, int>();
        const int k = 2;
        foreach (var n in new[] { 4, 5, 8, 2 })
        {
            kthLargestHeap.Enqueue(n, n);
            if (kthLargestHeap.Count > k)
                kthLargestHeap.Dequeue(); // evict the current smallest
        }
        Console.WriteLine($"2nd largest of [4,5,8,2] via size-limited min-heap: {kthLargestHeap.Peek()}");
    }
}
