using System.Diagnostics;
using System.Text;

namespace SudokuCrackerCBT;

public readonly struct Sudoku
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public readonly Tile[,] Tiles = new Tile[9, 9];

	public Sudoku() { }
	/// <param name="numbers"> The 81 numbers of the sudoku, from top-left to bottom-right. </param>
	public Sudoku (ReadOnlySpan<byte> numbers)
	{
		for (var i = 0; i < 81; i++) // For every tile in the input
		{
			Tiles[i % 9, i / 9] = numbers[i] != 0
				// If the number is not 0, set the tile to that number, and mark it as fixed
				? new Tile(numbers[i], true,  new int[] { numbers[i] })
				// If the number is 0, set the tile to 0, and mark it as not fixed
				: new Tile(0,  false, Enumerable.Range(1, 9));
		}
	}
	
	/// <returns> A deep-copy of this sudoku instance. </returns>
	public Sudoku Clone ()
	{
		var clone = new Sudoku(); // Initialize a new sudoku
		for (var y = 0; y < 9; y++) for (var x = 0; x < 9; x++) // For every tile in the sudoku
			clone.Tiles[x, y] = Tiles[x, y]; // Copy its contents by value (not reference!)
		return clone;
	}

	public bool AllTilesFilled() // IS NEVER USED
	{
		// Start looking from the lower-right side of the array, to see if a tile has not been set yet.
		// This direction assumes that tiles are set starting from the top-left in reading direction.
		for (var y = 8; y >= 0; y--) for (var x = 8; x >= 0; x--) // For every tile
			if (Tiles[x, y].Value is 0) return false; // If the tile has not been set yet, return false
		return true;
	}
	
	// This is where the ASCII magic happens
	public override string ToString()
	{
		StringBuilder sb = new();
		for (var y = 0; y < 9; y++) // For every row
		{
			for (var x = 0; x < 9; x++) // For every column
			{
				sb.Append($"{Tiles[x,y].ToString()} ");
				if (x is 2 or 5)
					sb.Append("│ ");
			}
			sb.AppendLine();
			
			if (y is 2 or 5)
				sb.AppendLine("──────┼───────┼──────");
		}
		
		return sb.ToString();
	}
}