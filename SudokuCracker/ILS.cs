namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum) {
        int kaas = 1000;
        
        int platCount = 0;
        while (s.H > 0){      //stopconditie (h == 0)
            if (step(ref s)){ //klein plateau
                ++platCount;
                if (platCount >= 10){ //groot (genoeg) plateau
                    for (int i = 0; i < platStepNum; i++){
                        plateauStep(ref s); //S keer willekeurig swappen
                    }
                    platCount = 0;
                }
            } else platCount = 0; //geen plateau (meer)
            if (s.H < kaas) {
                kaas = s.H;
                Console.WriteLine(kaas);
            }
        }

        return s;
    }
    /// <remarks>Resets the heuristic value of the sudoku to -1</remarks>
    private static Sudoku plateauStep(ref Sudoku s) {
        // Get random block
        var randomBlockIndex = Random.Shared.Next(0, 9);
        var randomBlock         = s.GetBlock((byte) randomBlockIndex);
        // Get a non-fixed random tile pair to be swapped
        var randomTileIndex1 = Random.Shared.Next(0, 9);
        var randomTileIndex2 = Random.Shared.Next(0, 9);

        if (randomTileIndex1 == randomTileIndex2)
            return plateauStep(ref s); // Retry

        // Translate index of a tile to coordinate of the BLOCK
        int blockX = randomBlockIndex % 3 * 3,
            blockY = randomBlockIndex / 3 * 3;
        int ax = randomTileIndex1 % 3, ay = randomTileIndex1 / 3,
            bx = randomTileIndex2 % 3, by = randomTileIndex2 / 3;
        
        // Don't swap a fixed tile
        if (randomBlock.IsFixed(ax, ay) 
         || randomBlock.IsFixed(bx, by))
            return plateauStep(ref s); // Retry TODO: causes infinite recursion if a block is completely fixed
        
        // At this point, we found a valid swap. Swap!
        // To do this properly, we need to translate the Block coordinates to Sudoku coordinates
        ax += blockX; ay += blockY;
        bx += blockX; by += blockY;
        
        s.Swap(ax, ay, bx, by, -1); // We did not determine the new heuristic value, thus reset it
        return s;
    }
    
    /// <summary>
    /// Picks a random block and finds and performs the best swap possible in tha block, based on the heuristic value
    /// </summary>
    /// <param name="s"></param>
    /// <returns>Whether a swap was performed</returns>
    private static bool step(ref Sudoku s)
    {
        // Prepare data
        var bestH            = s.H;
        var bestSwap         = Array.Empty<int>();
        var randomBlockIndex = Random.Shared.Next(0, 9);
        var randomBlock      = s.GetBlock((byte) randomBlockIndex);
        int blockX = randomBlockIndex % 3 * 3,
            blockY = randomBlockIndex / 3 * 3;
		
        // Try all swaps
        for (var i = 0; i < 8; i++) for (var j = i + 1; j < 9; j++) // Note that j starts at i + 1, to prevent duplicate tests
        {
            // Translate index of a tile to coordinate of the BLOCK
            //     [0..8] -> (x, y)
            //     Example: 4 -> (1, 1)
            int ax = i % 3, ay = i / 3,
                bx = j % 3, by = j / 3;
            
            if (randomBlock.IsFixed(ax, ay) || randomBlock.IsFixed(bx, by))
                continue; // Don't try to swap a fixed value
            
            // Convert coordinates within block to coordinates within SUDOKU
            ax += blockX; ay += blockY;
            bx += blockX; by += blockY;

            // Try the swap, store it if is the best one yet
            var h = s.DetermineHeuristicChangeAfterSwap(ax, ay, bx, by, s.H);
            if (h < bestH)
            {
                bestH    = h;
                bestSwap = new [] { ax, ay, bx, by };
            }
        }
        
        // Apply best swap, if existing
        if (!bestSwap.Any())
            return false; // Indicate that no swap was made
        s.Swap(bestSwap[0], bestSwap[1], bestSwap[2], bestSwap[3], bestH);
        return true; // Indicate that a swap was made
    }
}