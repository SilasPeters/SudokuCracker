namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum = 5){ 
        Console.WriteLine("Start: " + s.CalculateHeuristicValue());
        Console.WriteLine(s);
        var z = Zoekoperator(s, 0, 0, 1);
        Console.WriteLine("Z: " + z.CalculateHeuristicValue());
        Console.WriteLine(z);
        Console.WriteLine(Zoekoperator(s, 0, 0, 1));
        
        // Local search
        // chose a random block
        Random rnd = new Random();
        int blockIndex = 1;//rnd.Next(0, 9);
        Block block = s._blocks[blockIndex / 3, blockIndex % 3];
        // make a list of all tiles able to be swapped
        var tileList = block.GetAllTilesEnumerable().Where(tile => !tile.IsFixed).ToList();
        // generate a list of sudoku's from all possible swaps
        List<byte> hList = new();
        for (int i = 0; i < tileList.Count; i++){
            for (int j = 0; j < tileList.Count; j++){
                if (j <= i ) continue; // checks all combi's only once {[(OPTIMIZATION)]}
                Console.WriteLine(i + " " + j);
                //Console.WriteLine(Zoekoperator(s, blockIndex, i, j));
            }
        }
        // // choose the sudoku with the lowest heuristic value
        // foreach (Sudoku sudoku in sudokuList){
        //     Console.WriteLine(sudoku.CalculateHeuristicValue());
        //     Console.WriteLine(sudoku);
        // }
        // s = sudokuList.OrderBy(sudoku => sudoku.CalculateHeuristicValue()).Last();
        // System.Console.WriteLine("step: \n" + s.CalculateHeuristicValue());
        // Console.WriteLine(s);
        return s;
        
        //generate a list of sudoku's from all possible swaps
        

    }

    private static Sudoku Zoekoperator(Sudoku s, int blockIndex, int tileIndexA, int tileIndexB){
        //Make a copy of the sudoku
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