Console.WriteLine("Hello, World!");

RunSection(nameof(ArraysDemo), ArraysDemo.Run);
RunSection(nameof(ListsDemo), ListsDemo.Run);
RunSection(nameof(DictionariesDemo), DictionariesDemo.Run);
RunSection(nameof(SetsDemo), SetsDemo.Run);
RunSection(nameof(StacksDemo), StacksDemo.Run);
RunSection(nameof(QueuesDemo), QueuesDemo.Run);
RunSection(nameof(StringsDemo), StringsDemo.Run);
RunSection(nameof(NullableDemo), NullableDemo.Run);
RunSection(nameof(LinkedListsDemo), LinkedListsDemo.Run);
RunSection(nameof(TreesDemo), TreesDemo.Run);
RunSection(nameof(RecursionDemo), RecursionDemo.Run);
RunSection(nameof(Arrays2DDemo), Arrays2DDemo.Run);
RunSection(nameof(SortingDemo), SortingDemo.Run);
RunSection(nameof(HeapsDemo), HeapsDemo.Run);
RunSection(nameof(ConditionalsDemo), ConditionalsDemo.Run);

// prints a header before each topic so console output doesn't blur into one wall of text
void RunSection(string name, Action run)
{
    Console.WriteLine();
    Console.WriteLine($"===== {name} =====");
    run();
}
