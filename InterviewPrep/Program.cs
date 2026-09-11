Console.WriteLine("Hello, World!");

// create a number array
var numArray = new[] { 1, 2, 3, 4, 5 };
Console.WriteLine($"Number array: {string.Join(", ", numArray)}");

// create a string array
var strArray = new[] { "a", "b", "c", "d", "e" };
Console.WriteLine($"String array: {string.Join(", ", strArray)}");

var repeatedStr = new string('a', 9);
Console.WriteLine($"Repeated string ('a' x 9): {repeatedStr}");

// create a list of int
var numList = new List<int> { 1, 2, 3, 4, 5 };
Console.WriteLine($"Number list: {string.Join(", ", numList)}");

// add all the number in the list
var total = numList.Sum();
Console.WriteLine($"Sum of number list: {total}");

// create a list of string
var strList = new List<string> { "a", "b", "c", "d", "e" };
Console.WriteLine($"String list: {string.Join(", ", strList)}");
Console.WriteLine($"String list joined with no separator: {string.Join("", strList)}");

// add an element to list
strList.Add("f");
Console.WriteLine($"String list after Add(\"f\"): {string.Join(", ", strList)}");

// remove an element from the list
strList.Remove("f");
Console.WriteLine($"String list after Remove(\"f\"): {string.Join(", ", strList)}");

// access the last element on the list
Console.WriteLine($"Last element via strList[^1]: {strList[^1]}");

// remove a list element at a certain index
strList.RemoveAt(strList.Count - 1);
Console.WriteLine($"String list after RemoveAt(last index): {string.Join(", ", strList)}");

// create a dictionary
var lookupTable = new Dictionary<int, string>
{
    { 1, "a" },
    { 2, "b" },
};
Console.WriteLine($"Dictionary (key-value pairs): {string.Join(", ", lookupTable)}");

// create a set
var mySet = new HashSet<int> { 1, 2, 3, 4, 5 };
Console.WriteLine($"HashSet: {string.Join(", ", mySet)}");
mySet.Add(6);
Console.WriteLine($"HashSet after Add(6): {string.Join(", ", mySet)}");
mySet.Remove(6);
Console.WriteLine($"HashSet after Remove(6): {string.Join(", ", mySet)}");

//stack
var stack = new Stack<int>();
stack.Push(1);
stack.Push(2);
stack.Push(3);
foreach (var item in stack)
    Console.WriteLine($"Stack item (foreach, top-to-bottom order): {item}");

Console.WriteLine($"Stack.Peek() (top item, not removed): {stack.Peek()}");
var poppedItem = stack.Pop();
Console.WriteLine($"Stack.Pop() (removed top item): {poppedItem}");

stack.Clear();
bool success = stack.TryPeek(out int value);
Console.WriteLine($"TryPeak result = {success},  value = {value}");
// value == default(int) == 0   (for reference types, it'd be null)

bool popped = stack.TryPop(out int poppedValue);
Console.WriteLine($"TryPop() result = {popped},  value = {poppedValue}");
// poppedValue == default(int) == 0

//use nullable
var listNode = new ListNode(1, null);
ListNode? listNode2 = null;

var l1Val = listNode?.Value ?? 0;
var l2Val = listNode2?.Value ?? 0;
Console.WriteLine($"Null-conditional/coalescing results -> listNode.Value: {l1Val}, listNode2 (null).Value defaulted: {l2Val}");

if (listNode2 is null)
    Console.WriteLine($"Notice you can print a variable with null like {listNode2}");

if (listNode2 == null)
    Console.WriteLine($"Notice you can print a variable with null like {listNode2}");


/* if (null) { }      // ❌ compile error — cannot convert null to bool
if (0) { }          // ❌ compile error — cannot convert int to bool
if ("") { }         // ❌ compile error — cannot convert string to bool
if (someList) { }   // ❌ compile error — cannot convert List<T> to bool


if (obj == null) { }         // ✅ explicit null check
if (num == 0) { }             // ✅ explicit comparison
if (str == "") { }            // ✅ or string.IsNullOrEmpty(str)
if (list.Count == 0) { }      // ✅ or list.Count > 0 for the positive case
*/

public class ListNode(int val, ListNode? n)
{
    public int Value = val;
    public ListNode? Next = n;
}
