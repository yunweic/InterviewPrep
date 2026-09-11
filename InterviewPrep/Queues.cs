public static class QueuesDemo
{
    public static void Run()
    {
        //queue — backed by a circular buffer; Enqueue/Dequeue/Peek/TryDequeue/TryPeek are all O(1) amortized
        var queue = new Queue<int>();
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
    }
}
