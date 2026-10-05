public static class LinkedListCollectionDemo
{
    public static void Run()
    {
        // LinkedList<T> — the BUILT-IN doubly linked list (System.Collections.Generic). Not the same as the
        // hand-rolled ListNode in LinkedListsDemo: that one is what LeetCode hands you for pointer-rewiring
        // problems; this one is a ready-made collection you'd reach for in a design problem (LRU Cache).
        //
        // vs List<T> (a dynamic array — see CollectionsCheatSheetDemo):
        //   - add/remove at the FRONT is O(1) here, O(n) on a List
        //   - insert/remove next to a node you already HOLD is O(1) here, O(n) on a List (shifting)
        //   - but there's NO indexer: getting to position i is an O(n) walk
        //   - finding a value is O(n) on both
        //
        // Like Stack/Queue, it has no public Add method, so a populated collection expression
        // (`LinkedList<int> l = [1, 2, 3];`) does NOT compile — only the empty `[]` does.
        // To pre-populate, pass a sequence to the constructor (adds to the end, in order). O(n).
        var list = new LinkedList<int>([2, 3, 4]);
        Console.WriteLine($"LinkedList from constructor [2, 3, 4]: {string.Join(" <-> ", list)}");

        // AddFirst / AddLast — O(1). Both RETURN the new LinkedListNode<T>, which you can keep a
        // reference to for O(1) inserts/removes later.
        list.AddFirst(1);
        LinkedListNode<int> fiveNode = list.AddLast(5);
        Console.WriteLine($"After AddFirst(1), AddLast(5): {string.Join(" <-> ", list)}");

        // First / Last are PROPERTIES returning the end NODES (null when the list is empty) — O(1).
        // Not to be confused with LINQ's First() / Last() methods, which return the value.
        // A node exposes Value, Next, Previous (null at either end), and List (the list it belongs to).
        Console.WriteLine($"list.First.Value: {list.First!.Value}, list.Last.Value: {list.Last!.Value}, list.First.Next.Value: {list.First.Next!.Value}, list.First.Previous: {list.First.Previous?.Value.ToString() ?? "null"}");

        // Find(value) — O(n), returns the FIRST node with that value, or null. FindLast searches from the end.
        // This is the "get a node" step you pay O(n) for — the O(1) operations below assume you already have one.
        LinkedListNode<int>? threeNode = list.Find(3);
        Console.WriteLine($"Find(3) found a node: {threeNode is not null}, Find(99): {list.Find(99)?.Value.ToString() ?? "null"}");

        // AddBefore / AddAfter(node, value) — O(1): just rewires a couple of pointers, nothing shifts
        list.AddAfter(threeNode!, 30);
        list.AddBefore(threeNode!, 20);
        Console.WriteLine($"After AddBefore(node3, 20), AddAfter(node3, 30): {string.Join(" <-> ", list)}");

        // removing — Remove(node) is O(1); Remove(value) is O(n) (it has to Find first) and returns bool.
        // RemoveFirst / RemoveLast are O(1) but THROW on an empty list (no Try* versions here — check Count first).
        list.Remove(fiveNode);
        bool removed20 = list.Remove(20);
        list.RemoveFirst();
        Console.WriteLine($"After Remove(node5), Remove(20) (returned {removed20}), RemoveFirst(): {string.Join(" <-> ", list)}");

        // Count — O(1), a stored field (unlike the hand-rolled ListNode, which needs a full traversal).
        // Contains — O(n), linear scan.
        Console.WriteLine($"Count: {list.Count}, Contains(30): {list.Contains(30)}");

        // no indexer — `list[1]` doesn't compile. LINQ's ElementAt(i) works but walks i nodes, O(n).
        Console.WriteLine($"list.ElementAt(1) (O(n) walk — there is no list[1]): {list.ElementAt(1)}");

        // traversal — foreach goes front to back. Going BACKWARDS is the doubly-linked advantage:
        // start at Last and follow Previous. Both O(n).
        var backwards = new List<int>();
        for (var node = list.Last; node != null; node = node.Previous)
            backwards.Add(node.Value);
        Console.WriteLine($"Forward (foreach): {string.Join(" <-> ", list)}, backward (Last -> Previous): {string.Join(" <-> ", backwards)}");

        // moving a node — Remove(node) then AddFirst(node) re-links the SAME node object, O(1), no allocation.
        // Gotcha: a node can only be in one list at a time — AddFirst(node) on a node that's still in a
        // list throws InvalidOperationException, so always Remove it first.
        var lastNode = list.Last!;
        list.Remove(lastNode);
        list.AddFirst(lastNode);
        Console.WriteLine($"Moved the last node to the front (Remove(node) + AddFirst(node)): {string.Join(" <-> ", list)}");
        try
        {
            list.AddLast(list.First!); // still in the list -> throws
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine($"AddLast(a node already in the list) threw: {e.GetType().Name}");
        }

        // as a DEQUE (double-ended queue) — C# has no Deque<T> class, so LinkedList<T> fills that role:
        // AddFirst / AddLast / RemoveFirst / RemoveLast are all O(1). Classic use: Sliding Window Maximum
        // (LC 239) — keep indices in decreasing value order; front = current window's max.
        int[] nums = [1, 3, -1, -3, 5, 3, 6, 7];
        const int k = 3;
        var deque = new LinkedList<int>(); // holds INDICES into nums
        var windowMaxes = new List<int>();
        for (var i = 0; i < nums.Length; i++)
        {
            if (deque.Count > 0 && deque.First!.Value <= i - k)
                deque.RemoveFirst(); // front index slid out of the window
            while (deque.Count > 0 && nums[deque.Last!.Value] <= nums[i])
                deque.RemoveLast(); // smaller values behind a bigger new one can never be a max again
            deque.AddLast(i);
            if (i >= k - 1)
                windowMaxes.Add(nums[deque.First!.Value]);
        }
        Console.WriteLine($"Sliding Window Maximum of [{string.Join(", ", nums)}], k={k}, using LinkedList as a deque: [{string.Join(", ", windowMaxes)}]");

        // the classic design problem: LRU Cache (LC 146) — Get and Put both O(1).
        //   - Dictionary<key, node> finds a key's node in O(1)
        //   - LinkedList keeps usage order: most recently used at the FRONT, least recently used at the BACK
        //   - "use" = move node to front (O(1)); "evict" = RemoveLast (O(1))
        // Neither structure alone does it: a Dictionary has no order, and a LinkedList's Find is O(n).
        var cache = new LruCache(capacity: 2);
        cache.Put(1, 100);
        cache.Put(2, 200);
        Console.WriteLine($"LRU Get(1) (hit, 1 becomes most recent): {cache.Get(1)}");
        cache.Put(3, 300); // over capacity -> evicts key 2, the least recently used
        Console.WriteLine($"LRU after Put(3) evicts the LRU key — Get(2): {cache.Get(2)} (miss), Get(3): {cache.Get(3)}, Get(1): {cache.Get(1)}");
    }

    // LRU Cache (LC 146). The node stores (Key, Value) — the key is needed so that when we evict the
    // last node, we know which Dictionary entry to remove too.
    private sealed class LruCache(int capacity)
    {
        private readonly Dictionary<int, LinkedListNode<(int Key, int Value)>> _map = [];
        private readonly LinkedList<(int Key, int Value)> _order = [];

        public int Get(int key)
        {
            if (!_map.TryGetValue(key, out var node))
                return -1;
            _order.Remove(node); // O(1) — we hold the node
            _order.AddFirst(node); // mark as most recently used
            return node.Value.Value;
        }

        public void Put(int key, int value)
        {
            if (_map.TryGetValue(key, out var existing))
                _order.Remove(existing);
            else if (_map.Count == capacity)
            {
                _map.Remove(_order.Last!.Value.Key); // evict least recently used
                _order.RemoveLast();
            }
            _map[key] = _order.AddFirst((key, value));
        }
    }
}
