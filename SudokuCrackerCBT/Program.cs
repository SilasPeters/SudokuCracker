using BenchmarkDotNet.Running;

namespace SudokuCrackerCBT;

internal static class Program
{
	// Parameters: change these as you wish
	private const bool Benchmark = false; // Set this to false to see the answers
	private const string SudokuTestsPath = "Sudoku_puzzels_5.txt";
	
	static void Main()
	{
		if (Benchmark)
		{
			BenchmarkRunner.Run<Benchmarks>();
		}
		else
		{
			var sudokus = LoadSudokus();
			for (var i = 0; i < sudokus.Length; i++)
			{
				var result = SolveSudoku(sudokus[i]);
				
				// Print output
				Console.WriteLine($"Result {i}:");
				Console.WriteLine(result.ToString());
			}
		}
		
		//RecursionTestTile(new Tile(0, false, new byte[] { }));
		// LoadSudokus();
		// RecursionTestSudoku(Sudokus[0]);
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
		
	}
	
	public static Sudoku SolveSudoku(Sudoku s)
	{
		CBT.SetDomains(ref s);
		CBT.TrySearch(s, out var result);
		return result;
	}

	public static Sudoku[] LoadSudokus()
	{
		// Read all sudokus from the file
		var lines = File.ReadAllLines(SudokuTestsPath);

		// Prepare the array of sudokus
		var sudokuCount = lines.Length / 2;
		var sudokus = new Sudoku[sudokuCount];

		// For every sudoku to be read, trim only the lines containing numbers, and parse it to bytes
		for (var i = 0; i < sudokuCount; i++)
			sudokus[i] = new Sudoku(lines[1 + i * 2].Split(' ').Skip(1).Select(byte.Parse).ToArray());
		//^^ Compensates for that every line starts with ' '

		return sudokus;
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
