public static class TuplesDemo
{
    public static void Run()
    {
        // ValueTuple syntax `(T1, T2)` — the modern way to bundle multiple values without
        // declaring a class/record for it. This is what you'll reach for by default in C#.
        var point = (3, 4);
        Console.WriteLine($"Tuple: {point}, point.Item1={point.Item1}, point.Item2={point.Item2}");

        // named elements — much more readable than the default Item1/Item2
        var namedPoint = (x: 3, y: 4);
        Console.WriteLine($"Named tuple: x={namedPoint.x}, y={namedPoint.y}");

        // deconstruction — unpack a tuple into separate variables
        var (px, py) = namedPoint;
        Console.WriteLine($"Deconstructed: px={px}, py={py}");

        // returning multiple values from a method — the most common reason to reach for a tuple
        var (min, max) = MinMax([5, 1, 8, 3]);
        Console.WriteLine($"MinMax([5,1,8,3]): min={min}, max={max}");

        // swap two variables — deconstruction assignment, no temp variable needed
        var a = 1;
        var b = 2;
        (a, b) = (b, a);
        Console.WriteLine($"Swapped via tuple deconstruction: a={a}, b={b}");

        // tuples have built-in VALUE equality — unlike a plain class, two tuples with the same
        // elements are == equal automatically, no Equals()/GetHashCode() override needed
        var t1 = (1, "a");
        var t2 = (1, "a");
        Console.WriteLine($"(1,\"a\") == (1,\"a\"): {t1 == t2}");

        // that built-in value equality is exactly why tuples make convenient Dictionary keys —
        // e.g. memoizing a 2D recursion / DP state keyed on (row, col)
        var memo = new Dictionary<(int Row, int Col), int>();
        memo[(0, 0)] = 42;
        Console.WriteLine($"Dictionary keyed on a tuple, memo[(0,0)]: {memo[(0, 0)]}");

        // tuples in a collection — deconstruct directly in the foreach
        List<(int Id, string Name)> people = [(1, "Alice"), (2, "Bob")];
        foreach (var (id, name) in people)
            Console.WriteLine($"Person {id}: {name}");

        // declaring a Queue of tuples (BFS on a grid, e.g. Rotting Oranges) — the type has to be
        // written once, either on the right with `var` or on the left with target-typed `new()`.
        // `var q = new();` won't compile: there's nothing to infer the type from.
        var bfsQueueVar = new Queue<(int i, int j)>();
        Queue<(int i, int j)> bfsQueueTargetTyped = new(); // also the only option for fields/properties
        bfsQueueVar.Enqueue((0, 1));
        bfsQueueTargetTyped.Enqueue((2, 3));
        Console.WriteLine($"Queue via var: [{string.Join(", ", bfsQueueVar)}], via target-typed new(): [{string.Join(", ", bfsQueueTargetTyped)}]");

        // unpacking after Dequeue — O(1). Element names survive, so either access by name or
        // deconstruct straight into locals (the usual BFS-loop shape).
        var cell = bfsQueueVar.Dequeue();
        Console.WriteLine($"Dequeue() then access by name: cell.i={cell.i}, cell.j={cell.j}");

        var (row, col) = bfsQueueTargetTyped.Dequeue();
        Console.WriteLine($"Dequeue() deconstructed into (row, col): row={row}, col={col}");

        // TryDequeue — you can't deconstruct inside `out (...)`, so take `out var` then unpack it
        bfsQueueVar.Enqueue((4, 5));
        while (bfsQueueVar.TryDequeue(out var next))
        {
            var (ni, nj) = next;
            Console.WriteLine($"TryDequeue(out var next) deconstructed: ni={ni}, nj={nj}");
        }

        // this ValueTuple `(T1, T2)` syntax (C# 7+) is a STRUCT (value type) and is what you
        // should default to. The older System.Tuple<T1, T2> (via Tuple.Create(...)) is a class
        // (reference type), has no named elements, and no built-in value equality on == — it's
        // rarely used in modern C#, mostly seen in older codebases.
    }

    private static (int Min, int Max) MinMax(int[] nums)
    {
        var min = nums[0];
        var max = nums[0];
        foreach (var n in nums)
        {
            if (n < min) min = n;
            if (n > max) max = n;
        }

        return (min, max);
    }
}
