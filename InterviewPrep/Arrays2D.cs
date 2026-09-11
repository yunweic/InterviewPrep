public static class Arrays2DDemo
{
    public static void Run()
    {
        // rectangular 2D array — every row has the same fixed length, grid[row, col]
        int[,] rectGrid = new int[2, 3];
        rectGrid[0, 0] = 1;
        rectGrid[1, 2] = 9;
        Console.WriteLine($"Rectangular grid[1,2]: {rectGrid[1, 2]}, GetLength(0)={rectGrid.GetLength(0)}, GetLength(1)={rectGrid.GetLength(1)}");

        // jagged array — an array of arrays, rows can have different lengths. This is what
        // most LeetCode problems actually hand you (int[][] grid), and what you'll build yourself
        // most often since each row is independently allocated.
        int[][] jaggedGrid = new int[3][];
        for (var i = 0; i < jaggedGrid.Length; i++)
            jaggedGrid[i] = new int[i + 1]; // row 0 has 1 column, row 1 has 2, row 2 has 3
        jaggedGrid[2][2] = 5;
        Console.WriteLine($"Jagged grid[2][2]: {jaggedGrid[2][2]}, row lengths: {string.Join(", ", jaggedGrid.Select(row => row.Length))}");

        // jagged array literal, the form you'll write constantly for test input
        int[][] matrix =
        [
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        ];

        // nested-loop traversal — row by row, column by column
        foreach (var row in matrix)
            Console.WriteLine($"Row: {string.Join(", ", row)}");

        // Array.Fill — initialize every cell to a sentinel value, common for DP grids ("unvisited" = -1)
        var dp = new int[3][];
        for (var i = 0; i < dp.Length; i++)
        {
            dp[i] = new int[3];
            Array.Fill(dp[i], -1);
        }
        Console.WriteLine($"DP grid row 0 after Array.Fill(-1): {string.Join(", ", dp[0])}");

        // 4-directional neighbor traversal — the standard pattern for grid BFS/DFS (flood fill,
        // number of islands, shortest path on a grid, etc.). Encode the 4 moves as delta pairs.
        int[][] directions = [[-1, 0], [1, 0], [0, -1], [0, 1]]; // up, down, left, right
        var rows = matrix.Length;
        var cols = matrix[0].Length;
        var row0 = 1;
        var col0 = 1;
        var neighbors = new List<int>();
        foreach (var dir in directions)
        {
            var newRow = row0 + dir[0];
            var newCol = col0 + dir[1];
            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols) // bounds check — easy to forget
                neighbors.Add(matrix[newRow][newCol]);
        }
        Console.WriteLine($"4-directional neighbors of matrix[1][1]: {string.Join(", ", neighbors)}");
    }
}
