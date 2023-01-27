//implement IClonable interface


namespace SudokuCracker2;

public class CBT
{
    public CBT(){
        History = new Sudoku[81];
    }
    public Sudoku[] History = new Sudoku[81];
    public Sudoku? TrySearch(Sudoku sdk, byte i = 0)
    {
        this.History[i] = sdk.Clone();

        if (AllTilesFilled(ref sdk)) return sdk; //if sudoku filled, return it
        byte x = (byte)(i % 9), y = (byte)(i / 9);

        if (sdk.Tiles[x,y].IsFixed) return TrySearch(sdk, (byte)((int)i+1)); // Value is fixed, don't waste time on branching

        List<byte> Domain = new List<byte>(History[i].Tiles[x,y].Domain); // Copy domain to prevent modification of original domain
	    foreach(byte s in Domain) { //go through entire (original) domain
            Sudoku sdk2 = (Sudoku) History[i].Clone();//copy sudoku from history to prevent modification of original sudoku
		    sdk2.Tiles[x,y].Value = s;
		    if (CBTAllowsIt(sdk2, x, y)) { //check if filling in the value doesn't break the rules
                Sudoku? sdkSimplified = TryForwardCheck(sdk2, x, y);
                if (sdkSimplified != null) { //if domain could be simplified (doesn't yield an empty domain somewhere)
                    Sudoku? attempt = TrySearch((Sudoku) sdkSimplified!, (byte)((int)i+1)); //try to solve the simplified sudoku
                    if (attempt != null) return attempt; //if solved, return the (partially) solved sudoku
                }
            } 
	    }
		return null;
    }

    /// After setting a tile, the domains should be culled
     private Sudoku? TryForwardCheck(Sudoku sudoku, byte x, byte y)
    {
		Sudoku result = sudoku;
		var valueOfSetTile = (byte) result.Tiles[x, y].Value!;

		// Process the row containing the tile that was set
		for (var xi = 0; xi < 9; xi++)
		{
			if (xi != x) result.Tiles[xi, y].Domain.Remove(valueOfSetTile); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			if (result.Tiles[xi, y].Domain.Count() <= 0) return null; //if setting this tile to this value causes some other value to have no possibilities, stop.
		}

		// Process the column containing the tile that was set
		for (var yi = 0; yi < 9; yi++)
		{
			if (yi != y) result.Tiles[x, yi].Domain.Remove(valueOfSetTile); 
			if (result.Tiles[x, yi].Domain.Count() <= 0) return null;
		}

		// Process the remains of the block containing the tile that was set
		int blockX = x - x % 3,
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx != x && by != y){// The tile is not on a previously mutated row/column
				result.Tiles[bx, by].Domain.Remove(valueOfSetTile); 
            } 
			if (result.Tiles[bx, by].Domain.Count() <= 0) return null;
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
				    result = (Sudoku)TryForwardCheck(result, x, y); // Simplify neighbouring domains

        return result; //This will/should always succeed
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
