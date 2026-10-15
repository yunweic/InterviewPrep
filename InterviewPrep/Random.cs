public static class RandomDemo
{
    public static void Run()
    {
        // Random.Shared (.NET 6+) — a ready-made, THREAD-SAFE instance. The default choice when you
        // just need "some random number" and don't care about reproducibility. Output differs every run.
        Console.WriteLine($"Random.Shared.Next(1, 7) (dice roll, differs each run): {Random.Shared.Next(1, 7)}");

        // seeded Random — the same seed always produces the same sequence, so results are reproducible
        // (handy for debugging/tests). Every other example below uses this so the console trace is stable.
        // A plain `new Random()` instance is NOT thread-safe — share Random.Shared across threads instead.
        var rng = new Random(42);
        var sameSeed = new Random(42);
        Console.WriteLine($"new Random(42).Next(100) twice from two instances: {rng.Next(100)} and {sameSeed.Next(100)} (identical)");

        // Next overloads — the UPPER bound is EXCLUSIVE, the classic off-by-one gotcha:
        //   Next()          -> [0, int.MaxValue)
        //   Next(max)       -> [0, max)
        //   Next(min, max)  -> [min, max)   so a 1-6 dice roll is Next(1, 7), not Next(1, 6)
        Console.WriteLine($"Next(): {rng.Next()}, Next(10) -> [0,10): {rng.Next(10)}, Next(1, 7) -> [1,7): {rng.Next(1, 7)}");

        // other types: NextDouble -> [0.0, 1.0), NextInt64 for long ranges, NextBytes fills a buffer
        var bytes = new byte[4];
        rng.NextBytes(bytes);
        Console.WriteLine($"NextDouble() [0,1): {rng.NextDouble():0.000}, NextInt64(1, 10_000_000_000): {rng.NextInt64(1, 10_000_000_000)}, NextBytes(4): [{string.Join(", ", bytes)}]");

        // random double in an arbitrary range [min, max) — scale and shift NextDouble
        double min = 5.0, max = 10.0;
        Console.WriteLine($"Random double in [5, 10) via min + NextDouble() * (max - min): {min + (rng.NextDouble() * (max - min)):0.000}");

        // pick a random element — O(1) on anything indexable. Next(Count) is already [0, Count),
        // so it can never go out of bounds.
        List<string> fruits = ["apple", "banana", "cherry", "date"];
        Console.WriteLine($"Random element via fruits[rng.Next(fruits.Count)]: {fruits[rng.Next(fruits.Count)]}");

        // GetItems (.NET 8+) — pick `count` items WITH replacement (the same item can repeat)
        Console.WriteLine($"rng.GetItems(fruits array, 5) (with replacement): [{string.Join(", ", rng.GetItems(fruits.ToArray(), 5))}]");

        // Shuffle (.NET 8+) — in-place Fisher-Yates over an array or Span, O(n)
        int[] deck = [1, 2, 3, 4, 5, 6];
        rng.Shuffle(deck);
        Console.WriteLine($"rng.Shuffle(deck) (in place): [{string.Join(", ", deck)}]");

        // hand-written Fisher-Yates — what an interviewer expects if asked to "shuffle an array"
        // (LeetCode 384). Walk from the end; swap each slot with a random index in [0, i] INCLUSIVE.
        // Every permutation is equally likely; O(n) time, O(1) extra space.
        int[] manualDeck = [1, 2, 3, 4, 5, 6];
        FisherYatesShuffle(manualDeck, rng);
        Console.WriteLine($"FisherYatesShuffle(manualDeck): [{string.Join(", ", manualDeck)}]");

        // gotcha: the "naive" shuffle — swapping with Next(n) (the WHOLE array) instead of Next(i + 1) —
        // looks equivalent but is BIASED: it produces n^n equally likely swap sequences, which can't
        // divide evenly into n! permutations. Same trap: OrderBy(_ => rng.Next()) works but is O(n log n).
        var orderByShuffle = fruits.OrderBy(_ => rng.Next()).ToList();
        Console.WriteLine($"fruits.OrderBy(_ => rng.Next()) (works, but O(n log n) vs Fisher-Yates O(n)): [{string.Join(", ", orderByShuffle)}]");

        // random sample WITHOUT replacement — shuffle a copy and take the first k. O(n).
        int[] pool = [10, 20, 30, 40, 50];
        int[] poolCopy = [.. pool];
        rng.Shuffle(poolCopy);
        Console.WriteLine($"Sample 3 without replacement (shuffle copy, Take(3)): [{string.Join(", ", poolCopy.Take(3))}], original untouched: [{string.Join(", ", pool)}]");

        // LeetCode 380 "Insert Delete GetRandom O(1)" — List gives O(1) random index access,
        // Dictionary gives O(1) value -> index lookup. Remove swaps the target with the LAST element
        // then RemoveAt(Count - 1), because removing from the end of a List is O(1) (vs O(n) mid-list).
        var randomizedSet = new RandomizedSet(rng);
        randomizedSet.Insert(1);
        randomizedSet.Insert(2);
        randomizedSet.Insert(3);
        var removed = randomizedSet.Remove(2);
        Console.WriteLine($"RandomizedSet after Insert(1,2,3), Remove(2)={removed}: GetRandom() -> {randomizedSet.GetRandom()}, Insert(1) again (duplicate): {randomizedSet.Insert(1)}");

        // LeetCode 528 "Random Pick with Weight" — build prefix sums once (O(n)), then each pick is a
        // random target in [0, total) + binary search for the first prefix sum > target (O(log n)).
        // weights [1, 3] -> prefix [1, 4]: index 0 owns [0,1), index 1 owns [1,4) -> 25% / 75%.
        var weightedPicker = new WeightedPicker([1, 3], rng);
        var pickCounts = new int[2];
        for (var i = 0; i < 10_000; i++)
            pickCounts[weightedPicker.PickIndex()]++;
        Console.WriteLine($"WeightedPicker([1, 3]) over 10,000 picks: index 0 = {pickCounts[0]}, index 1 = {pickCounts[1]} (expect ~2,500 / ~7,500)");

        // reservoir sampling (LeetCode 382) — pick ONE uniformly random item from a stream of unknown
        // length in a single pass, O(1) space: keep the i-th item (1-based) with probability 1/i.
        var reservoirCounts = new Dictionary<int, int>();
        for (var trial = 0; trial < 9_000; trial++)
        {
            var chosen = ReservoirSample(Enumerable.Range(1, 3), rng);
            reservoirCounts[chosen] = reservoirCounts.GetValueOrDefault(chosen) + 1;
        }
        Console.WriteLine($"ReservoirSample over stream [1,2,3], 9,000 trials: {string.Join(", ", reservoirCounts.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}"))} (expect ~3,000 each)");

        // Random is NOT cryptographically secure — fine for algorithms/games, never for tokens,
        // passwords, or keys. Use System.Security.Cryptography.RandomNumberGenerator for those.
        // (Its GetInt32 upper bound is exclusive too.)
        Console.WriteLine($"RandomNumberGenerator.GetInt32(100) (crypto-secure, differs each run): {System.Security.Cryptography.RandomNumberGenerator.GetInt32(100)}");
    }

    private static void FisherYatesShuffle(int[] arr, Random rng)
    {
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1); // [0, i] inclusive — i + 1 because the upper bound is exclusive
            (arr[i], arr[j]) = (arr[j], arr[i]); // tuple swap
        }
    }

    private static int ReservoirSample(IEnumerable<int> stream, Random rng)
    {
        var chosen = 0;
        var seen = 0;
        foreach (var item in stream)
        {
            seen++;
            if (rng.Next(seen) == 0) // true with probability 1/seen
                chosen = item;
        }
        return chosen;
    }

    private sealed class RandomizedSet(Random rng)
    {
        private readonly List<int> _values = [];
        private readonly Dictionary<int, int> _indexOf = [];

        public bool Insert(int val)
        {
            if (_indexOf.ContainsKey(val)) return false;
            _indexOf[val] = _values.Count;
            _values.Add(val);
            return true;
        }

        public bool Remove(int val)
        {
            if (!_indexOf.TryGetValue(val, out var index)) return false;
            var last = _values[^1];
            _values[index] = last; // move the last element into the hole
            _indexOf[last] = index;
            _values.RemoveAt(_values.Count - 1); // O(1) — removing from the end shifts nothing
            _indexOf.Remove(val);
            return true;
        }

        public int GetRandom() => _values[rng.Next(_values.Count)];
    }

    private sealed class WeightedPicker
    {
        private readonly int[] _prefixSums;
        private readonly Random _rng;

        public WeightedPicker(int[] weights, Random rng)
        {
            _rng = rng;
            _prefixSums = new int[weights.Length];
            var running = 0;
            for (var i = 0; i < weights.Length; i++)
            {
                running += weights[i];
                _prefixSums[i] = running;
            }
        }

        public int PickIndex()
        {
            var target = _rng.Next(_prefixSums[^1]); // [0, total)
            // binary search for the FIRST prefix sum strictly greater than target
            int lo = 0, hi = _prefixSums.Length - 1;
            while (lo < hi)
            {
                var mid = lo + ((hi - lo) / 2);
                if (_prefixSums[mid] > target) hi = mid;
                else lo = mid + 1;
            }
            return lo;
        }
    }
}
