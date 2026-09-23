public static class MathDemo
{
    public static void Run()
    {
        // integer division truncates TOWARD ZERO, not floor like Python — a classic gotcha with
        // negative operands. Same story for % (remainder takes the sign of the DIVIDEND in C#).
        Console.WriteLine($"7 / 2 = {7 / 2}");
        Console.WriteLine($"-7 / 2 = {-7 / 2}"); // -3, NOT -4 like Python's -7 // 2
        Console.WriteLine($"1 / 2 = {1 / 2}");
        Console.WriteLine($"7 % 2 = {7 % 2}");
        Console.WriteLine($"-7 % 2 = {-7 % 2}"); // -1, NOT 1 like Python's -7 % 2
        Console.WriteLine($"1 % 2 = {1 % 2}");

        // Math.DivRem — quotient and remainder from a single division instead of computing / and % separately
        var (quotient, remainder) = Math.DivRem(7, 2);
        Console.WriteLine($"Math.DivRem(7, 2): quotient={quotient}, remainder={remainder}");

        // Math.Abs — gotcha: int.MinValue has no positive counterpart in the int range (its magnitude
        // is one more than int.MaxValue), so Math.Abs(int.MinValue) throws OverflowException
        // unconditionally, even outside a `checked` block. Widen to long first to sidestep it.
        Console.WriteLine($"Math.Abs(-5): {Math.Abs(-5)}");
        try
        {
            Math.Abs(int.MinValue);
        }
        catch (OverflowException e)
        {
            Console.WriteLine($"Math.Abs(int.MinValue) threw: {e.GetType().Name}");
        }
        Console.WriteLine($"Math.Abs((long)int.MinValue): {Math.Abs((long)int.MinValue)}");

        // Math.Max/Math.Min only take two arguments — for more than two, either nest calls or use
        // LINQ's Max()/Min() over a sequence — O(n), one full pass
        Console.WriteLine($"Math.Max(3, 7): {Math.Max(3, 7)}, Math.Min(3, 7): {Math.Min(3, 7)}");
        int[] numbers = [4, 8, 2, 9, 5];
        Console.WriteLine($"numbers.Max() (LINQ, O(n)): {numbers.Max()}, numbers.Min() (LINQ, O(n)): {numbers.Min()}");

        // Math.Sign — collapses a number to -1, 0, or 1 based on its sign
        Console.WriteLine($"Math.Sign(-5): {Math.Sign(-5)}, Math.Sign(0): {Math.Sign(0)}, Math.Sign(5): {Math.Sign(5)}");

        // Math.Pow always computes in double — fine for approximate results, but for EXACT large
        // integer powers it silently loses precision once the true result exceeds a double's 53-bit
        // mantissa (~9.007e15). Repeated multiplication in long stays exact (until long itself overflows).
        Console.WriteLine($"Math.Pow(2, 10): {Math.Pow(2, 10)}"); // 1024, exact here — small enough
        Console.WriteLine($"(long)Math.Pow(3, 34): {(long)Math.Pow(3, 34)}"); // rounded through double, may be off
        long exactPow = 1;
        for (var i = 0; i < 34; i++)
            exactPow *= 3; // O(exponent); binary exponentiation gets this to O(log exponent)
        Console.WriteLine($"3^34 via repeated multiplication (exact): {exactPow}");

        // Math.Sqrt returns double — for an exact "is this a perfect square?" check, round and
        // re-square rather than trusting the double comparison directly (floating-point sqrt can be
        // slightly off for large inputs).
        const int n = 49;
        var sqrtN = (int)Math.Sqrt(n);
        var isPerfectSquare = sqrtN * sqrtN == n || (sqrtN + 1) * (sqrtN + 1) == n;
        Console.WriteLine($"Is {n} a perfect square? {isPerfectSquare} (Math.Sqrt({n})={Math.Sqrt(n)})");

        // Math.Floor/Ceiling are unsurprising, but Math.Round defaults to "banker's rounding"
        // (MidpointRounding.ToEven) — a .5 value rounds to the NEAREST EVEN integer, not always up.
        // MidpointRounding.AwayFromZero gives the "textbook" rounding most people expect instead.
        Console.WriteLine($"Math.Floor(2.7): {Math.Floor(2.7)}, Math.Ceiling(2.3): {Math.Ceiling(2.3)}");
        Console.WriteLine($"Math.Round(2.5): {Math.Round(2.5)}"); // 2 — rounds down to the nearest even
        Console.WriteLine($"Math.Round(3.5): {Math.Round(3.5)}"); // 4 — rounds up to the nearest even
        Console.WriteLine($"Math.Round(2.5, MidpointRounding.AwayFromZero): {Math.Round(2.5, MidpointRounding.AwayFromZero)}"); // 3

        // floating-point equality gotcha: never compare computed doubles with == — binary
        // floating-point can't represent most decimal fractions exactly, so use an epsilon instead
        Console.WriteLine($"0.1 + 0.2 == 0.3: {0.1 + 0.2 == 0.3}"); // false!
        const double epsilon = 1e-9;
        Console.WriteLine($"Math.Abs((0.1 + 0.2) - 0.3) < epsilon: {Math.Abs(0.1 + 0.2 - 0.3) < epsilon}");

        // integer overflow: C# silently WRAPS AROUND by default (an `unchecked` context) instead of
        // throwing — a classic footgun, and exactly what LeetCode's "Reverse Integer" (LC 7) requires
        // you to detect. Wrapping `checked { }` around the operation turns it into an OverflowException instead.
        var maxValue = int.MaxValue; // through a variable, not a literal — a constant expression would overflow at COMPILE time instead
        var wrapped = unchecked(maxValue + 1);
        Console.WriteLine($"unchecked(int.MaxValue + 1): {wrapped}"); // wraps to int.MinValue
        try
        {
            var overflowed = checked(maxValue + 1);
            Console.WriteLine($"checked(int.MaxValue + 1): {overflowed}");
        }
        catch (OverflowException e)
        {
            Console.WriteLine($"checked(int.MaxValue + 1) threw: {e.GetType().Name}");
        }

        // the practical fix on LeetCode: widen to long BEFORE the operation, then compare against int bounds
        long widened = (long)int.MaxValue + 1;
        Console.WriteLine($"Widened to long first: {widened}, fits in int range: {widened is >= int.MinValue and <= int.MaxValue}");

        // GCD (greatest common divisor) via the Euclidean algorithm — O(log(min(a, b))). Not built
        // into Math for int/long (only System.Numerics.BigInteger.GreatestCommonDivisor exists),
        // so it's worth having memorized — LCM is then just a division and multiplication away.
        Console.WriteLine($"Gcd(48, 18): {Gcd(48, 18)}");
        Console.WriteLine($"Lcm(4, 6): {Lcm(4, 6)}");
    }

    // divide before multiplying to reduce overflow risk when a and b are large
    private static long Lcm(int a, int b) => (long)a / Gcd(a, b) * b;

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
