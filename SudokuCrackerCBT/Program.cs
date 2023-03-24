using BenchmarkDotNet.Running;

namespace SudokuCrackerCBT;

internal static class Program
{
	// Parameters: change these as you wish
	private const bool Benchmark = false; // Set this to false to see the answers to the sudokus
	private const string SudokusPath = "Sudoku_puzzels_5.txt";
	
	static void Main()
	{
		if (Benchmark)
		{
			BenchmarkRunner.Run<Benchmarks>();
		}
		else
		{
			var sudokus = LoadSudokus();
			for (var i = 0; i < sudokus.Length; i++) // For every sudoku loaded
			{
				var result = SolveSudokuCBT(sudokus[i]); // Try to solve it
				
				// Print the result
				Console.WriteLine($"Solution to sudoku {i + 1}:");
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
		// Console.WriteLine(t.DomainNotEmpty());
		// Console.WriteLine(t.Domain().ToArray().Aggregate("", (s, b) => s + b));
		//
		// t.ConstraintAdd(6);
		// t.DomainRemove(2);
		//
		// Console.WriteLine(t.DomainNotEmpty());
		// Console.WriteLine(t.Domain().ToArray().Aggregate("", (s, b) => s + b));
		// return;
		
	}
	
	/// <summary> Tries to solve the sudoku '<paramref name="s"/>' using the CBT algorithm. </summary>
	/// <returns> The sudoku that results after attempting to solve it. </returns>
	public static Sudoku SolveSudokuCBT (Sudoku s)
	{
		// Console.WriteLine("Solving:");
		// Console.WriteLine(s.ToString());
		CBT.SetDomains(ref s);
		CBT.TrySearch(s, out var result);
		return result;
	}

	/// <returns> Parsed sudokus originating from the file specified above in Program.cs </returns>
	public static Sudoku[] LoadSudokus ()
	{
		// Read all sudokus from the file
		var lines = File.ReadAllLines(SudokusPath);

		// Prepare the array of sudokus
		var sudokuCount = lines.Length / 2; // Only every second line contains numbers
		var sudokus = new Sudoku[sudokuCount];

		// Parse the sudokus
		for (var i = 0; i < sudokuCount; i++) // For every sudoku to be parsed
			sudokus[i] = new Sudoku(
				lines[1 + i * 2] // Get the line with the numbers
				.Split(' ')
				.Skip(1) // Compensate for the fact that every line starts with ' '
				.Select(byte.Parse) // Parse characters to bytes
				.ToArray());

		return sudokus;
	}

	// static void RecursionTestTile(Tile t)
	// {
	// 	if (t.Value == 8)
	// 	{
	// 		Console.WriteLine("Done!");
	// 		return;
	// 	}
	// 	
	// 	t.Value += 1;
	// 	t.ConstraintAdd((byte) (t.Value + 1));
	// 	
	// 	Console.WriteLine(t.Debug);
	// 	RecursionTestTile(t);
	// 	Console.WriteLine(t.Debug);
	// }
	//
	// static void RecursionTestSudoku(Sudoku s, int i = 0)
	// {
	// 	if (i == 3)
	// 	{
	// 		Console.WriteLine("Done!");
	// 		return;
	// 	}
	// 	
	// 	s.Tiles[i, i].Value += 1;
	// 	
	// 	Console.WriteLine(sum());
	// 	RecursionTestSudoku(s.Clone(), i + 1);
	// 	Console.WriteLine(sum());
	//
	// 	int sum()
	// 	{
	// 		var sum = 0;
	// 		for (var y = 0; y < 9; y++) for (var x = 0; x < 9; x++)
	// 			sum += s.Tiles[x, y].Value;
	// 		return sum;
	// 	}
	// }
}
