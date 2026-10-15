// a bank account is the classic interview example for `lock`: Withdraw is a CHECK-THEN-ACT
// operation (read the balance, decide, then write it), and without a lock two threads can both pass
// the check before either one subtracts, overdrawing the account.
public class BankAccount(int initialBalance)
{
    // System.Threading.Lock (.NET 9+ / C# 13) is the dedicated lock type and the preferred thing to
    // lock on now. It's private + readonly so no outside code can lock on it (which could deadlock
    // us) and nobody can swap it out for a different object mid-run. Before .NET 9 the idiom was
    // `private readonly object _balanceLock = new();`, which still works the same way.
    private readonly Lock _balanceLock = new();
    private int _balance = initialBalance;

    public int Balance
    {
        get
        {
            // reads take the lock too, so a reader never sees a half-finished update
            lock (_balanceLock)
            {
                return _balance;
            }
        }
    }

    public bool TryWithdraw(int amount)
    {
        // the check AND the subtraction happen inside ONE lock, so no other thread can run
        // between them. Locking only the subtraction would still allow overdrawing.
        lock (_balanceLock)
        {
            if (_balance < amount)
            {
                return false;
            }

            _balance -= amount;
            return true;
        }
    }

    public void Deposit(int amount)
    {
        lock (_balanceLock)
        {
            _balance += amount;
        }
    }

    // lock is REENTRANT: a thread that already holds the lock can enter it again without
    // deadlocking itself, so a locked method can call another locked method on the same object.
    public void DepositTwice(int amount)
    {
        lock (_balanceLock)
        {
            Deposit(amount); // re-acquires _balanceLock on the same thread, which is fine
            Deposit(amount);
        }
    }
}

// LeetCode 1114 "Print in Order": three threads call First/Second/Third in any order, but the
// output must always be first, second, third. `lock` can't express "wait until X happened", so
// this uses SemaphoreSlim as a signal: it starts at 0 (closed), Wait() blocks until another
// thread calls Release() to open it. SemaphoreSlim is IDisposable, so a class that owns one should
// be IDisposable too and dispose it (analyzer CA1001 flags it otherwise).
public sealed class PrintInOrder : IDisposable
{
    private readonly SemaphoreSlim _firstDone = new(0);
    private readonly SemaphoreSlim _secondDone = new(0);

    public void First(Action printFirst)
    {
        printFirst();
        _firstDone.Release(); // let Second run
    }

    public void Second(Action printSecond)
    {
        _firstDone.Wait(); // block until First has released
        printSecond();
        _secondDone.Release(); // let Third run
    }

    public void Third(Action printThird)
    {
        _secondDone.Wait();
        printThird();
    }

    public void Dispose()
    {
        _firstDone.Dispose();
        _secondDone.Dispose();
    }
}

public static class LocksDemo
{
    private static readonly Lock CounterLock = new();

