// a custom exception is just a class that derives from Exception (by convention the name ends in
// "Exception"). Extra properties carry the details a catch block might need. The primary constructor
// passes the message up to the base Exception, which is what ex.Message returns.
public class InsufficientFundsException(int balance, int requested)
    : Exception($"Cannot withdraw {requested}, balance is only {balance}")
{
    public int Balance { get; } = balance;
    public int Requested { get; } = requested;
}

public static class ExceptionsDemo
{
    public static void Run()
    {
        // basic throw + catch: `throw new XyzException("message")` stops the current method right
        // there and jumps to the nearest matching catch, unwinding the call stack on the way.
        try
        {
            Withdraw(balance: 50, amount: 80);
        }
        catch (InsufficientFundsException ex)
        {
            Console.WriteLine($"caught InsufficientFundsException: {ex.Message} (Balance={ex.Balance}, Requested={ex.Requested})");
        }

        // which built-in exception to throw (pick the most specific one that fits):
        //   ArgumentNullException        an argument is null that must not be
        //   ArgumentOutOfRangeException  an argument is outside the allowed range (negative k, index too big)
        //   ArgumentException            an argument is invalid in some other way
        //   InvalidOperationException    the object is in the wrong state for this call (Pop on an empty stack)
        //   NotImplementedException      a placeholder for code you haven't written yet
        //   NotSupportedException        this operation is never allowed (Add on a read-only collection)
        // Never `throw new Exception(...)` itself: callers can't catch it without catching everything.

        // guard clauses: since .NET 6/8 the argument exceptions have static ThrowIfXyz helpers, which
        // also fill in the parameter name for you (via CallerArgumentExpression).
        try
        {
            KthLargest([3, 1, 2], k: 0);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k): ParamName={ex.ParamName}, Message={ex.Message}");
        }

        try
        {
            KthLargest(null, k: 1);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"ArgumentNullException.ThrowIfNull(nums): ParamName={ex.ParamName}");
        }

        // LeetCode pattern: a method with a return type must return (or throw) on EVERY path, even when
        // the problem guarantees an answer exists (Two Sum: "exactly one solution"). After the loop the
        // compiler still needs something, so throwing there is cleaner than returning a fake [-1, -1].
        Console.WriteLine($"TwoSum([2, 7, 11, 15], 9): [{string.Join(", ", TwoSum([2, 7, 11, 15], 9))}]");
        try
        {
            TwoSum([1, 2], 10);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"TwoSum([1, 2], 10) threw InvalidOperationException: {ex.Message}");
        }

        // throw EXPRESSIONS (C# 7): `throw` can be used as a value inside ??, ?:, and => bodies,
        // so a null check plus assignment fits on one line.
        var config = new Dictionary<string, string> { ["mode"] = "fast" };
        var mode = config.GetValueOrDefault("mode") ?? throw new KeyNotFoundException("mode");
        Console.WriteLine($"config[\"mode\"] ?? throw: {mode}");
        try
        {
            _ = config.GetValueOrDefault("level") ?? throw new KeyNotFoundException("missing key: level");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"config[\"level\"] ?? throw -> KeyNotFoundException: {ex.Message}");
        }

        // multiple catch blocks: the FIRST one whose type matches wins, so order them from most
        // specific to most general. (Putting catch (Exception) first is a compile error, CS0160,
        // because the later ones could never run.) finally runs no matter what: success, caught
        // exception, or even a `return` from inside the try.
        foreach (var input in new[] { "42", "abc", "99999999999" })
        {
            try
            {
                var n = int.Parse(input); // the throwing version; int.TryParse is the non-throwing one
                Console.WriteLine($"int.Parse(\"{input}\"): {n}");
            }
            catch (FormatException)
            {
                Console.WriteLine($"int.Parse(\"{input}\") threw FormatException (not a number)");
            }
            catch (OverflowException)
            {
                Console.WriteLine($"int.Parse(\"{input}\") threw OverflowException (too big for int)");
            }
            finally
            {
                Console.WriteLine($"  finally ran for \"{input}\"");
            }
        }

        // exception filters (`catch ... when (condition)`): the catch only runs if the condition is
        // true; otherwise the exception keeps going up as if this catch weren't there.
        foreach (var amount in new[] { 80, 5000 })
        {
            try
            {
                Withdraw(balance: 50, amount: amount);
            }
            catch (InsufficientFundsException ex) when (ex.Requested > 1000)
            {
                Console.WriteLine($"catch ... when (Requested > 1000): flagged large withdrawal of {ex.Requested}");
            }
            catch (InsufficientFundsException ex)
            {
                Console.WriteLine($"catch (no filter): ordinary shortfall, requested {ex.Requested}");
            }
        }

        // rethrowing: `throw;` rethrows the SAME exception with its original stack trace intact.
        // `throw ex;` restarts the stack trace at this line, hiding where it really came from
        // (analyzer CA2200 warns about it). To add context instead, wrap it: pass the original as
        // the innerException so it's still reachable through ex.InnerException.
        try
        {
            LoadScore("not a number");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"wrapped exception: {ex.Message} | InnerException: {ex.InnerException?.GetType().Name}: {ex.InnerException?.Message}");
        }

        // performance: throwing is SLOW (it captures a stack trace), roughly thousands of times the
        // cost of a normal return. Exceptions are for situations that shouldn't happen. For expected
        // failures in a hot loop (parsing input, looking up keys), use the TryXyz methods instead:
        //   int.TryParse vs int.Parse, dict.TryGetValue vs dict[key], stack.TryPop vs stack.Pop.
        var parsedCount = 0;
        foreach (var token in new[] { "1", "x", "3", "", "5" })
        {
            if (int.TryParse(token, out _))
            {
                parsedCount++;
            }
        }
        Console.WriteLine($"int.TryParse over 5 tokens (no exceptions thrown): {parsedCount} parsed");

        // UnreachableException (.NET 7): marks a code path that should be impossible, e.g. the
        // default arm of a switch over every enum value. Better than a silent `return 0`.
        Console.WriteLine($"Direction 'L' via switch with UnreachableException default: {Turn('L')}");
    }

    private static int Withdraw(int balance, int amount)
    {
        if (amount > balance)
        {
            throw new InsufficientFundsException(balance, amount);
        }

        return balance - amount;
    }

    private static int KthLargest(int[]? nums, int k)
    {
        ArgumentNullException.ThrowIfNull(nums);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(k);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(k, nums.Length);
        return nums.OrderDescending().ElementAt(k - 1);
    }

    private static int[] TwoSum(int[] nums, int target)
    {
        var seen = new Dictionary<int, int>(); // value -> index
        for (var i = 0; i < nums.Length; i++)
        {
            if (seen.TryGetValue(target - nums[i], out var j))
            {
                return [j, i];
            }

            seen[nums[i]] = i;
        }

        throw new InvalidOperationException($"no two numbers add up to {target}");
    }

    private static int LoadScore(string raw)
    {
        try
        {
            return int.Parse(raw);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException($"could not load score from \"{raw}\"", ex);
        }
    }

    private static string Turn(char direction) => direction switch
    {
        'L' => "left",
        'R' => "right",
        _ => throw new System.Diagnostics.UnreachableException($"unknown direction {direction}"),
    };
}
