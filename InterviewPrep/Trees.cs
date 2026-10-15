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

        // terminology, using the tree above:
        //   root           — the top node with no parent (1)
        //   parent / child — 2 is the parent of 4 and 5; 4 and 5 are children of 2
        //   siblings       — nodes sharing a parent (4 and 5; 2 and 3)
        //   leaf           — a node with no children (4, 5, 3)
        //   internal node  — a node with at least one child (1, 2)
        //   ancestor       — any node on the path from a node up to the root (ancestors of 4: 2, 1)
        //   descendant     — any node below a node (descendants of 2: 4, 5)
        //   subtree        — a node plus all its descendants (the subtree rooted at 2 is 2, 4, 5)
        //   edge           — the link between a parent and a child (this tree has n - 1 = 4 edges)
        //   depth of a node  — number of edges from the ROOT DOWN to that node (depth(1)=0, depth(2)=1, depth(4)=2)
        //   height of a node — number of edges on the longest path from that node DOWN to a leaf (height(4)=0, height(2)=1)
        //   height of a tree — height of its root (2 here); equals the depth of the deepest node
        //   level          — all nodes at the same depth (level 0: 1 | level 1: 2, 3 | level 2: 4, 5)
        //   size           — total number of nodes (5)
        //   diameter       — number of edges on the longest path between ANY two nodes; the path doesn't
        //                    have to go through the root (4 -> 2 -> 1 -> 3 = 3 edges)
        //
        // gotcha: textbooks count height/depth in EDGES (above), but LeetCode's "Maximum Depth of
        // Binary Tree" counts NODES — that answer would be 3 here, not 2. Empty tree: -1 in edges, 0 in nodes.
        // Check which one a problem means; the code is the same, only the base case shifts by one.
        //
        // shape names:
        //   full      — every node has 0 or 2 children (true here)
        //   complete  — every level filled except possibly the last, which fills left to right (true here)
        //             — a binary heap is a complete tree stored in an array
        //   perfect   — every internal node has 2 children and all leaves share one depth (false — 3 is
        //               a leaf at depth 1, while 4/5 are at depth 2); a perfect tree of height h has 2^(h+1) - 1 nodes
        //   balanced  — at every node, the left and right subtree heights differ by at most 1 (true here)
        //               → height stays O(log n), which is what keeps BST operations O(log n)
        //   skewed    — every node has at most one child, so it's really a linked list, height O(n)
        //   BST       — for every node, everything in the LEFT subtree < node < everything in the RIGHT
        //               subtree (false here — 1's left subtree holds 2, 4, 5, all > 1). An inorder traversal of a BST is sorted.

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

        // height — post-order recursion (children first, then combine), O(n) time, O(h) stack.
        // Counting NODES (LeetCode 104 "Maximum Depth") vs EDGES (textbook height) differs only in the
        // null base case: 0 vs -1.
        Console.WriteLine($"MaxDepthInNodes(root) (LeetCode 104 style): {MaxDepthInNodes(root)}, HeightInEdges(root) (textbook): {HeightInEdges(root)}");
        Console.WriteLine($"HeightInEdges(node 2): {HeightInEdges(root.Left)}, HeightInEdges(leaf 4): {HeightInEdges(root.Left!.Left)}, HeightInEdges(null): {HeightInEdges(null)}");

        // depth of a specific node — pre-order recursion carrying the depth DOWN as a parameter
        // (height is computed bottom-up from the return values; depth is passed top-down). O(n).
        Console.WriteLine($"DepthOf(1): {DepthOf(root, 1, 0)}, DepthOf(2): {DepthOf(root, 2, 0)}, DepthOf(4): {DepthOf(root, 4, 0)}, DepthOf(99) (missing): {DepthOf(root, 99, 0)}");

        // size and leaf count — O(n). The leaf check uses a property pattern instead of two null checks.
        Console.WriteLine($"CountNodes(root) (size): {CountNodes(root)}, CountLeaves(root): {CountLeaves(root)}");

        // levels — BFS, but snapshot queue.Count at the start of each level so you know where it
        // ends. This "level size" trick is the basis of LeetCode 102 (Level Order Traversal).
        var levels = new List<List<int>>();
        var levelQueue = new Queue<TreeNode>();
        levelQueue.Enqueue(root);
        while (levelQueue.Count > 0)
        {
            var levelSize = levelQueue.Count; // everything currently queued is exactly one level
            var level = new List<int>();
            for (var i = 0; i < levelSize; i++)
            {
                var node = levelQueue.Dequeue();
                level.Add(node.Value);
                if (node.Left != null) levelQueue.Enqueue(node.Left);
                if (node.Right != null) levelQueue.Enqueue(node.Right);
            }
            levels.Add(level);
        }
        Console.WriteLine($"Levels via BFS + levelSize snapshot: {string.Join(" | ", levels.Select((lvl, d) => $"level {d}: {string.Join(", ", lvl)}"))}");

        // balanced check — O(n). The naive version calls a height function at every node (O(n log n)
        // when balanced, O(n^2) when skewed); returning -1 as an "already unbalanced" signal does it in one pass.
        var skewed = new TreeNode(1, new TreeNode(2, new TreeNode(3, null, null), null), null);
        Console.WriteLine($"IsBalanced(root): {IsBalanced(root)}, IsBalanced(skewed 1 -> 2 -> 3): {IsBalanced(skewed)}");

        // diameter (LeetCode 543) — longest path in EDGES between any two nodes. At each node the
        // longest path THROUGH it is leftHeight + rightHeight (in nodes), so track the max of that
        // while computing heights. O(n).
        Console.WriteLine($"Diameter(root) (4 -> 2 -> 1 -> 3): {Diameter(root)}, Diameter(skewed): {Diameter(skewed)}");
    }

    private static int MaxDepthInNodes(TreeNode? node) =>
        node == null ? 0 : 1 + Math.Max(MaxDepthInNodes(node.Left), MaxDepthInNodes(node.Right));

    private static int HeightInEdges(TreeNode? node) =>
        node == null ? -1 : 1 + Math.Max(HeightInEdges(node.Left), HeightInEdges(node.Right));

    // returns -1 if target isn't in the tree
    private static int DepthOf(TreeNode? node, int target, int depth)
    {
        if (node == null) return -1;
        if (node.Value == target) return depth;
        var leftResult = DepthOf(node.Left, target, depth + 1);
        return leftResult != -1 ? leftResult : DepthOf(node.Right, target, depth + 1);
    }

    private static int CountNodes(TreeNode? node) =>
        node == null ? 0 : 1 + CountNodes(node.Left) + CountNodes(node.Right);

    private static int CountLeaves(TreeNode? node) => node switch
    {
        null => 0,
        { Left: null, Right: null } => 1, // property pattern: a node with no children is a leaf
        _ => CountLeaves(node.Left) + CountLeaves(node.Right),
    };

    private static bool IsBalanced(TreeNode? root) => BalancedHeight(root) != -1;

    // height in nodes if balanced, or -1 as soon as any subtree is unbalanced
    private static int BalancedHeight(TreeNode? node)
    {
        if (node == null) return 0;
        var left = BalancedHeight(node.Left);
        if (left == -1) return -1;
        var right = BalancedHeight(node.Right);
        if (right == -1) return -1;
        return Math.Abs(left - right) > 1 ? -1 : 1 + Math.Max(left, right);
    }

    private static int Diameter(TreeNode? root)
    {
        var best = 0;
        Height(root);
        return best;

        // local function — captures `best` from the enclosing method instead of needing a field or ref param
        int Height(TreeNode? node)
        {
            if (node == null) return 0;
            var left = Height(node.Left);
            var right = Height(node.Right);
            best = Math.Max(best, left + right); // edges on the path through this node
            return 1 + Math.Max(left, right);
        }
    }

    private static void DfsRecursive(TreeNode? node, List<int> result)
    {
        if (node == null) return; // base case — this null check is the whole trick to recursive tree code
        result.Add(node.Value);
        DfsRecursive(node.Left, result);
        DfsRecursive(node.Right, result);
    }
}
