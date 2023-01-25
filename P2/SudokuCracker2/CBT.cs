namespace SudokuCracker2;

public class CBT
{
    public static bool TrySearch(Sudoku sdk, out Sudoku result, byte i = 0)
    {
        /*
         * Assumes that all domains are set
         * 
		 * if (sudoku.AllTilesFilled()) return true;
         * 
         * int x = i % 9, y = i / 9 of zo
         * 
         * for every value in tile[x,y].Domain
         *  tile[x,y].Value = iterated value
         *  if (!CBTAllowsIt()) break;
         * 
         *  if (TryForwardCheck(sudoku, x, y, out var sdk))
         *      if (TrySearch(sdk, i + 1)) return true;
         * 
         * tile[x,y] = null
         * return false
         */


	    result = sdk;
        if (sdk.AllTilesFilled())
	        return true;
        byte x = (byte)(i % 9), y = (byte)(i / 9);
        
        if (sdk.Tiles[x,y].IsFixed) // Value is fixed, don't waste time on branching
	        return TrySearch(sdk, out result, ++i);
        
	    foreach(var s in sdk.Tiles[x,y].Domain) {
		    sdk.Tiles[x,y].Value = byte.Parse(s.ToString());
		    if (!CBTAllowsIt(ref sdk, x, y)) continue;
		    if (TryForwardCheck(sdk, x, y, out var sdkSimplified)) {
			    if (TrySearch(sdkSimplified, out var attempt, ++i)) {
				    result = attempt;
				    return true;
			    }
		    }
	    }
	    
		return false;
    }

    /// <summary>
    /// After setting a tile, the constraints should be simplified by performing this forward check.
    /// This check removes the value of tile[<paramref name="x"/>,<paramref name="y"/>] from the domain of every tile in the block, row and column containing
    /// tile[<paramref name="x"/>,<paramref name="y"/>]. If a domain turns out to be empty afterwards, it stops.
    /// </summary>
    /// <param name="sudoku"> A copy of the sudoku to mutate. </param>
    /// <param name="x"> The x-coordinate of the tile which was set before this check. </param>
    /// <param name="y"> The y-coordinate of the tile which was set before this check. </param>
    /// <param name="result"> The sudoku with simplified constraints/domains. Only partially simplified if the check fails. </param>
    /// <returns> Whether the forward check did succeed, and thus if all domains where simplified to a non-empty set. </returns>
    private static bool TryForwardCheck(Sudoku sudoku, byte x, byte y, out Sudoku result)
    {
		result = sudoku;
		var valueOfSetTile = (byte) result.Tiles[x, y].Value!;

		// Process the row containing the tile that was set
		for (var column = 0; column < 9; column++)
		{
			if (column == x) continue;
			ref var tile = ref result.Tiles[column, y];
				
			var indexOfCurrentValue = tile.Domain.IndexOf(valueOfSetTile.ToString(), StringComparison.Ordinal);
			if (indexOfCurrentValue != -1)
				tile.Domain = tile.Domain.Remove(indexOfCurrentValue, 1); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			
			if (!tile.Domain.Any())
				return false;
		}

		// Process the column containing the tile that was set
		for (var row = 0; row < 9; row++)
		{
			if (row == y) continue;
			ref var tile = ref result.Tiles[x, row];
				
			var indexOfCurrentValue = tile.Domain.IndexOf(valueOfSetTile.ToString(), StringComparison.Ordinal);
			if (indexOfCurrentValue != -1)
				tile.Domain = tile.Domain.Remove(indexOfCurrentValue, 1); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
				
			if (!tile.Domain.Any())
				return false;
		}
		
		// Process the remains of the block containing the tile that was set
		int blockX = x - x % 3,
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx == x || by == y) continue; // The tile is not on a previously mutated row/column
			ref var tile = ref result.Tiles[bx, by];
			
			var indexOfCurrentValue = tile.Domain.IndexOf(valueOfSetTile.ToString(), StringComparison.Ordinal);
			if (indexOfCurrentValue != -1)
				tile.Domain = tile.Domain.Remove(indexOfCurrentValue, 1); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			
			if (!tile.Domain.Any())
				return false;
		}
		
		return true;
    }

    /// <summary> Checks if the given sudoku still is a partial answer after setting tile[x,y] </summary>
    private static bool CBTAllowsIt(ref Sudoku sdk, byte x, byte y)
    {
        /*
         * De code hieronder kan veel code hergebruiken van Block.txt en de methode herboven
         * Een sudoku wordt vanaf nu een 9x9 array aan Tiles
         * Een Tile is niks anders dan een byte (waarde) en een ByteSet (domein)
         * 
         * Voor elke waarde in rij y
         *  Check of er geen duplicates in zitten
         * Voor elke waarde in kolom x
         *  Check of er geen duplicates in zitten
         * Voor elke waarde in block van [x,y]
         *  Check of er geen duplicates in zitten
         */
        var found = new bool[10];
        for (byte xi = 0; xi < 9; ++xi){
            var num =sdk.Tiles[xi,y].Value;
            if (num == null) continue;
            if (found[(byte)num]) return false;
            found[(byte)num] = true;
        }
        found = new bool[10];
        for (byte yi = 0; yi < 9; ++yi){
            var num =sdk.Tiles[x,yi].Value;
            if (num == null) continue;
            if (found[(byte)num]) return false;
            found[(byte)num] = true;
        }
        found = new bool[10];
        byte BlockX = (byte)(x - x % 3), BlockY = (byte)(y - y % 3);
        for (byte xi = BlockX; xi < BlockX+3; ++xi){
            for (byte yi = BlockY; yi < BlockY+3; ++yi){
                var num =sdk.Tiles[xi,yi].Value;
                if (num == null) continue;
                if (found[(byte)num]) return false;
                found[(byte)num] = true;
            }
        }
        return true;
    }

    /// <summary> Maakt alles knoop-consistent </summary>
    public static void SetDomains(ref Sudoku sudoku)
    {
	    for (byte y = 0; y < 9; y++)
		    for (byte x = 0; x < 9; x++)
			    if (sudoku.Tiles[x, y].Value is not null) // For every non-empty tile
				    TryForwardCheck(sudoku, x, y, out sudoku); // Simplify neighbouring domains
					// ^ This will/should always succeed
					
	    /*
         * Ik heb geen idee of alles nu knoop-consistent is.
         * Een toevoeging zou dit zijn: Indien je singletons hebt in een domein van een tegel,
         * moeten alle buren die singelton dus niet in hun domein hebben. Je kunt ook meteen de tegel met een singelton-
         * domein zijn waarde geven (de singelton). Dit allemaal kan in één zweepslag meegenomen worden in de bestaande
         * ForwardCheck() methode, die dan PropagatedForwardCheck() kan heten. Dit kan alles sneller maken door onnodige
         * branching te voorkomen. Het is niet vereist, dus leg ik het hier uit, maar misschien toch weer wel nodig voor
         * hier, dus leg ik het hier uit :}. Ik heb t al uitgepluisd in mijn hoofd, dus vraag er vooral naar.
         */
    }
}
