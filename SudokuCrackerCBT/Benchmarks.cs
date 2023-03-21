using BenchmarkDotNet.Attributes;

namespace SudokuCrackerCBT;

[MemoryDiagnoser]
public class Benchmarks
{
	private readonly Sudoku[] _sudokus;
	public Benchmarks() => _sudokus = Program.LoadSudokus();

	[ParamsSource(nameof(SudokuIndexes))]
	public int Sudoku; // BenchmarkDotNet will fill this by iterating over SudokuIndexes

	public int[] SudokuIndexes => Enumerable.Range(0, _sudokus.Length).ToArray();

	[Benchmark]
	public void CBT()
	{
		Program.SolveSudokuCBT(_sudokus[Sudoku]);
	}
	
	// [Benchmark]
	// public void CBT_MCV()
	// {
	// 	// Program.SolveSudokuCBT(_sudokus[Sudoku]);
	// }
}