    public static void Run()
    {
        // the problem: `counter++` is NOT atomic. It's three steps (read, add 1, write back), so
        // two threads can both read 5 and both write 6, and one increment is lost. Parallel.For runs
        // the body on several threads at once. The result is usually below 100000 and varies run to run.
        const int iterations = 100_000;
        var unsafeCounter = 0;
        Parallel.For(0, iterations, _ => unsafeCounter++);
        Console.WriteLine($"Unsynchronized counter++ x {iterations} (usually LESS, varies per run): {unsafeCounter}");

        // the fix: lock (obj) { ... } lets only ONE thread at a time run the block. Others wait at
        // the `lock` line until it's released. The lock is released automatically when the block
        // exits, even if it throws.
        var lockedCounter = 0;
        Parallel.For(0, iterations, _ =>
        {
            lock (CounterLock)
            {
                lockedCounter++;
            }
        });
        Console.WriteLine($"counter++ inside lock (CounterLock) x {iterations} (always exact): {lockedCounter}");

        // for a SINGLE number, Interlocked is the lighter option: one atomic CPU instruction, no
        // lock at all. Use lock once you need several steps to happen together (like TryWithdraw).
        // Interlocked needs `ref`, so it only works on fields and locals, not properties.
        var interlockedCounter = 0;
        Parallel.For(0, iterations, _ => Interlocked.Increment(ref interlockedCounter));
        Console.WriteLine($"Interlocked.Increment(ref counter) x {iterations} (always exact, no lock): {interlockedCounter}");

        // check-then-act: 100 threads each try to withdraw 10 from a balance of 500. Exactly 50
        // must succeed and the balance must end at 0, never negative.
        var account = new BankAccount(500);
        var successfulWithdrawals = 0;
        Parallel.For(0, 100, _ =>
        {
            if (account.TryWithdraw(10))
            {
                Interlocked.Increment(ref successfulWithdrawals);
            }
        });
        Console.WriteLine($"BankAccount: 100 parallel TryWithdraw(10) from 500 -> successes: {successfulWithdrawals}, final Balance: {account.Balance}");

        // reentrancy: DepositTwice holds the lock and calls Deposit, which takes the same lock again
        account.DepositTwice(25);
        Console.WriteLine($"BankAccount after DepositTwice(25) (reentrant lock, no self-deadlock): {account.Balance}");

        // what `lock` compiles to:
        //   - locking a System.Threading.Lock:  using (lockObj.EnterScope()) { ... }
        //   - locking any other object:         Monitor.Enter(obj, ref taken); try { ... } finally { if (taken) Monitor.Exit(obj); }
        // Either way, the release is in a finally/Dispose, so an exception can't leave the lock held.
        // Monitor.TryEnter(obj, timeout) is the version that gives up instead of waiting forever.
        object monitorObj = new();
        var acquired = Monitor.TryEnter(monitorObj, TimeSpan.FromMilliseconds(100));
        try
        {
            Console.WriteLine($"Monitor.TryEnter(obj, 100ms) acquired: {acquired}");
        }
        finally
        {
            if (acquired)
            {
                Monitor.Exit(monitorObj);
            }
        }

        // rules for WHAT to lock on:
        //   - DO: a private readonly Lock (or object) field dedicated to locking.
        //   - DON'T lock(this) or lock(typeof(X)): outside code can lock the same object and deadlock you.
        //   - DON'T lock a string: identical string literals are interned (shared app-wide), so
        //     unrelated code locking "abc" is locking YOUR lock.
        //   - DON'T lock a value type (int, struct): it's boxed into a NEW object every time, so each
        //     thread locks a different object and nothing is protected. lock(int) is a compile error
        //     (CS0185), but casting to object first compiles and silently does nothing useful.
        //   - DON'T store a Lock in an `object` variable and lock that: it falls back to Monitor and
        //     loses the Lock type's fast path (compiler warning CS9216).

        // rules for what goes INSIDE a lock:
        //   - keep it short. Every other thread that needs the lock waits the whole time.
        //   - NO `await` inside a lock: it's a compile error (CS1996), because the code after the
        //     await may resume on a DIFFERENT thread, and a lock must be released by the thread that
        //     took it. For async code use SemaphoreSlim(1, 1) with `await sem.WaitAsync()` +
        //     try/finally `sem.Release()`.
        using var asyncGate = new SemaphoreSlim(1, 1);
        var asyncCounter = 0;
        var tasks = Enumerable.Range(0, 50).Select(async _ =>
        {
            await asyncGate.WaitAsync(); // the async-friendly "lock"
            try
            {
                await Task.Yield(); // an await inside the protected section, which `lock` can't allow
                asyncCounter++;
            }
            finally
            {
                asyncGate.Release();
            }
        });
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"SemaphoreSlim(1, 1) as an async lock, 50 tasks each awaiting inside: {asyncCounter}");

        // deadlock: thread 1 locks A then waits for B, while thread 2 locks B then waits for A. Both
        // wait forever. (Not run here, since it would hang the program.) The fix is to always take
        // multiple locks in the SAME global order everywhere (e.g. always A before B; for two
        // accounts in a transfer, lock the one with the lower Id first).

        // often you don't need your own lock at all. The System.Collections.Concurrent types do the
        // locking internally: ConcurrentDictionary, ConcurrentQueue, ConcurrentBag. A plain
        // Dictionary/List is NOT safe to write from multiple threads at once.
        var concurrentCounts = new System.Collections.Concurrent.ConcurrentDictionary<int, int>();
        Parallel.For(0, iterations, i => concurrentCounts.AddOrUpdate(i % 3, 1, (_, old) => old + 1));
        Console.WriteLine($"ConcurrentDictionary.AddOrUpdate from {iterations} parallel iterations (keys 0..2): {string.Join(", ", concurrentCounts.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"))}");

        // signaling between threads (LeetCode concurrency problems): start Third, Second, First in
        // REVERSE order on purpose; the semaphores still force the output into first, second, third.
        using var printer = new PrintInOrder();
        var printed = new System.Collections.Concurrent.ConcurrentQueue<string>();
        var third = Task.Run(() => printer.Third(() => printed.Enqueue("third")));
        var second = Task.Run(() => printer.Second(() => printed.Enqueue("second")));
        var first = Task.Run(() => printer.First(() => printed.Enqueue("first")));
        Task.WaitAll(first, second, third);
        Console.WriteLine($"PrintInOrder (threads started third, second, first): {string.Join(", ", printed)}");
    }
}
