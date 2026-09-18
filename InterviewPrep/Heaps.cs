public static class HeapsDemo
{
    public static void Run()
    {
        // PriorityQueue<TElement, TPriority> — a MIN-heap by default: Dequeue always returns
        // the element with the LOWEST priority first. It's backed by a binary heap, so:
        //   - Enqueue: O(log n) — adds to the end, then "sifts up" to restore the heap property.
        //   - Dequeue: O(log n) — removes the root, moves the last element there, "sifts down".
        //   - Peek:    O(1)     — just reads the root, no restructuring needed.
        //   - Count:   O(1)
        // there's no built-in TPriority-only overload for value types like int — you enqueue
        // (element, priority) pairs, and often element == priority for simple cases.
        //
        // PriorityQueue does NOT support collection expressions at all — not even `[]` for empty
        // (the compiler reports CS9174 "type is not constructible"). To pre-populate cleanly
        // instead of a loop of Enqueue calls, pass a sequence of (element, priority) tuples to
        // the constructor — and it's not just cleaner syntax: building from a known batch this
        // way runs the classic O(n) "heapify" algorithm, vs. O(n log n) for n individual Enqueue
        // calls. Worth reaching for whenever you already have all the initial elements upfront.
        var minHeapFromCtor = new PriorityQueue<int, int>([(5, 5), (1, 1), (3, 3), (2, 2)]);
        Console.WriteLine($"PriorityQueue from tuple-sequence ctor -> Peek() (lowest): {minHeapFromCtor.Peek()}");

        var minHeap = new PriorityQueue<int, int>();
        minHeap.Enqueue(5, 5); // O(log n) each
        minHeap.Enqueue(1, 1);
        minHeap.Enqueue(3, 3);
        minHeap.Enqueue(2, 2);

        Console.WriteLine($"Peek() (lowest priority, not removed): {minHeap.Peek()}"); // O(1)

        // draining a heap of n items via a Dequeue loop is O(n log n) total — n dequeues, each O(log n)
        var order = new List<int>();
        while (minHeap.Count > 0) // Count is O(1)
            order.Add(minHeap.Dequeue()); // O(log n) each
        Console.WriteLine($"Dequeue order (ascending): {string.Join(", ", order)}");

        // TryDequeue is the non-throwing version, same idea as List/Stack/Queue's TryX methods — O(log n)
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
        // Processing n elements this way is O(n log k) total — each Enqueue/Dequeue is O(log k)
        // since the heap never holds more than k+1 items, not O(log n) like an unbounded heap.
        // That's the actual payoff of capping the heap size, not just memory savings.
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
