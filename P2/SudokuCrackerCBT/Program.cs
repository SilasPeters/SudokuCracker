namespace SudokuCrackerCBT;

internal static class Program
{
	// Parameters: change these as you wish
	private const string SudokuTestsPath = "Sudoku_puzzels_5.txt";
	private const int SUDOKU_INDEX  = 0;
	private const int PLATEU_LENGTH = 10;
    
	
	private static Sudoku[] _sudokus;
	
	static void Main()
	{
		// Tile t = new(0, false, new byte[] {});
		//
		// Console.WriteLine(t);
		// Console.WriteLine(t.Value);
		// Console.WriteLine(t.IsFixed);
		// Console.WriteLine(t.ConstraintAny());
		// Console.WriteLine(t.Constraint().ToArray().Aggregate("", (s, b) => s + b));
		//
		// t.ConstraintAdd(6);
		// t.ConstraintRemove(2);
		//
		// Console.WriteLine(t.ConstraintAny());
		// Console.WriteLine(t.Constraint().ToArray().Aggregate("", (s, b) => s + b));
		// return;
		
		// Load sudokus
		LoadSudokus();
		Console.WriteLine("The selected sudoku:");
		Console.WriteLine(_sudokus[SUDOKU_INDEX]);
        
		// Solve the sudoku
		CBT.SetDomains(ref _sudokus[SUDOKU_INDEX]);
		
		var success = CBT.TrySearch(_sudokus[SUDOKU_INDEX], out var solution);

		// Print output
		Console.WriteLine(solution);
		if (!success) Console.WriteLine("Whoopsie daisy, you caught us! We could not find a solution.");
	}

	static void LoadSudokus()
	{
		// Read all soduokus from the file
		var lines = File.ReadAllLines(SudokuTestsPath);

		// Prepare the array of sudokus
		var sudokuCount = lines.Length / 2;
		_sudokus = new Sudoku[sudokuCount];

		// For every sudoku to be read, trim only the lines containing numbers, and parse it to bytes
		for (var i = 0; i < sudokuCount; i++)
			_sudokus[i] = new Sudoku(lines[1 + i * 2].Split(' ').Skip(1).Select(byte.Parse).ToArray());
		//^^ Compensates for that every line starts with ' '
	}
}
