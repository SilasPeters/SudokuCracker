//implement IClonable interface


namespace SudokuCracker2;

public class CBT
{
    public CBT(){
        History = new Sudoku[82];
    }
    public Sudoku[] History = new Sudoku[81];
    public Sudoku? TrySearch(Sudoku sdk, byte i = 0)
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
         History[i] = sdk;

        if (AllTilesFilled(ref sdk)) return sdk; //if sudoku filled, return it
        byte x = (byte)(i % 9), y = (byte)(i / 9);

        if (sdk.Tiles[x,y].IsFixed) return TrySearch(sdk, ++i); // Value is fixed, don't waste time on branching

        Console.WriteLine($"Domain of tile {x},{y} is of size {sdk.Tiles[x,y].Domain.Count}");


        List<byte> Domain = new List<byte>(History[i].Tiles[x,y].Domain); // Copy domain to prevent modification of original domain
	    foreach(byte s in Domain) { //go through entire (original) domain
            Sudoku sdk2 = (Sudoku) History[i].Clone();//copy sudoku from history to prevent modification of original sudoku
		    sdk2.Tiles[x,y].Value = s;
		    if (CBTAllowsIt(sdk2, x, y)) { //check if filling in the value doesn't break the rules
                Sudoku? sdkSimplified = TryForwardCheck(sdk2, x, y);
                if (sdkSimplified != null) { //if domain could be simplified (doesn't yield an empty domain somewhere)
                    Sudoku? attempt = TrySearch((Sudoku) sdkSimplified!, ++i); //try to solve the simplified sudoku
                    Console.WriteLine($"Attempt at {x},{y} with {s} returned {attempt != null}");
                    if (attempt != null) return attempt; //if solved, return the (partially) solved sudoku
                    Console.WriteLine($"domain at {x},{y} is now size {sdk2.Tiles[x,y].Domain.Count}");
                }
            } else {
                Console.WriteLine("CBT doesn't allow it");  //if this value doesn't work, try the next one
            }
	    }
        Console.WriteLine("\n");
        //print domain
        Console.Write("done with domain: ");
        foreach (byte s in Domain) Console.Write($"{s} ");
        Console.Write($"at pos {x},{y} \n");
		return null;
    }

    /// <summary>
    /// After setting a tile, the domains should be culled
    /// This check removes the value of tile[<paramref name="x"/>,<paramref name="y"/>] from the domain of every tile in the block, row and column containing
    /// tile[<paramref name="x"/>,<paramref name="y"/>]. If a domain turns out to be empty afterwards, it stops and returns false.
    /// </summary>
    /// <param name="sudoku"> A copy of the sudoku to mutate. </param>
    /// <param name="x"> The x-coordinate of the tile which was set before this check. </param>
    /// <param name="y"> The y-coordinate of the tile which was set before this check. </param>
    /// <param name="result"> The sudoku with simplified constraints/domains. Only partially simplified if the check fails. </param>
    /// <returns> Whether the forward check did succeed, and thus if all domains where simplified to a non-empty set. </returns>
     private static Sudoku? TryForwardCheck(Sudoku sudoku, byte x, byte y)
    {
		Sudoku result = sudoku;
		var valueOfSetTile = (byte) result.Tiles[x, y].Value!;

		// Process the row containing the tile that was set
		for (var xi = 0; xi < 9; xi++)
		{
        if (x == 1 && y == 0 && valueOfSetTile == 8){

        }
			if (xi != x)
				result.Tiles[xi, y].Domain.Remove(valueOfSetTile); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			if (!result.Tiles[xi, y].Domain.Any())
				return null;
		}

		// Process the column containing the tile that was set
		for (var yi = 0; yi < 9; yi++)
		{
			if (yi != y)
				result.Tiles[x, yi].Domain.Remove(valueOfSetTile); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			if (!result.Tiles[x, yi].Domain.Any())
				return null;
		}

		// Process the remains of the block containing the tile that was set
		int blockX = x - x % 3,
			blockY = y - y % 3;
		for (var by = blockY; by < blockY + 3; by++) for (var bx = blockX; bx < blockX + 3; bx++)
		{
			if (bx != x && by != y) // The tile is not on a previously mutated row/column
				result.Tiles[bx, by].Domain.Remove(valueOfSetTile); // Sometimes the contents domain will be empty if the tile is fixed, but that's fine
			if (!result.Tiles[bx, by].Domain.Any())
				return null;
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
    public static Sudoku SetDomains(ref Sudoku sudoku)
    {
        Sudoku result = sudoku;
	    for (byte y = 0; y < 9; y++)
		    for (byte x = 0; x < 9; x++)
			    if (sudoku.Tiles[x, y].Value is not null) // For every non-empty tile
				    result = (Sudoku)TryForwardCheck(sudoku, x, y); // Simplify neighbouring domains

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
