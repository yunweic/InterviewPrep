public static class NullableDemo
{
    public static void Run()
    {
        //use nullable
        var listNode = new ListNode(1, null);
        ListNode? listNode2 = null;

        var l1Val = listNode?.Value ?? 0;
        var l2Val = listNode2?.Value ?? 0;
        Console.WriteLine($"Null-conditional/coalescing results -> listNode.Value: {l1Val}, listNode2 (null).Value defaulted: {l2Val}");

        if (listNode2 is null)
            Console.WriteLine($"Notice you can print a variable with null like {listNode2}");

        if (listNode2 == null)
            Console.WriteLine($"Notice you can print a variable with null like {listNode2}");

        // only works with record (class)
        // var listNode3 = listNode with { Value = 22};

        /* if (null) { }      // ❌ compile error — cannot convert null to bool
        if (0) { }          // ❌ compile error — cannot convert int to bool
        if ("") { }         // ❌ compile error — cannot convert string to bool
        if (someList) { }   // ❌ compile error — cannot convert List<T> to bool


        if (obj == null) { }         // ✅ explicit null check
        if (num == 0) { }             // ✅ explicit comparison
        if (str == "") { }            // ✅ or string.IsNullOrEmpty(str)
        if (list.Count == 0) { }      // ✅ or list.Count > 0 for the positive case
        */
    }
}
