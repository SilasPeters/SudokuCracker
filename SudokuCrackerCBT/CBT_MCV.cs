namespace SudokuCrackerCBT;

// public struct TileOrdering
// {
// 	private readonly byte[] _tileIndexes;
//
// 	public TileOrdering(byte[] tileIndexes)
// 	{
// 		_tileIndexes = tileIndexes;
// 	}
//
// 	public byte First => _tileIndexes[0];
//
// 	public void Order(Tile[] tiles)
// 	{
// 		Array.Sort(_tileIndexes, (a, b) =>
// 		{	// Specify ordering
// 			// Chosen ordering: non-set tiles sorted on smallest domain first, set tiles
// 			// second (disregarding their domain size)
// 			ref var A = ref tiles[a];
// 			ref var B = ref tiles[b];
// 			
// 			if (A.Value != 0 || B.Value != 0) // Some tile is set already
// 				return A.Value != 0 ? B.Value != 0 ? 0 : 1 : -1; // Determine sort based on if both are fixed
// 			return A.Domain().Count() - B.Domain().Count(); // Sort on domain size
// 		});
// 	}
// }

public class CBT_MCV
{
	public static int IterationCount = 0;
	
	/// <summary>
	/// Finds the tile in '<paramref name="tiles"/>' which has the smallest domain within the tiles not set yet.
	/// </summary>
	/// <param name="tiles">The tiles array to search through</param>
	/// <returns>The x and y coordinates of the Most Constrained Variable</returns>
	/// <remarks>If all tiles are set, the coordinates returned do belong to a tile which is already set</remarks>
	static (int, int) MostConstrainedVariable(in Tile[,] tiles)
	{
		var smallestDomain = 10; // All tiles have a smaller domain than this
		var (resX, resY) = (0, 0);
		for (var y = 0; y < 9; y++) for (var x = 0; x < 9; x++)
		{
			if (tiles[x,y].Value != 0) continue; // Skip if tile is already set
			
			var domainSize = tiles[x, y].Domain().Count();
			if (domainSize < smallestDomain)
			{
				smallestDomain = domainSize;
				(resX, resY)   = (x, y);
			}
		}
		return (resX, resY);
	}

	/// <summary>
	/// Performs the CBT algorithm on the sudoku '<paramref name="sudoku"/>', using Most-Constrained-Variable heuristics.
	/// This method is called recursively. Every individual call is responsible for finding a value for the then
	/// most-constrained variable.
	/// </summary>
	/// <param name="sudoku"> The sudoku to solve. </param>
	/// <param name="result"> The state of the sudoku as far as the algorithm could solve it. </param>
	/// <returns> Whether or not the current state leads to a solution of the sudoku. </returns>
    public static bool TrySearch(Sudoku sudoku, out Sudoku result)
    {
		IterationCount++; // Used for statistics only

	    // These are the coordinates of the tile with the smallest domain, which is not yet set
	    // If the tile is somehow set already, this means that all tiles have been set and the sudoku is solved
	    var (x, y) = MostConstrainedVariable(sudoku.Tiles);
	    
		if (sudoku.Tiles[x,y].Value != 0) // If the tile is set already, all tiles are filled. The sudoku is solved
	    {
		    result = sudoku; // Pass on the resulting sudoku
		    return true; // Mark this tiles as successfully set
	    }
	    
	    // if (sudoku.Tiles[x,y].Value != 0) // If the tile is fixed/given, skip it
	    //  return TrySearch(sudoku, out result, i + 1); // Recurse and try the next tile
		 
	    foreach (var c in sudoku.Tiles[x,y].Domain())
	    {
		    sudoku.Tiles[x, y].Value = c; // Set the tile to the next value in it's domain
		    
		    if (!TryForwardCheck(sudoku, x, y, out var sudokuSimplified)) // Prevent trashing
			    continue; // If the forward check fails, try the next value in the domain
		    if (TrySearch(sudokuSimplified, out result)) // Recurse and try the next tile
			    return true; // Only if the recursion succeeds, we stop iterating and report success
	    }
		 
		result = sudoku; // Pass on the resulting sudoku
		return false; // Given the current state, no value in the domain leads to a solution
    }

    /// <summary>
    /// Simplifies the domain of tiles neighbouring the tile[<paramref name="x"/>,<paramref name="y"/>] that
    /// was set before requiring this local forward check afterwards.
    /// </summary>
    /// <param name="sudoku"> The current state of the sudoku. </param>
    /// <param name="x"> The x-coordinate of the tile which was set before this check. </param>
    /// <param name="y"> The y-coordinate of the tile which was set before this check. </param>
    /// <param name="result"> The sudoku with simplified domains. Only partially simplified if the check fails. </param>
    /// <returns> Whether the forward check did succeed, and thus if all domains where simplified to a non-empty set only. </returns>
    private static bool TryForwardCheck (in Sudoku sudoku, int x, int y, out Sudoku result)
    {
		result = sudoku.Clone(); // Create a deep-copy of the current state of the sudoku.
                           // Changes must be made to this copy to prevent persistent mutations.
		var valueOfSetTile = sudoku.Tiles[x, y].Value; // The value of the tile that was set

		// Process the row containing the tile that was set
		for (var column = 0; column < 9; column++)
		{
			if (column == x) continue; // Skip the tile that was set
			ref var tile = ref result.Tiles[column, y];

			tile.DomainRemove(valueOfSetTile); // Remove redundant value from the domain
			if (!tile.DomainNotEmpty())
				return false; // Report that backtracking is required
		}

		// Process the column containing the tile that was set
		for (var row = 0; row < 9; row++)
		{
			if (row == y) continue; // Skip the tile that was set
			ref var tile = ref result.Tiles[x, row];
			
			tile.DomainRemove(valueOfSetTile); // Remove redundant value from the domain
			if (!tile.DomainNotEmpty())
				return false; // Report that backtracking is required
		}
		
		// Process the remaining tiles of the block containing the tile that was set
		int blockX = x - x % 3, // Coordinates of the top-left tile of the block of the tile that was set
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx == x || by == y) continue; // Skip tiles already checked above (and the set tile)
			ref var tile = ref result.Tiles[bx, by];
			
			tile.DomainRemove(valueOfSetTile); // Remove redundant value from the domain
			if (!tile.DomainNotEmpty())
				return false; // Report that backtracking is required
		}

		return true; // All domains of neighbouring tiles simplified successfully
    }

    /// <summary> Makes a given sudoku node-consistent by simplifying domains </summary>
    public static void SetDomains (ref Sudoku sudoku)
    {
	    for (byte y = 0; y < 9; y++) for (byte x = 0; x < 9; x++)
			if (sudoku.Tiles[x, y].Value is not 0) // For every non-empty tile
				TryForwardCheck(sudoku, x, y, out sudoku); // Simplify neighbouring domains
				// ^ This will always succeed in this case, even though it says 'try'
    }
}
