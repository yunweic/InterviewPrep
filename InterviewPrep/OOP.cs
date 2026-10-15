// a plain class — fields are backed by auto-implemented properties (a compiler-generated hidden
// field per property), which is the default way to expose class state in modern C#
public class Animal
{
    public string Name { get; set; } // get/set — freely mutable after construction
    public int Age { get; init; } // init-only — settable during construction/object-initializer only, immutable after

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // virtual — subclasses MAY override this; without `virtual` here, `override` below wouldn't compile
    public virtual string Speak() => $"{Name} makes a sound.";
}

// inheritance — `: Animal` — Dog IS-AN Animal and gets all of Animal's public/protected members
public class Dog : Animal
{
    public Dog(string name, int age) : base(name, age) { } // constructor chaining — forwards to Animal's constructor

    public override string Speak() => $"{Name} says Woof!"; // overrides the base implementation
}

// sealed — GuardDog can still inherit from Dog, but nothing can inherit FROM GuardDog
// (`class PuppyGuard : GuardDog` would not compile). `sealed` can also lock down a single override (see below).
public sealed class GuardDog : Dog
{
    public GuardDog(string name, int age) : base(name, age) { }
    // `public sealed override string Speak() => ...` would also be legal here — sealing a SPECIFIC
    // override so no further subclass can re-override just that one member
}

// primary constructor (C# 12+) — same idea as the LeetCode-provided ListNode/TreeNode types
// elsewhere in this repo, but for an ordinary interview-authored class. Parameters (x, y) are in
// scope for the whole class body, not just a constructor method.
public class Point(int x, int y)
{
    public int X { get; } = x; // get-only — assigned once from the primary constructor parameter, then immutable
    public int Y { get; } = y;
}

// a mutable class, used below to contrast REFERENCE semantics against struct VALUE semantics
public class MutablePoint(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;
}

// static members belong to the TYPE itself, shared across every instance, rather than to any one object
public class Counter
{
    private static int _instancesCreated; // one shared field for the whole type, not per-instance
    public int Id { get; }

    public Counter()
    {
        _instancesCreated++;
        Id = _instancesCreated;
    }

    public static int InstancesCreated => _instancesCreated; // accessed via Counter.InstancesCreated, never an instance
}

// abstract class — cannot be instantiated directly (`new Shape()` is a compile error); exists to be
// subclassed. Mixes an abstract member (must be implemented) with a concrete one (shared as-is).
public abstract class Shape
{
    public abstract double Area(); // no body — every concrete subclass MUST provide one
    public string Describe() => $"This shape has area {Area():0.00}"; // ordinary method, inherited as-is
}

public class Circle(double radius) : Shape
{
    public override double Area() => Math.PI * radius * radius;
}

// interfaces — a type can implement any number of them (unlike single class inheritance)
public interface IMovable
{
    void Move();
}

public interface IHasDefaultGreeting
{
    // default interface method (C# 8+) — implementing types get this behavior for free unless
    // they explicitly provide their own
    string Greet() => "Hello!";
}

public class Robot : IMovable, IHasDefaultGreeting
{
    public void Move() => Console.WriteLine("Robot rolls forward.");
}

// explicit interface implementation — only reachable through a reference typed as the interface,
// not through the class's own type. Useful when a member name would otherwise clash, or to hide
// an implementation detail from the class's normal public surface.
public interface IIdentifiable
{
    string Id { get; }
}

public class Widget : IIdentifiable
{
    string IIdentifiable.Id => "widget-1"; // note: no `public` modifier on an explicit implementation
}

// composition — an interface-typed field/property ("HAS-A") instead of implementing the interface
// ("IS-A"). Car doesn't implement IEngine itself; it HOLDS some IEngine and delegates to it, so
// which engine it uses is decided by whoever constructs the Car (or swapped later), not baked
// into Car's class definition.
public interface IEngine
{
    string Start();
}

public class GasEngine : IEngine
{
    public string Start() => "Vroom (gas)";
}

public class ElectricEngine : IEngine
{
    public string Start() => "Hum (electric)";
}

// a class can do both: Car IS-A IMovable (implements it) and HAS-A IEngine (holds it)
public class Car(IEngine engine) : IMovable
{
    // property, not a private field, because callers are allowed to swap the engine at runtime.
    // If it should be fixed after construction and hidden, use a private field instead:
    // `private readonly IEngine _engine = engine;` (or just use the primary-constructor parameter).
    public IEngine Engine { get; set; } = engine;

    public void Move() => Console.WriteLine($"Car moves: {Engine.Start()}"); // delegates to whichever engine it holds
}

// overriding ToString/Equals/GetHashCode — the contract every LeetCode-style "design a class" question
// eventually runs into: if you override Equals, you MUST also override GetHashCode (equal objects must
// report the same hash code), or the type silently breaks inside Dictionary<K,V>/HashSet<T>.
public class Money(int cents)
{
    public int Cents { get; } = cents;

    public override string ToString() => $"${Cents / 100.0:0.00}"; // called automatically by string interpolation
    public override bool Equals(object? obj) => obj is Money other && Cents == other.Cents;
    public override int GetHashCode() => Cents.GetHashCode();
}

// records — reference types like classes, but the compiler auto-generates value-based Equals,
// GetHashCode, ToString, and deconstruction from the positional parameters, so you don't hand-write
// the Money-style boilerplate above for simple data-holder types.
public record PointRecord(int X, int Y);

// a value type — copied by VALUE on assignment/pass, unlike a class (reference type). Primary
// constructor works on structs too.
public struct PointStruct(int x, int y)
{
    public int X = x;
    public int Y = y;
}

