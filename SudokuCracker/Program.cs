namespace SudokuCracker;

class Program
{
    private const string SudokuTestsPath = "Sudoku_puzzels_5.txt";
    private static Sudoku[] _sudokus;

    static void Main()
    {
        Console.WriteLine("Hello, silas");
        LoadSudokus();

        foreach (var sudoku in _sudokus)
        {
            Console.WriteLine(sudoku);
            Console.WriteLine();
        }
    }

    static void LoadSudokus()
    {
        var lines = File.ReadAllLines(SudokuTestsPath);

        var sudokuCount = lines.Length / 2;
        _sudokus = new Sudoku[sudokuCount];

        for (var i = 0; i < sudokuCount; i++)
            _sudokus[i] = new Sudoku(lines[1 + i * 2].Split(' ').Skip(1).Select(byte.Parse).ToArray());
                                                    //^^ Compensates for that every line starts with ' '
    }
}