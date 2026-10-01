using System.Text;

public static class CollectionsCheatSheetDemo
{
    public static void Run()
    {
        // Side-by-side recap of every collection's declare / add / remove / peek / check / size, plus
        // common array / string / StringBuilder operations and the Try* methods, all with Big-O,
        // so the verbs don't blur together. Each collection's own topic file has the details.
        //
        // Memory hook — the collections split into two families:
        //
        //   1) "Bag" collections — you pick WHICH item to touch (by value, key, or index):
        //        List<T>, HashSet<T>, Dictionary<K,V>
        //        verbs:  Add / Remove / Contains (ContainsKey for Dictionary)
        //        init:   [1, 2, 3] collection expression (Dictionary: `{ [k] = v }` instead)
        //
        //   2) "Line" collections — the COLLECTION picks which item comes out next:
        //        Stack<T>        Push / Pop          LIFO   — "push down, pop up" (stack of plates)
        //        Queue<T>        Enqueue / Dequeue   FIFO   — "join the queue, leave the queue"
        //        PriorityQueue   Enqueue / Dequeue   lowest priority first (a queue with VIP cuts)
        //        verbs:  all three share Peek + the Try* versions (TryPop / TryDequeue / TryPeek)
        //        init:   NO populated [1, 2, 3] — they have no Add method, so pass a sequence
        //                to the constructor: new Stack<int>([1, 2, 3])
        //
        //   Size: Count everywhere — Length is only for arrays, strings, and StringBuilder.
        //
        // Big-O cheat table (average case). "amortized" = usually O(1), but an occasional internal
        // resize copies everything once (O(n)), which averages out to O(1) per call.
        //
        //   Collection        Add                        Remove                     Lookup / check                   Peek     Size
        //   ---------------   ------------------------   ------------------------   ------------------------------   ------   ----------
        //   T[] array         (fixed size, no Add)       (fixed size, no Remove)    arr[i] O(1), Contains O(n)       —        Length
        //   string            (immutable)                (immutable)                s[i] O(1), Contains O(n*m)       —        Length
        //   StringBuilder     Append O(1) amortized      Remove(i, len) O(n)        sb[i] O(1) (get AND set)         —        Length (settable!)
        //                     Insert(i, s) O(n)          Length-- O(1) (drop last)  ToString() O(n)
        //   List<T>           Add O(1) amortized         Remove(value) O(n)         list[i] O(1)                     —        Count
        //                     Insert(i, x) O(n)          RemoveAt(i) O(n - i)       Contains O(n)  <- the slow one
        //   HashSet<T>        Add O(1) amortized         Remove O(1)                Contains O(1)                    —        Count
        //   Dictionary<K,V>   Add / d[k] = v O(1) am.    Remove O(1)                ContainsKey / TryGetValue O(1)   —        Count
        //                                                                           ContainsValue O(n)
        //   Stack<T>          Push O(1) amortized        Pop O(1)                   Contains O(n)                    O(1)     Count
        //   Queue<T>          Enqueue O(1) amortized     Dequeue O(1)               Contains O(n)                    O(1)     Count
        //   PriorityQueue     Enqueue O(log n)           Dequeue O(log n)           (no Contains)                    O(1)     Count
        //
        //   Length and Count are both O(1) PROPERTIES (no parentheses) — a stored number, nothing is counted.
        //   Rule of thumb: fixed-size things (array, string) have Length; growable collections have Count.
        //   The exception is StringBuilder: it grows, but it's "string-like", so it uses Length — and you
        //   can even ASSIGN it (sb.Length-- trims the last char, sb.Length = 0 clears it).
        //   Don't confuse them with LINQ's Count() METHOD (with parentheses): it works on anything, but on
        //   a string or other plain IEnumerable it walks every item, O(n). See ListsDemo for details.
        //   HashSet / Dictionary are O(n) WORST case if many keys collide into the same hash bucket —
        //   rare with built-in types, so interviews treat them as O(1).

        // Length — arrays and strings only. Fixed size, so it's a property, O(1)
        int[] arr = [1, 2, 3];
        var str = "hello";
        Console.WriteLine($"int[] [1,2,3].Length: {arr.Length}, \"hello\".Length: {str.Length}  (vs. every collection below, which uses Count)");

        // Common array operations (n = array length, k = slice length)
        //
        //   Operation                          Big-O        Notes
        //   --------------------------------   ----------   -------------------------------------------------
        //   new int[n]                         O(n)         every slot starts at default (0 / null / false)
        //   arr[i], arr[^1]                    O(1)         ^1 = last element
        //   arr[a..b]                          O(k)         slice is a NEW array copy (b exclusive)
        //   Array.Fill(arr, x)                 O(n)         e.g. fill a dp array with -1 / int.MaxValue
        //   Array.Sort(arr)                    O(n log n)   in place; Array.Sort(arr, (a, b) => ...) for custom order
        //   Array.Reverse(arr)                 O(n)         in place
        //   Array.IndexOf(arr, x)              O(n)         -1 if missing
        //   Array.BinarySearch(arr, x)         O(log n)     array must be SORTED; negative if missing
        //   arr.Contains(x)                    O(n)         LINQ
        //   arr.Max() / Min() / Sum()          O(n)         LINQ
        //   (int[])arr.Clone(), [.. arr]       O(n)         shallow copy
        int[] nums = [5, 2, 8, 1];
        Array.Sort(nums);
        Console.WriteLine($"int[] [5,2,8,1] -> Array.Sort: [{string.Join(", ", nums)}], nums[^1]: {nums[^1]}, nums[1..3]: [{string.Join(", ", nums[1..3])}], Array.BinarySearch(nums, 5): {Array.BinarySearch(nums, 5)}");
        Array.Reverse(nums);
        var dp = new int[4];
        Array.Fill(dp, -1);
        Console.WriteLine($"Array.Reverse: [{string.Join(", ", nums)}], Array.IndexOf(nums, 8): {Array.IndexOf(nums, 8)}, new int[4] + Array.Fill(dp, -1): [{string.Join(", ", dp)}]");

        // Common string operations (n = string length, k = result/part length).
        // Strings are IMMUTABLE — every operation that "changes" a string returns a NEW one, O(n) to build it.
        //
        //   Operation                          Big-O        Notes
        //   --------------------------------   ----------   -------------------------------------------------
        //   s[i]                               O(1)         read only — s[0] = 'x' doesn't compile
        //   s[a..b], s.Substring(a, len)       O(k)         NEW string; note Substring takes a LENGTH, not an end index
        //   s.IndexOf(x), s.Contains(x)        O(n*m)       m = length of x; ~O(n) in practice
        //   s.StartsWith(x), s.EndsWith(x)     O(m)
        //   s.Split(','), string.Join(",", p)  O(n)
        //   s.ToCharArray(), new string(arr)   O(n)         the round trip for sorting / reversing / editing
        //   s.Replace("a", "b")                O(n)
        //   s.ToLower(), s.ToUpper(), s.Trim() O(n)
        //   new string('-', k)                 O(k)         repeat a char k times
        //   s1 == s2                           O(n)         compares by VALUE in C# (not reference, unlike Java)
        var s = "hello world";
        Console.WriteLine($"\"{s}\" -> s[0]: {s[0]}, s[6..]: {s[6..]}, Substring(0, 5): {s.Substring(0, 5)}, IndexOf('o'): {s.IndexOf('o')}, Contains(\"world\"): {s.Contains("world")}");
        char[] sChars = s.ToCharArray();
        Array.Reverse(sChars);
        Console.WriteLine($"Split(' '): [{string.Join("|", s.Split(' '))}], Replace(\"l\", \"L\"): {s.Replace("l", "L")}, ToCharArray + Array.Reverse + new string: {new string(sChars)}, new string('-', 5): {new string('-', 5)}");

        // StringBuilder — a mutable string (a growable char buffer). Use it instead of `s += c` in a loop,
        // which copies the whole string every time (O(n^2) total) — n Appends are O(n) total.
        // Append O(1) amortized, Insert / Remove O(n) (shift chars, same as List<T>), sb[i] O(1), ToString() O(n).
        var sb = new StringBuilder("ab");
        sb.Append('c').Append("de"); // Append returns the builder, so calls chain
        sb.Insert(0, "X");
        sb.Remove(1, 1); // Remove(startIndex, length)
        sb[0] = 'x'; // unlike string, the indexer can WRITE
        sb.Length--; // drop the last char in place — a staple for backtracking "undo the last choice"
        Console.WriteLine($"StringBuilder \"ab\" -> Append('c').Append(\"de\") -> Insert(0, \"X\") -> Remove(1, 1) -> sb[0] = 'x' -> Length--: \"{sb}\", Length: {sb.Length}, ToString(): \"{sb.ToString()}\"");

        // List<T> — dynamic array. Add O(1) amortized, Remove(value) O(n), Contains O(n), list[i] O(1)
        List<int> list = [1, 2, 3];
        list.Add(4);
        list.Remove(1);
        Console.WriteLine($"List<int> [1,2,3] -> Add(4) -> Remove(1): [{string.Join(", ", list)}], list[0]: {list[0]}, Contains(2): {list.Contains(2)}, Count: {list.Count}");

        // HashSet<T> — unique values, no index. Add / Remove / Contains all O(1) average.
        // Add returns false on a duplicate instead of throwing.
        HashSet<int> set = [1, 2, 3];
        bool addedDup = set.Add(3);
        set.Remove(1);
        Console.WriteLine($"HashSet<int> [1,2,3] -> Add(3) returned {addedDup} -> Remove(1): [{string.Join(", ", set)}], Contains(2): {set.Contains(2)}, Count: {set.Count}");

        // Dictionary<K,V> — key -> value. Everything O(1) average. dict[k] = v adds-or-overwrites;
        // dict[k] on a missing key THROWS, so read with TryGetValue.
        var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
        dict["c"] = 3;
        dict.Remove("a");
        bool hasB = dict.TryGetValue("b", out int bValue);
        Console.WriteLine($"Dictionary<string,int> {{a:1,b:2}} -> [\"c\"] = 3 -> Remove(\"a\"): {string.Join(", ", dict)}, TryGetValue(\"b\"): {hasB} ({bValue}), ContainsKey(\"a\"): {dict.ContainsKey("a")}, Count: {dict.Count}");

        // Stack<T> — LIFO. Push / Pop / Peek all O(1). Constructor pushes in order, so LAST item is on top.
        var stack = new Stack<int>([1, 2, 3]);
        stack.Push(4);
        int popped = stack.Pop();
        Console.WriteLine($"Stack<int> ctor [1,2,3] -> Push(4) -> Pop(): {popped}, Peek() (top): {stack.Peek()}, Count: {stack.Count}");

        // Queue<T> — FIFO. Enqueue / Dequeue / Peek all O(1). Constructor enqueues in order, so FIRST item is at the front.
        var queue = new Queue<int>([1, 2, 3]);
        queue.Enqueue(4);
        int dequeued = queue.Dequeue();
        Console.WriteLine($"Queue<int> ctor [1,2,3] -> Enqueue(4) -> Dequeue(): {dequeued}, Peek() (front): {queue.Peek()}, Count: {queue.Count}");

        // PriorityQueue<TElement, TPriority> — min-heap. Enqueue / Dequeue O(log n), Peek O(1).
        // Same verbs as Queue, but Enqueue takes TWO arguments (element, priority), and
        // Dequeue returns the lowest priority, not the oldest item.
        var pq = new PriorityQueue<string, int>([("low", 5), ("urgent", 1)]);
        pq.Enqueue("medium", 3);
        string first = pq.Dequeue();
        Console.WriteLine($"PriorityQueue<string,int> (low:5, urgent:1) -> Enqueue(\"medium\", 3) -> Dequeue(): {first}, Peek(): {pq.Peek()}, Count: {pq.Count}");

        // the Try* versions — same shape across all three "line" collections: return bool, value via out.
        // Use these in a loop instead of checking Count first. (the plain versions throw on empty)
        var emptyStack = new Stack<int>();
        var emptyQueue = new Queue<int>();
        var emptyPq = new PriorityQueue<int, int>();
        Console.WriteLine($"On empty — Stack.TryPop: {emptyStack.TryPop(out _)}, Queue.TryDequeue: {emptyQueue.TryDequeue(out _)}, PriorityQueue.TryDequeue: {emptyPq.TryDequeue(out _, out _)}");

        // Try* methods for every collection. They all share one shape: return bool (did it work?),
        // hand back the value through an `out` parameter, and NEVER throw. On failure, the out value is
        // default (0 / null). Same Big-O as the non-Try version.
        //
        //   Collection        Try methods                                    Throwing version it replaces
        //   ---------------   --------------------------------------------   ----------------------------------------
        //   Dictionary<K,V>   TryGetValue(k, out v)                          d[k] — throws KeyNotFoundException
        //                     TryAdd(k, v)                                   Add(k, v) — throws on duplicate key
        //                     Remove(k, out v)  (bool + removed value)       d[k] then Remove(k) — two lookups
        //   HashSet<T>        TryGetValue(x, out stored)                     —  (Add / Remove already return bool)
        //   Stack<T>          TryPop(out x), TryPeek(out x)                  Pop() / Peek() — throw when empty
        //   Queue<T>          TryDequeue(out x), TryPeek(out x)              Dequeue() / Peek() — throw when empty
        //   PriorityQueue     TryDequeue(out e, out p), TryPeek(out e, out p)   same, but TWO outs: element + priority
        //   int / long / ...  int.TryParse(s, out n)                         int.Parse(s) — throws FormatException
        //   List<T>, T[]      none — bounds-check the index yourself, or use FindIndex / IndexOf (-1 if missing)
        var counts = new Dictionary<char, int> { ['a'] = 2 };
        bool tryAddNew = counts.TryAdd('b', 1);
        bool tryAddDup = counts.TryAdd('a', 99); // key exists -> false, value stays 2
        bool gotZ = counts.TryGetValue('z', out int zCount);
        bool removedA = counts.Remove('a', out int removedACount);
        Console.WriteLine($"Dictionary — TryAdd('b', 1): {tryAddNew}, TryAdd('a', 99): {tryAddDup}, TryGetValue('z'): {gotZ} (value {zCount}), Remove('a', out v): {removedA} (v = {removedACount})");

        var peekStack = new Stack<int>([1, 2]);
        var peekQueue = new Queue<int>([1, 2]);
        var peekPq = new PriorityQueue<string, int>([("task", 7)]);
        peekStack.TryPeek(out int stackTop);
        peekQueue.TryPeek(out int queueFront);
        peekPq.TryPeek(out string? pqElement, out int pqPriority);
        Console.WriteLine($"TryPeek — Stack: {stackTop}, Queue: {queueFront}, PriorityQueue: {pqElement} (priority {pqPriority})");

        bool parsed = int.TryParse("12x", out int parsedValue);
        Console.WriteLine($"int.TryParse(\"12x\"): {parsed} (value {parsedValue})");

        // the classic loop pattern — drain a queue / stack without a separate Count check (e.g. BFS)
        var bfsQueue = new Queue<int>([1, 2, 3]);
        var drained = new List<int>();
        while (bfsQueue.TryDequeue(out int next))
            drained.Add(next);
        Console.WriteLine($"while (queue.TryDequeue(out int next)) drained: [{string.Join(", ", drained)}]");
    }
}
