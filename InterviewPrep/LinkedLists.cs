public class ListNode(int val, ListNode? n)
{
    public int Value = val;
    public ListNode? Next = n;
}

public static class LinkedListsDemo
{
    public static void Run()
    {
        // build a small list: 1 -> 2 -> 3 -> 4 -> null
        var head = new ListNode(1, new ListNode(2, new ListNode(3, new ListNode(4, null))));

        // traversal — the fundamental pattern: walk with a cursor until it hits null. O(n).
        var values = new List<int>();
        var cursor = head;
        while (cursor != null)
        {
            values.Add(cursor.Value);
            cursor = cursor.Next;
        }
        Console.WriteLine($"Traversal: {string.Join(" -> ", values)}");

        // Length/Count: this hand-rolled ListNode has neither — nothing here maintains a running
        // count as nodes are added/removed, so "how long is this list?" requires a full O(n)
        // traversal (like the one above). Contrast with the BCL's System.Collections.Generic.LinkedList<T>,
        // which DOES implement ICollection<T> and maintains Count as a field — O(1), same as List<T>.Count.
        var length = 0;
        for (var node = head; node != null; node = node.Next)
            length++;
        Console.WriteLine($"Length via full traversal (no Count field exists): {length}");

        // quick copy — no built-in copy for this hand-rolled ListNode; build a new chain node-by-node
        // while walking the original — O(n). This is the core of the classic "Copy List with Random
        // Pointer" problem, minus the random-pointer complication.
        var copiedHead = CopyList(head);
        copiedHead!.Value = 999; // mutate the copy
        var copiedValues = new List<int>();
        for (var node = copiedHead; node != null; node = node.Next)
            copiedValues.Add(node.Value);
        Console.WriteLine($"Copied list (independent nodes): {string.Join(" -> ", copiedValues)}, original head unaffected: {head.Value}");

        // reverse a linked list in place — O(n) time, O(1) space.
        // Classic three-pointer walk: keep the previous node, snapshot next before overwriting Next.
        ListNode? prev = null;
        ListNode? curr = head;
        while (curr != null)
        {
            var next = curr.Next; // snapshot before we overwrite it
            curr.Next = prev;     // reverse the pointer
            prev = curr;
            curr = next;
        }
        var reversedHead = prev; // prev ends up as the new head

        var reversedValues = new List<int>();
        for (var node = reversedHead; node != null; node = node.Next)
            reversedValues.Add(node.Value);
        Console.WriteLine($"Reversed: {string.Join(" -> ", reversedValues)}");

        // Floyd's cycle detection ("tortoise and hare") — O(n) time, O(1) space.
        // slow moves 1 step, fast moves 2 steps; if there's a cycle they will eventually meet.
        var noCycleResult = HasCycle(reversedHead);
        Console.WriteLine($"HasCycle on an acyclic list: {noCycleResult}");

        // build a small cyclic list: a -> b -> c -> back to a
        var a = new ListNode(1, null);
        var b = new ListNode(2, null);
        var c = new ListNode(3, null);
        a.Next = b;
        b.Next = c;
        c.Next = a; // creates the cycle
        var cycleResult = HasCycle(a);
        Console.WriteLine($"HasCycle on a cyclic list: {cycleResult}");
    }

    private static ListNode? CopyList(ListNode? src)
    {
        if (src == null) return null;
        var newHead = new ListNode(src.Value, null);
        var srcCursor = src.Next;
        var newCursor = newHead;
        while (srcCursor != null)
        {
            newCursor.Next = new ListNode(srcCursor.Value, null);
            newCursor = newCursor.Next;
            srcCursor = srcCursor.Next;
        }
        return newHead;
    }

    private static bool HasCycle(ListNode? head)
    {
        var slow = head;
        var fast = head;
        while (fast?.Next != null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
            if (slow == fast) return true; // reference equality — same node, not same value
        }
        return false;
    }
}
