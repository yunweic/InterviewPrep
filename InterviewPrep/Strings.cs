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

        // IsLetterOrDigit — the classic building block for "Valid Palindrome"-style problems:
        // filter a string down to only alphanumeric characters before comparing
        Console.WriteLine($"char.IsLetterOrDigit('a'): {char.IsLetterOrDigit('a')}, char.IsLetterOrDigit('!'): {char.IsLetterOrDigit('!')}");

        // string <-> int conversion
        var parsedInt = int.Parse("42"); // throws FormatException if the string isn't a valid int
        Console.WriteLine($"int.Parse(\"42\"): {parsedInt}");

        bool parsedOk = int.TryParse("abc", out int triedInt); // safe version — no exception, returns bool
        Console.WriteLine($"int.TryParse(\"abc\", out ...): success={parsedOk}, value={triedInt}");

        Console.WriteLine($"42.ToString(): {42.ToString()}");

        // converting a single DIGIT CHARACTER to its numeric value — char is a numeric type under
        // the hood, so subtracting '0' is much cheaper than int.Parse(c.ToString()). A staple in
        // digit-DP / string-to-number / sum-of-digits style problems.
        var digitChar = '7';
        var digitValue = digitChar - '0';
        Console.WriteLine($"'{digitChar}' - '0': {digitValue}");

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

        // sort a string alphabetically — same pattern: no built-in string.Sort(), go through
        // char[] + Array.Sort — O(n log n). Sort is ORDINAL by default, so uppercase letters sort
        // before lowercase ('C' < 'L' < 'd' in ASCII) — a common gotcha if input isn't all one case.
        char[] sortedChars = word.ToCharArray();
        Array.Sort(sortedChars);
        var sortedWord = new string(sortedChars);
        Console.WriteLine($"Sorted alphabetically (Array.Sort): {sortedWord}");

        // LINQ one-liner alternative — same ordinal ordering, just more concise. Handy for the
        // classic "are these two strings anagrams?" check: sort both, compare for equality.
        var sortedWord2 = new string(word.OrderBy(c => c).ToArray());
        Console.WriteLine($"Sorted alphabetically (OrderBy): {sortedWord2}");

        // StringBuilder — use instead of += in a loop to avoid O(n^2) string reallocation.
        // Append is O(1) amortized per call, so building a string of length n this way is O(n) total —
        // compare to `result += c` in a loop, which reallocates and copies the whole string every
        // iteration (O(n) per append -> O(n^2) overall).
        var sb = new StringBuilder();
        foreach (var c in word)
            sb.Append(c).Append('-');
        Console.WriteLine($"StringBuilder Append in a loop: {sb.ToString().TrimEnd('-')}");

        // StringBuilder is a REAL mutable buffer, unlike string — Length can be read AND set
        // (shortening it truncates in place, no reallocation), and the indexer reads/writes
        // individual characters directly.
        Console.WriteLine($"StringBuilder.Length: {sb.Length}");
        sb.Length--; // trims the trailing '-' by shrinking the buffer in place
        Console.WriteLine($"After Length-- (trims trailing '-'): {sb}");
        sb[0] = 'l';
        Console.WriteLine($"After sb[0] = 'l': {sb}");

        // Insert / Remove / Replace all mutate in place and return the StringBuilder, so they chain
        var sb2 = new StringBuilder("LeetCode");
        sb2.Insert(4, "-"); // Insert(index, value)
        Console.WriteLine($"Insert(4, \"-\"): {sb2}");
        sb2.Remove(4, 1); // Remove(startIndex, length)
        Console.WriteLine($"Remove(4, 1): {sb2}");
        sb2.Replace("Code", "Sharp"); // find-and-replace, like string.Replace but in place
        Console.WriteLine($"Replace(\"Code\", \"Sharp\"): {sb2}");

        // gotcha: there's no built-in Reverse() on StringBuilder (unlike List<T>.Reverse()) —
        // go through ToCharArray() + Array.Reverse, same as reversing a plain string
        var sb3 = new StringBuilder("LeetCode");
        var sb3Chars = sb3.ToString().ToCharArray();
        Array.Reverse(sb3Chars);
        Console.WriteLine($"StringBuilder reversed (no built-in Reverse()): {new string(sb3Chars)}");

        // ToString(startIndex, length) pulls out a substring without building an intermediate
        // full string first
        Console.WriteLine($"sb3.ToString(0, 4): {sb3.ToString(0, 4)}");

        // if you know the final size upfront, pre-sizing the capacity avoids internal
        // reallocations as it grows — new StringBuilder(capacity), not a length
        var sbPresized = new StringBuilder(capacity: 64);
        Console.WriteLine($"Pre-sized StringBuilder capacity: {sbPresized.Capacity}");

        // string comparison — == does value comparison in C# (unlike Java's reference-comparison gotcha) — O(n)
        Console.WriteLine($"\"abc\" == \"abc\": {"abc" == "abc"}");
        Console.WriteLine($"string.Equals(\"abc\", \"ABC\", StringComparison.OrdinalIgnoreCase): {string.Equals("abc", "ABC", StringComparison.OrdinalIgnoreCase)}");

        // Length (not Count) — string.Length is an O(1) field read, same idea as an array's Length.
        // But LINQ's Count() extension method is evaluated DYNAMICALLY (see ListsDemo for the general
        // rule): it only gets the O(1) fast path when the source implements ICollection<T>, and string
        // does NOT implement ICollection<char> (only IEnumerable<char>) — so word.Count() has no fast
        // path and walks every character, O(n), even though word.Length answers the same question in O(1).
        Console.WriteLine($"word.Length: {word.Length}");
        Console.WriteLine($"word.Count() (LINQ extension, string has no ICollection<char> -> must enumerate, O(n)): {word.Count()}");

        // quick copy — strings are immutable, so plain assignment is always a safe, O(1) "copy": both
        // variables reference the SAME object, but since neither can be mutated, there's no way for a
        // change through one to affect the other. Forcing a genuinely different object with the same
        // content (rarely needed — mainly identity/interning edge cases) costs O(n) instead.
        var wordAssigned = word;
        var wordForcedNewObject = new string(word.ToCharArray());
        Console.WriteLine($"ReferenceEquals(word, wordAssigned): {ReferenceEquals(word, wordAssigned)} (same object, O(1) copy)");
        Console.WriteLine($"ReferenceEquals(word, wordForcedNewObject): {ReferenceEquals(word, wordForcedNewObject)} (different object, same value, O(n) copy)");
    }
}
