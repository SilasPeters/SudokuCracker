namespace SudokuCracker2;


public class CBT
{
	private const bool UseTfc = true; //whether or not you want to use TryForwardCheck (used for testing)
    
    public CBT(){
        History = new Sudoku[82];
    }
    public readonly Sudoku[] History;
    public Sudoku? TrySearch(Sudoku sdk, byte i = 0)
    {
        this.History[i] = sdk.Clone();

        if (AllTilesFilled(ref sdk)) return sdk; //if sudoku filled, return it
        byte x = (byte)(i % 9), y = (byte)(i / 9);

        if (sdk.Tiles[x,y].IsFixed) return TrySearch(sdk, (byte)(i+1)); // Value is fixed, don't waste time on branching

        var Domain = new List<byte>(History[i].Tiles[x,y].Domain); // Copy domain to prevent modification of original domain
	    foreach(byte s in Domain) { 
            Sudoku sdk2 = History[i].Clone(); //copy from history to prevent cross-contamination between attempts
		    sdk2.Tiles[x,y].Value = s;
		    if (CBTAllowsIt(sdk2, x, y)) { //check if filling in the value doesn't break the rules
                if (UseTfc){
                    Sudoku? sdkSimplified = TryForwardCheck(sdk2, x, y);
                    if (sdkSimplified != null) { //if domain could be simplified (doesn't yield an empty domain somewhere)
                        Sudoku? attempt = TrySearch((Sudoku) sdkSimplified!, (byte)(i+1)); //try to solve the simplified sudoku
                        if (attempt != null) return attempt; //if solved, return the (partially) solved sudoku
                    }
                }
		    } 
	    }
		return null;
    }

    /// <summary>
    /// After setting a tile, the domains should be culled
    /// </summary>
     private Sudoku? TryForwardCheck(Sudoku sudoku, byte x, byte y)
    {
		Sudoku result = sudoku;
		var valueOfSetTile = (byte) result.Tiles[x, y].Value!;

		// Process the row containing the tile that was set
		for (var xi = 0; xi < 9; xi++)
		{
			if (xi != x) result.Tiles[xi, y].Domain.Remove(valueOfSetTile); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			if (!result.Tiles[xi, y].Domain.Any()) return null; //if setting this tile to this value causes some other value to have no possibilities, stop.
		}

		// Process the column containing the tile that was set
		for (var yi = 0; yi < 9; yi++)
		{
			if (yi != y) result.Tiles[x, yi].Domain.Remove(valueOfSetTile); 
			if (!result.Tiles[x, yi].Domain.Any()) return null;
		}

		// Process the remains of the block containing the tile that was set
		int blockX = x - x % 3,
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx != x && by != y){// The tile is not on a previously mutated row/column
				result.Tiles[bx, by].Domain.Remove(valueOfSetTile); 
            } 
			if (!result.Tiles[bx, by].Domain.Any()) return null;
		}

		return result;
    }

    /// <summary> Checks if the given sudoku still is a partial answer after setting tile[x,y] </summary>
    private static bool CBTAllowsIt(Sudoku sdk, byte x, byte y)
    {
        var found = new bool[10]; // een array aan bools houdt bij of een nummer al gevonden is
        for (byte xi = 0; xi < 9; ++xi){ //check rij
            var num =sdk.Tiles[xi,y].Value; //pak value
            if (num != null){
                if (found[(byte)num]) return false; //als value al gevonden is klopt de sudoku niet
                found[(byte)num] = true; //zet de gevonden value op true
            }
        }
        found = new bool[10]; //check kolom
        for (byte yi = 0; yi < 9; ++yi){
            var num =sdk.Tiles[x,yi].Value;
            if (num != null) {
                if (found[(byte)num]) return false;
                found[(byte)num] = true;
            }
        }
        found = new bool[10]; //check block
        byte BlockX = (byte)(x - x % 3), BlockY = (byte)(y - y % 3);
        for (byte xi = BlockX; xi < BlockX+3; ++xi){
            for (byte yi = BlockY; yi < BlockY+3; ++yi){
                var num =sdk.Tiles[xi,yi].Value;
                if (num != null){
                    if (found[(byte)num]) return false;
                    found[(byte)num] = true;
                }
            }
        }
        return true;
    }

    /// <summary> Maakt alles knoop-consistent </summary>
    public Sudoku SetDomains(ref Sudoku sudoku)
    {
        Sudoku result = sudoku;
	    for (byte y = 0; y < 9; y++)
		    for (byte x = 0; x < 9; x++)
			    if (sudoku.Tiles[x, y].Value is not null) // For every non-empty tile
				    result = (Sudoku)TryForwardCheck(result, x, y)!; // Simplify neighbouring domains

        return result;
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

	public static bool AllTilesFilled(ref Sudoku sdk)
	{
		// Start looking from the lower-right side of the array, to see if a tile has not been set yet.
		// This assumes that tiles are set starting from the top-left in reading direction.
		for (byte y = 0; y <= 8; y++){
            for (byte x = 0; x <= 8; x++){
                if (sdk.Tiles[x, y].Value is null) return false;
            }
        }
		return true;
	}

}
