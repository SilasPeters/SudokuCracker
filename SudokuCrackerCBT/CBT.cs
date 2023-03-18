namespace SudokuCrackerCBT;

public class CBT
{
    public static bool TrySearch(Sudoku sudoku, out Sudoku result, int i = 0)
    {
	    // Console.Out.WriteLine("i = {0}", i);
	    // Console.WriteLine(sudoku.ToString());
	    if (i == 81)
	    {
		    result = sudoku;
		    return true;
	    }

	    int x = i % 9, y = i / 9;
	    
	    if (sudoku.Tiles[x,y].IsFixed) // Don't waste time on branching if the value is fixed
		    return TrySearch(sudoku, out result, i + 1);
		 
	    foreach (var c in sudoku.Tiles[x,y].Constraint())
	    {
		    sudoku.Tiles[x, y].Value = c;
		    // if (!ChangeWasValidOn(sudoku, x, y)) continue;
		    
		    if (TryForwardCheck(sudoku, x, y, out var sudokuSimplified)) 
			    if (TrySearch(sudokuSimplified, out result, i + 1)) return true;
	    }
		 
		sudoku.Tiles[x,y].Value = 0;
		result = sudoku;
		return false;


	 //    var result = sudoku;
  //       if (sudoku.AllTilesFilled())
	 //        return true;
  //       byte x = (byte)(i % 9), y = (byte)(i / 9);
  //       
  //       if (sudoku.Tiles[x,y].IsFixed) // Value is fixed, don't waste time on branching
	 //        return TrySearch(sudoku, out result, ++i);
  //       
	 //    foreach(var s in sudoku.Tiles[x,y].Constraint()) {
		//     sudoku.Tiles[x,y].Value = s;
		//     if (!ChangeWasValidOn(ref sudoku, x, y)) continue;
		//     if (TryForwardCheck(sudoku, x, y, out var sdkSimplified)) {
		// 	    if (TrySearch(sdkSimplified, out var attempt, ++i)) {
		// 		    result = attempt;
		// 		    return true;
		// 	    }
		//     }
	 //    }
	 //    
		// return false;
    }

    /// <summary>
    /// After setting a tile, the constraint should be simplified by performing this forward check.
    /// This check removes the value of tile[<paramref name="x"/>,<paramref name="y"/>] from the domain of every tile in the block, row and column containing
    /// tile[<paramref name="x"/>,<paramref name="y"/>]. If a domain turns out to be empty afterwards, it exits and reports failure.
    /// </summary>
    /// <param name="sudoku"> The sudoku to mutate. </param>
    /// <param name="x"> The x-coordinate of the tile which was set before this check. </param>
    /// <param name="y"> The y-coordinate of the tile which was set before this check. </param>
    /// <param name="result"> The sudoku with simplified constraints/domains. Only partially simplified if the check fails. </param>
    /// <returns> Whether the forward check did succeed, and thus if all domains where simplified to a non-empty set only. </returns>
    private static bool TryForwardCheck(Sudoku sudoku, int x, int y, out Sudoku result)
    {
		result = sudoku.Clone();
		var valueOfSetTile = sudoku.Tiles[x, y].Value; // The value of the tile that was set

		// Process the row containing the tile that was set
		for (var column = 0; column < 9; column++)
		{
			if (column == x) continue; // Skip the tile that was set
			ref var tile = ref result.Tiles[column, y];

			tile.ConstraintRemove(valueOfSetTile);
			if (!tile.ConstraintAny())
				return false;
		}

		// Process the column containing the tile that was set
		for (var row = 0; row < 9; row++)
		{
			if (row == y) continue; // Skip the tile that was set
			ref var tile = ref result.Tiles[x, row];
			
			tile.ConstraintRemove(valueOfSetTile);
			if (!tile.ConstraintAny())
				return false;
		}
		
		// Process the remains of the block containing the tile that was set
		int blockX = x - x % 3, // Coordinates of the top-left tile of the block of the tile that was set
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx == x || by == y) continue; // Skip tiles already checked above (and the set tile)
			ref var tile = ref result.Tiles[bx, by];
			
			tile.ConstraintRemove(valueOfSetTile);
			if (!tile.ConstraintAny())
				return false;
		}

		return true;
		
		// TODO: propagated ForwardCheck is still an option
    }

    /// <summary> Checks if the given sudoku still is a partial answer after setting tile[x,y].
    /// It does so by checking if there are no duplicates in the neighbours of tile[x,y] </summary>
    private static bool ChangeWasValidOn(in Sudoku s, int x, int y)
    {
        // Check if there are no duplicates in the column of tile[x,y]
        Span<bool> found = stackalloc bool[10];
		for (var row = 0; row < 9; row++)
		{
			if (row == y) continue;
			var value = s.Tiles[x, row].Value;

			if (value == 0) continue;
			if (found[value]) return false;
			found[value] = true;
		}
		
        // Check if there are no duplicates in the row of tile[x,y]
        found = stackalloc bool[10];
		for (var column = 0; column < 9; column++)
		{
			if (column == x) continue;
			var value = s.Tiles[column, y].Value;
			
			if (value == 0) continue;
			if (found[value]) return false;
			found[value] = true;
		}
		
        // Check if there are no duplicates in the block of tile[x,y]
        found = stackalloc bool[10];
		int blockX = x - x % 3, // Coordinates of the top-left tile of the block of tile[x,y]
			blockY = y - y % 3;
		for (var by = 0; by < 3; by++) for(var bx = 0; bx < 3; bx++)
			if (bx != x || by != y) // Skip tiles already checked above (and tile[x,y])
			{
				var value = s.Tiles[blockX + bx, blockY + by].Value;
				
				if (value == 0) continue;
				if (found[value]) return false;
				found[value] = true;
			}

		return true;
    }

    /// <summary> Makes a given sudoku node-consistent </summary>
    public static void SetDomains(ref Sudoku sudoku)
    {
	    for (byte y = 0; y < 9; y++) for (byte x = 0; x < 9; x++)
			if (sudoku.Tiles[x, y].Value is not 0) // For every non-empty tile
				TryForwardCheck(sudoku, x, y, out sudoku); // Simplify neighbouring domains
				// ^ This will always succeed
    }
}
