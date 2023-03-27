using BenchmarkDotNet.Running;

namespace SudokuCrackerCBT;

internal static class Program
{
	// Parameters: change these as you wish
	private const bool Benchmark = false; // Set this to false to see the answers to the sudokus
	private const bool UseMostConstrainedVariable = true; // Opt in to MCV heuristics. Does not affect benchmarks
	private const bool CountIterations = true; // Opt in to counting iterations. Does not affect benchmarks
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
				var result = UseMostConstrainedVariable // Try to solve it
					? SolveSudokuCBT_MCV(sudokus[i])
					: SolveSudokuCBT(sudokus[i]);
				
				// Print the result
				Console.WriteLine($"Solution to sudoku {i}:");
				if (CountIterations) {
					var iterations = UseMostConstrainedVariable ? CBT_MCV.IterationCount : CBT.IterationCount;
					Console.WriteLine($"Iterations needed: {iterations}");
				}
				Console.WriteLine(result.ToString());
				
				// Reset iteration counter
				CBT.IterationCount = 0;
				CBT_MCV.IterationCount = 0;
			}
		}
	}
	
	/// <summary> Tries to solve the sudoku '<paramref name="s"/>' using the CBT algorithm. </summary>
	/// <returns> The sudoku that results after attempting to solve it. </returns>
	public static Sudoku SolveSudokuCBT (Sudoku s)
	{
		CBT.SetDomains(ref s);
		CBT.TrySearch(s, out var result);
		return result;
	}
	
	/// <summary> Tries to solve the sudoku '<paramref name="s"/>' using the CBT algorithm using MCV heuristics. </summary>
	/// <returns> The sudoku that results after attempting to solve it. </returns>
	public static Sudoku SolveSudokuCBT_MCV (Sudoku s)
	{
		CBT_MCV.SetDomains(ref s);
		CBT_MCV.TrySearch(s, out var result);
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
}
