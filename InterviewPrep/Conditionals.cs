public static class ConditionalsDemo
{
    public static void Run()
    {
        // if / else if / else — the basics, no surprises
        var score = 85;
        string grade;
        if (score >= 90) grade = "A";
        else if (score >= 80) grade = "B";
        else if (score >= 70) grade = "C";
        else grade = "F";
        Console.WriteLine($"if/else-if/else: score {score} -> grade {grade}");

        // ternary operator — condition ? whenTrue : whenFalse
        var parity = score % 2 == 0 ? "even" : "odd";
        Console.WriteLine($"Ternary: {score} is {parity}");

        // classic switch STATEMENT — each non-empty case needs break/return/throw (no C-style
        // fallthrough). Stacking empty case labels to share one body IS allowed.
        var day = 3;
        string dayName;
        switch (day)
        {
            case 1:
            case 7:
                dayName = "Weekend-ish"; // shared body for cases 1 and 7
                break;
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                dayName = "Weekday";
                break;
            default:
                dayName = "Unknown";
                break;
        }
        Console.WriteLine($"Classic switch statement: day {day} -> {dayName}");

        // switch EXPRESSION (C# 8+) — no break, each arm is `pattern => value`, `_` is the
        // default/discard arm. This is what you'll reach for by default in modern C#.
        var dayName2 = day switch
        {
            1 or 7 => "Weekend-ish", // `or` combines multiple patterns on one arm
            2 or 3 or 4 or 5 or 6 => "Weekday",
            _ => "Unknown"
        };
        Console.WriteLine($"Switch expression: day {day} -> {dayName2}");

        // relational patterns — great for bucketing a value into ranges
        var temp = 72;
        var tempDescription = temp switch
        {
            < 32 => "freezing",
            >= 32 and < 60 => "cold", // `and` combines two relational patterns
            >= 60 and < 80 => "mild",
            >= 80 => "hot",
        };
        Console.WriteLine($"Relational patterns: {temp}F -> {tempDescription}");

        // `is` pattern matching — type pattern with an extra condition, an alternative to
        // `as` + null check. `s` is only in scope where the condition is true.
        object obj = "hello";
        if (obj is string s && s.Length > 3)
            Console.WriteLine($"'is' type pattern: obj is a string longer than 3 chars: \"{s}\"");

        // switch on TYPE patterns — handling a value that could be one of several types
        object?[] values = [1, "two", 3.0, null];
        foreach (var value in values)
        {
            var result = value switch
            {
                int i => $"int: {i}",
                string str => $"string: {str}",
                double d => $"double: {d}",
                null => "null",
                _ => "unknown type"
            };
            Console.WriteLine($"Type-pattern switch: {result}");
        }

        // tuple pattern — matching on a combination of values at once (e.g. classifying a point)
        var point = (x: 0, y: 5);
        var quadrant = point switch
        {
            (0, 0) => "origin",
            (0, _) => "on the y-axis",
            (_, 0) => "on the x-axis",
            ( > 0, > 0) => "quadrant I",
            ( < 0, > 0) => "quadrant II",
            ( < 0, < 0) => "quadrant III",
            ( > 0, < 0) => "quadrant IV",
            // no `_` arm needed — the compiler proves these 7 patterns are exhaustive for (int, int)
        };
        Console.WriteLine($"Tuple pattern switch: point {point} -> {quadrant}");

        // `when` guard — an extra boolean condition attached to a switch arm
        var n = 15;
        var fizzBuzz = n switch
        {
            _ when n % 15 == 0 => "FizzBuzz",
            _ when n % 3 == 0 => "Fizz",
            _ when n % 5 == 0 => "Buzz",
            _ => n.ToString()
        };
        Console.WriteLine($"'when' guards (FizzBuzz): {n} -> {fizzBuzz}");
    }
}
