namespace SudokuCracker2;

class Program
{
	// Parameters: change these as you wish
	private const string SudokuTestsPath = "Sudoku_puzzels_5.txt";
	private const int SUDOKU_INDEX  = 0;
	private const int PLATEU_LENGTH = 10;
    
	
	private static Sudoku[] _sudokus;
	
	static void Main()
	{
		// Load sudokus
		LoadSudokus();
		Console.WriteLine("The selected sudoku:");
		Console.WriteLine(_sudokus[SUDOKU_INDEX]);
        
		// Solve the sudoku
		CBT cBT = new CBT();
		CBT.SetDomains(ref _sudokus[SUDOKU_INDEX]);
		Sudoku? solution = cBT.TrySearch(_sudokus[SUDOKU_INDEX]);

		// Print output
		if (solution != null){
			Console.WriteLine(solution);
		} else{
			Console.WriteLine("No solution found");
		}
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
