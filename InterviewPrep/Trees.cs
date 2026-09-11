public class TreeNode(int val, TreeNode? left, TreeNode? right)
{
    public int Value = val;
    public TreeNode? Left = left;
    public TreeNode? Right = right;
}

public static class TreesDemo
{
    public static void Run()
    {
        //      1
        //     / \
        //    2   3
        //   / \
        //  4   5
        var root = new TreeNode(1,
            new TreeNode(2,
                new TreeNode(4, null, null),
                new TreeNode(5, null, null)),
            new TreeNode(3, null, null));

        // recursive DFS (preorder: root, left, right) — the default way to think about tree problems.
        // Call stack depth is O(h) where h = tree height (O(n) worst case for a skewed tree).
        var preorder = new List<int>();
        DfsRecursive(root, preorder);
        Console.WriteLine($"Recursive DFS (preorder): {string.Join(", ", preorder)}");

        // iterative DFS using an explicit Stack<T> — same traversal, no recursion/call-stack risk.
        // Push right before left so left gets popped (visited) first.
        var iterativeOrder = new List<int>();
        var stack = new Stack<TreeNode>();
        stack.Push(root);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            iterativeOrder.Add(node.Value);
            if (node.Right != null) stack.Push(node.Right);
            if (node.Left != null) stack.Push(node.Left);
        }
        Console.WriteLine($"Iterative DFS via Stack (preorder): {string.Join(", ", iterativeOrder)}");

        // BFS (level order) using a Queue<T> — visits nodes level by level, not depth first.
        var levelOrder = new List<int>();
        var queue = new Queue<TreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            levelOrder.Add(node.Value);
            if (node.Left != null) queue.Enqueue(node.Left);
            if (node.Right != null) queue.Enqueue(node.Right);
        }
        Console.WriteLine($"BFS via Queue (level order): {string.Join(", ", levelOrder)}");
    }

    private static void DfsRecursive(TreeNode? node, List<int> result)
    {
        if (node == null) return; // base case — this null check is the whole trick to recursive tree code
        result.Add(node.Value);
        DfsRecursive(node.Left, result);
        DfsRecursive(node.Right, result);
    }
}