public static class OOPDemo
{
    public static void Run()
    {
        // basic construction + auto-properties
        var dog = new Dog("Rex", 3);
        Console.WriteLine($"{dog.Name} is {dog.Age} years old.");

        // polymorphism — a BASE-CLASS-typed reference pointing at a DERIVED object; the virtual
        // method call dispatches to the derived override at runtime, not the base implementation
        Animal polymorphicPet = dog;
        Console.WriteLine($"polymorphicPet.Speak() (virtual dispatch -> Dog's override): {polymorphicPet.Speak()}");

        // `is` pattern matching — check the runtime type and cast in one step
        if (polymorphicPet is Dog dogPet)
            Console.WriteLine($"Pattern-matched: {dogPet.Name} is specifically a Dog");

        // abstract class — can't do `new Shape()`, but a concrete subclass works fine, and inherits
        // the base class's concrete method for free
        var circle = new Circle(2);
        Console.WriteLine($"circle.Area(): {circle.Area():0.00}, circle.Describe(): {circle.Describe()}");

        // interfaces — default interface method used as-is, ordinary interface method implemented normally
        var robot = new Robot();
        robot.Move();
        // gotcha: a default interface method is NOT part of the implementing class's own public
        // surface — `robot.Greet()` wouldn't compile. It's only reachable through an interface-typed
        // reference, same restriction as explicit interface implementation below.
        IHasDefaultGreeting greeter = robot;
        Console.WriteLine($"greeter.Greet() (default interface method, not overridden): {greeter.Greet()}");

        // explicit interface implementation — NOT visible through the class's own type...
        var widget = new Widget();
        // widget.Id; // ❌ would not compile — Id isn't part of Widget's public surface
        IIdentifiable identifiable = widget; // ...only through an interface-typed reference
        Console.WriteLine($"((IIdentifiable)widget).Id via explicit implementation: {identifiable.Id}");

        // composition — the SAME Car class gets different behavior depending on the IEngine passed in,
        // and the engine can be swapped at runtime. With inheritance you'd need a GasCar and an
        // ElectricCar subclass, and an object can never change its class after it's created.
        var car = new Car(new GasEngine());
        car.Move();
        car.Engine = new ElectricEngine(); // swap behavior at runtime through the interface-typed property
        car.Move();

        // you already use this pattern in LeetCode: Dictionary/HashSet hold an IEqualityComparer<T>
        // and SortedSet/PriorityQueue hold an IComparer<T>, passed in through the constructor. The
        // collection's code stays the same; the comparer you pass in decides how it compares keys.
        var caseInsensitive = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Apple"] = 1 };
        Console.WriteLine($"Dictionary with injected StringComparer.OrdinalIgnoreCase — ContainsKey(\"APPLE\"): {caseInsensitive.ContainsKey("APPLE")}");

        // static members — accessed via the TYPE, shared across every instance
        _ = new Counter();
        _ = new Counter();
        var thirdCounter = new Counter();
        Console.WriteLine($"thirdCounter.Id (instance): {thirdCounter.Id}, Counter.InstancesCreated (static): {Counter.InstancesCreated}");

        // overriding ToString/Equals/GetHashCode
        var m1 = new Money(150);
        var m2 = new Money(150);
        Console.WriteLine($"m1 (uses overridden ToString()): {m1}");
        Console.WriteLine($"m1.Equals(m2) (uses overridden Equals): {m1.Equals(m2)}");
        var moneySet = new HashSet<Money> { m1 };
        Console.WriteLine($"moneySet.Contains(m2) (relies on both Equals AND GetHashCode being overridden): {moneySet.Contains(m2)}");

        // record value equality vs class reference equality
        var r1 = new PointRecord(1, 2);
        var r2 = new PointRecord(1, 2);
        Console.WriteLine($"r1 == r2 (record: compares by VALUE): {r1 == r2}, ReferenceEquals(r1, r2): {ReferenceEquals(r1, r2)}");

        var r3 = r1 with { Y = 99 }; // `with` expression — non-destructive: copies r1, changes only Y
        Console.WriteLine($"r3 = r1 with {{ Y = 99 }}: {r3}, original r1 untouched: {r1}");

        var (rx, ry) = r1; // records get positional deconstruction for free
        Console.WriteLine($"Deconstructed r1: rx={rx}, ry={ry}");

        var classPoint1 = new Point(1, 2);
        var classPoint2 = new Point(1, 2);
        Console.WriteLine($"classPoint1 == classPoint2 (class: reference equality by default, no override): {classPoint1 == classPoint2}");

        // reference semantics (class) vs value semantics (struct)
        var classA = new MutablePoint(1, 2);
        var classB = classA; // copies the REFERENCE — both variables point at the SAME object
        classB.X = 99;
        Console.WriteLine($"Mutating classB also changed classA (reference semantics): classA.X={classA.X}, classB.X={classB.X}");

        var structA = new PointStruct(1, 2);
        var structB = structA; // copies the VALUE — an entirely independent struct
        structB.X = 99;
        Console.WriteLine($"Mutating structB left structA untouched (value semantics): structA.X={structA.X}, structB.X={structB.X}");

        // boxing — converting a value type to `object` (or an interface type) allocates a new object
        // on the HEAP to hold a copy. Easy to miss in interview code (e.g. storing structs in a
        // non-generic collection), and a real performance cost: every boxed value is a heap allocation.
        object boxedStruct = structA; // boxing happens here
        var unboxedBack = (PointStruct)boxedStruct; // unboxing — copies the value back out
        Console.WriteLine($"Boxed/unboxed struct roundtrip: X={unboxedBack.X}, Y={unboxedBack.Y}");
    }
}
