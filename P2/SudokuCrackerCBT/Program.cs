namespace SudokuCrackerCBT;

internal static class Program
{
	// Parameters: change these as you wish
	private const string SudokuTestsPath = "Sudoku_puzzels_5.txt";
	private const int SUDOKU_INDEX = 0;
	
	private static Sudoku[] _sudokus;
	
	static void Main()
	{
		//RecursionTestTile(new Tile(0, false, new byte[] { }));
		// LoadSudokus();
		// RecursionTestSudoku(_sudokus[0]);
		// return;
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
		
		LoadSudokus();
		// Console.WriteLine("The selected sudoku:");
		// Console.WriteLine(_sudokus[SUDOKU_INDEX].ToString());
        
		// Solve the sudokus
		
		CBT.SetDomains(ref _sudokus[SUDOKU_INDEX]);
		var success = CBT.TrySearch(_sudokus[SUDOKU_INDEX], out var result);

		// Print output
		Console.WriteLine("Result:");
		Console.WriteLine(success ? result.ToString() : "No solution found");
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

	static void RecursionTestTile(Tile t)
	{
		if (t.Value == 8)
		{
			Console.WriteLine("Done!");
			return;
		}
		
		t.Value += 1;
		t.ConstraintAdd((byte) (t.Value + 1));
		
		Console.WriteLine(t.Debug);
		RecursionTestTile(t);
		Console.WriteLine(t.Debug);
	}
	
	static void RecursionTestSudoku(Sudoku s, int i = 0)
	{
		if (i == 3)
		{
			Console.WriteLine("Done!");
			return;
		}
		
		s.Tiles[i, i].Value += 1;
		
		Console.WriteLine(sum());
		RecursionTestSudoku(s.Clone(), i + 1);
		Console.WriteLine(sum());

		int sum()
		{
			var sum = 0;
			for (var y = 0; y < 9; y++) for (var x = 0; x < 9; x++)
				sum += s.Tiles[x, y].Value;
			return sum;
		}
	}
}
