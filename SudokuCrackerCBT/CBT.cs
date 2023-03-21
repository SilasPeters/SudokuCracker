namespace SudokuCrackerCBT;

public class CBT
{
	/// <summary>
	/// Performs the CBT algorithm on the sudoku '<paramref name="sudoku"/>'.
	/// This method is called recursively. Every individual call is responsible for finding a value for tile with index
	/// '<paramref name="i"/>'. '<paramref name="i"/>' is incremented by 1 before going into recursion.
	/// </summary>
	/// <param name="sudoku"> The sudoku to solve. </param>
	/// <param name="result"> The state of the sudoku as far as the algorithm could solve it. </param>
	/// <param name="i"> The index of the tile to set. Start with 0. </param>
	/// <returns>
	/// Whether or not a value for the tile with index '<see cref="i"/>' could be found
	/// which leads to a solution.
	/// </returns>
    public static bool TrySearch(Sudoku sudoku, out Sudoku result, int i = 0)
    {
	    // Console.Out.WriteLine("i = {0}", i); // TODO remove
	    // Console.WriteLine(sudoku.ToString());
	    // if (i == 81) // TODO niet AllTilesEmpty()?
	    if (sudoku.AllTilesFilled()) // If all tiles are filled, we must have a solution because
                                     // a tile is only filled when that action is valid
	    {
		    result = sudoku; // Pass on the resulting sudoku
		    return true; // Mark this tiles as successfully filled
	    }
	    
	    int x = i % 9, y = i / 9; // x and y coordinates of the tile with index i
	    
	    if (sudoku.Tiles[x,y].IsFixed) // If the tile is fixed/given, skip it // TODO dit moet worden 'if (sudoku.Tiles[x,y].Value != 0)'
		    return TrySearch(sudoku, out result, i + 1); // Recurse and try the next tile
		 
	    foreach (var c in sudoku.Tiles[x,y].Domain())
	    {
		    sudoku.Tiles[x, y].Value = c; // Set the tile to the next value in it's domain
		    if (!IsPartialAnswer(sudoku, x, y)) continue; //TODO dit moet toch nodig zijn?

		    if (!TryForwardCheck(sudoku, x, y, out var sudokuSimplified)) // Prevent trashing
			    continue; // If the forward check fails, try the next value in the domain
		    if (TrySearch(sudokuSimplified, out result, i + 1)) // Recurse and try the next tile
			    return true; // Only if the recursion succeeds, we stop iterating and report success
	    }
		 
		// sudoku.Tiles[x,y].Value = 0; // TODO: test if solutions are still correct
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

    /// <summary>
    /// Checks if the given sudoku still is a partial answer after setting tile[<paramref name="x"/>,<paramref name="y"/>].
    /// It does so by checking if there are no duplicates in the neighbours of tile[<paramref name="x"/>,<paramref name="y"/>].
    /// </summary>
    /// <returns> Whether no constraint is violated. </returns>
    private static bool IsPartialAnswer (in Sudoku s, int x, int y)
    {
        // Check if there are no duplicates in the column of tile[x,y]
        Span<bool> found = stackalloc bool[10];
		for (var row = 0; row < 9; row++)
		{
			// if (row == y) continue;
			var value = s.Tiles[x, row].Value;

			if (value == 0) continue; // Empty tiles are not considered duplicates
			if (found[value]) return false;
			found[value] = true; // Already found, constraint violated
		}
		
        // Check if there are no duplicates in the row of tile[x,y]
        found = stackalloc bool[10];
		for (var column = 0; column < 9; column++)
		{
			// if (column == x) continue;
			var value = s.Tiles[column, y].Value;
			
			if (value == 0) continue; // Empty tiles are not considered duplicates
			if (found[value]) return false;
			found[value] = true; // Already found, constraint violated
		}
		
        // Check if there are no duplicates in the block of tile[x,y]
        found = stackalloc bool[10];
		int blockX = x - x % 3, // Coordinates of the top-left tile of the block of tile[x,y]
			blockY = y - y % 3;
		for (var by = 0; by < 3; by++) for(var bx = 0; bx < 3; bx++)
			// if (bx != x || by != y) // Skip the set tile
		{
			var value = s.Tiles[blockX + bx, blockY + by].Value;
			
			if (value == 0) continue; // Empty tiles are not considered duplicates
			if (found[value]) return false;
			found[value] = true; // Already found, constraint violated
		}

		return true; // No constraint violations found
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
