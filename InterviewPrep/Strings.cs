using System.Text;

public static class StringsDemo
{
    public static void Run()
    {
        var word = "LeetCode";

        // strings are immutable — go through char[] to mutate character-by-character
        char[] chars = word.ToCharArray();
        chars[0] = 'l';
        var lowerFirst = new string(chars);
        Console.WriteLine($"Mutate via ToCharArray -> new string(chars): {lowerFirst}");

        // Substring(start) and Substring(start, length) — O(k), k = length of the substring produced
        // (allocates and copies a new string; NOT O(1), unlike some languages' string-view slices)
        Console.WriteLine($"Substring(4): {word.Substring(4)}");
        Console.WriteLine($"Substring(0, 4): {word.Substring(0, 4)}");

        // Split — O(n); Join — O(n) (total length of all parts)
        var parts = "a,b,,c".Split(',');
        Console.WriteLine($"Split(','): [{string.Join("|", parts)}] (length {parts.Length}, note the empty string between b and c)");
        var noEmpty = "a,b,,c".Split(',', StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine($"Split(',', RemoveEmptyEntries): [{string.Join("|", noEmpty)}]");

        // IndexOf / Contains(string) — O(n*m) worst case (n = haystack length, m = needle length),
        // though .NET's implementation is optimized and close to O(n) in most real cases.
        // StartsWith — O(k), k = length of the prefix being compared.
        Console.WriteLine($"IndexOf('C'): {word.IndexOf('C')} (-1 means not found)");
        Console.WriteLine($"Contains(\"Code\"): {word.Contains("Code")}");
        Console.WriteLine($"StartsWith(\"Leet\"): {word.StartsWith("Leet")}");

        // case conversion and trimming — all O(n), each allocates a new string
        Console.WriteLine($"ToUpper(): {word.ToUpper()}");
        Console.WriteLine($"ToLower(): {word.ToLower()}");
        Console.WriteLine($"Trim() on \"  spaced  \": '{"  spaced  ".Trim()}'");

        // char classification — common in parsing/validation problems
        Console.WriteLine($"char.IsDigit('7'): {char.IsDigit('7')}, char.IsLetter('7'): {char.IsLetter('7')}");
        Console.WriteLine($"char.IsUpper('L'): {char.IsUpper('L')}, char.IsWhiteSpace(' '): {char.IsWhiteSpace(' ')}");

        // traversal — indexer gives read-only char access (can't do word[0] = 'x', string is immutable) — O(1)
        Console.WriteLine($"Indexer word[0]: {word[0]}");

        // simplest traversal: foreach when you don't need the index
        foreach (var c in word)
            Console.Write(c + " ");
        Console.WriteLine();

        // indexed for-loop: use when you need the index itself, or need to look ahead/behind
        for (var i = 0; i < word.Length; i++)
            Console.Write($"{i}:{word[i]} ");
        Console.WriteLine();

        // reverse traversal
        for (var i = word.Length - 1; i >= 0; i--)
            Console.Write(word[i]);
        Console.WriteLine();

        // two-pointer traversal — classic pattern for palindrome checks, reversing in place, etc.
        var left = 0;
        var right = word.Length - 1;
        var isPalindromeCheck = true;
        while (left < right)
        {
            if (word[left] != word[right]) { isPalindromeCheck = false; break; }
            left++;
            right--;
        }
        Console.WriteLine($"Two-pointer palindrome check on \"{word}\": {isPalindromeCheck}");

        // slicing — strings support Range indexers directly, just like arrays; each returns a NEW string
        // O(k), k = length of the slice (same cost as Substring, since it IS Substring under the hood)
        Console.WriteLine($"word[1..^1] (drop first and last char): {word[1..^1]}");
        Console.WriteLine($"word[..4] (first 4 chars): {word[..4]}");
        Console.WriteLine($"word[4..] (from index 4 to end): {word[4..]}");
        Console.WriteLine($"word[2..5] (indices 2 through 4): {word[2..5]}");
        // equivalent without ranges: word.Substring(2, 5 - 2) — ranges are the more readable option in modern C#

        // reverse a string — no built-in string.Reverse(), go through char[] + Array.Reverse — O(n)
        char[] reversedChars = word.ToCharArray();
        Array.Reverse(reversedChars);
        var reversed = new string(reversedChars);
        Console.WriteLine($"Reversed: {reversed}");

        // StringBuilder — use instead of += in a loop to avoid O(n^2) string reallocation.
        // Append is O(1) amortized per call, so building a string of length n this way is O(n) total —
        // compare to `result += c` in a loop, which reallocates and copies the whole string every
        // iteration (O(n) per append -> O(n^2) overall).
        var sb = new StringBuilder();
        foreach (var c in word)
            sb.Append(c).Append('-');
        Console.WriteLine($"StringBuilder Append in a loop: {sb.ToString().TrimEnd('-')}");

        // string comparison — == does value comparison in C# (unlike Java's reference-comparison gotcha) — O(n)
        Console.WriteLine($"\"abc\" == \"abc\": {"abc" == "abc"}");
        Console.WriteLine($"string.Equals(\"abc\", \"ABC\", StringComparison.OrdinalIgnoreCase): {string.Equals("abc", "ABC", StringComparison.OrdinalIgnoreCase)}");
    }
}
