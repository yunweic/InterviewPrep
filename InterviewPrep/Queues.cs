public static class QueuesDemo
{
    public static void Run()
    {
        //queue — backed by a circular buffer; Enqueue/Dequeue/Peek/TryDequeue/TryPeek are all O(1) amortized.
        // Like Stack<T>, Queue<T> has no Add method (only Enqueue), so it does NOT support a
        // populated collection expression — only `Queue<int> q = [];` (empty) works.
        //
        // To pre-populate, the constructor-from-sequence is cleanest: it enqueues in enumeration
        // order, so the FIRST element stays at the front.
        var queue = new Queue<int>([1, 2, 3]);
        Console.WriteLine($"Queue from constructor [1, 2, 3] -> Peek() (front): {queue.Peek()}");

        queue = new Queue<int>();
        queue.Enqueue(1);
        queue.Enqueue(2);
        queue.Enqueue(3);
        foreach (var item in queue)
            Console.WriteLine($"Queue item (foreach, front-to-back order): {item}");

        Console.WriteLine($"Queue.Peek() (front item, not removed): {queue.Peek()}");
        var dequeuedItem = queue.Dequeue();
        Console.WriteLine($"Queue.Dequeue() (removed front item): {dequeuedItem}");

        queue.Clear();
        bool peekSuccess = queue.TryPeek(out int queueValue);
        Console.WriteLine($"TryPeek result = {peekSuccess},  value = {queueValue}");
        // queueValue == default(int) == 0   (for reference types, it'd be null)

        bool dequeued = queue.TryDequeue(out int dequeuedValue);
        Console.WriteLine($"TryDequeue() result = {dequeued},  value = {dequeuedValue}");
        // dequeuedValue == default(int) == 0

        // `Queue<int> q = [];` — the empty collection expression, equivalent to `new Queue<int>()`
        Queue<int> emptyQueue = [];
        emptyQueue.Enqueue(1);
        Console.WriteLine($"Queue.Peek() after Enqueue(1) on an empty-collection-expression queue: {emptyQueue.Peek()}");

        // Count (not Length) — Queue<T> implements ICollection<T>, so Count is an O(1) field read
        // (see ListsDemo for the full Count-property vs LINQ-Count()-extension explanation).
        Console.WriteLine($"emptyQueue.Count: {emptyQueue.Count}");

        // quick copy — O(n): constructor-from-sequence. Queue<T>'s enumerator yields items
        // front-to-back (the same order Dequeue would remove them), and the constructor enqueues in
        // enumeration order, so the copy's front-to-back order matches the original exactly.
        var copySource = new Queue<int>([1, 2, 3]);
        var queueCopy = new Queue<int>(copySource);
        Console.WriteLine($"Queue copy preserves dequeue order — original: {string.Join(", ", copySource)}, copy: {string.Join(", ", queueCopy)}");
    }
}
