namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
	public static Sudoku search(Sudoku s, int platStepNum) {
		int kaas = 1000;
        
		int platCount = 0;
		while (s.H > 0){      //stopconditie (h == 0)
			if (Step(ref s)){ //klein plateau
				++platCount;
				if (platCount >= 10){ //groot (genoeg) plateau
					for (int i = 0; i < platStepNum; i++){
						plateauStep(s); //S keer willekeurig swappen
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
	private static (Sudoku, int) plateauStep(Sudoku s) { //willekeurige swaps om uit een plateau te ontsnappen
		Random rnd  = new Random();
		int    xlow = 3* rnd.Next(0, 3); //x lowerbound van random block
		int    ylow = 3* rnd.Next(0, 3); //y lowerbound van random block
		(int x, int y)   = (rnd.Next(0, 3)+xlow, rnd.Next(0, 3)+ylow);
		(int xs, int ys) = (rnd.Next(0, 3)+xlow, rnd.Next(0, 3)+ylow);
		if ((x,y) == (xs, ys) || s.IsFixed(x, y) || s.IsFixed(xs, ys)){ //als de random swap niks doet of niet kan
			return plateauStep(s);                                      //nog ene keer
		}
		s.Swap(x, y, xs, ys, -1); // Note: resets h as we did not calculate what it will be
		int h = s.CalculateHeuristicValue();
		s.H = h;
		return (s, h);
	}
    
	/// <summary>
	/// Picks a random block and finds and performs the best swap possible in tha block, based on the heuristic value
	/// </summary>
	/// <param name="s"></param>
	/// <returns>Whether a swap was performed</returns>
	private static bool Step(ref Sudoku s)
	{
		// Prepare data
		var bestH            = s.H;
		var bestSwap         = Array.Empty<int>();
		var randomBlockIndex = Random.Shared.Next(0, 9);
		var randomBlock      = s.GetBlock((byte) randomBlockIndex);
		int blockX = randomBlockIndex % 3 * 3,
			blockY = randomBlockIndex / 3 * 3;
		
		// Try all swaps
		for (var i = 0; i < 9; i++) for (var j = i + 1; j < 9; j++) // Note that j starts at i + 1, to prevent duplicate tests
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