namespace SudokuCracker;

class Program
{
    const string SudokuTestsPath = "Sudoku_puzzels_5.txt";

    static void Main()
    {
        Console.WriteLine("Hello, silas");
    }

    static void LoadSudoku()
    {
        // Dit is maar een random attempt
        string input = "1 2 3 4 5 6 7 8 9";
        var converted = input.Insert(9, "\n").Insert(18, "\n").Split().ToArray()
            .Select(x =>
                x.Select(y => int.Parse(x)))
            .ToArray();
    }
}