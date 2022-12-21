namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum = 5){
        Console.WriteLine("Start: \n" + s);
        Console.WriteLine("Z: \n" + Zoekoperator(s, 0, 0, 1));
        // var best = s;
        throw new NotImplementedException();

    }
    private static (Sudoku, int) Step(Sudoku s, bool forced){ 
        throw new NotImplementedException();
    }

    private static Sudoku Zoekoperator(Sudoku s, int blockIndex, int tileIndexA, int tileIndexB){
        //Swap two tiles, they are not fixed
        Block block = s._blocks[blockIndex / 3, blockIndex % 3];
        var tiles = block.GetAllTilesEnumerable();
        Tile tileA = tiles.ElementAt(tileIndexA);
        Tile tileB = tiles.ElementAt(tileIndexB);
        var temp = tileA.Value;
        tileA.Value = tileB.Value;
        tileB.Value = temp;
        return s;
    }
}