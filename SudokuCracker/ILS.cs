namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s){
        //voer "Step" uit todat je een locale optima vind
        return s;
    }
    public static Sudoku Step(Sudoku s){ 
        // 1. kies willekeurig 1 van de 9 blokken
        int h = s.CalculateHeuristicValue();
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,2); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,2); //y lowerbound van random block

        // 2. probeer alle swaps (tenzij je een fixed tile tegenkomt)
        List<(int, int, int, int, int)> swaps = new List<(int, int, int, int, int)>{}; //heuristic, x,y, xs,xy
        for (int y = ylow; y < ylow+3; y++) for (int x = xlow; x < xlow+3; x++){ 
            if (!s.IsFixed(x,y)){
                for (int ys = ylow; ys < ylow+3; ys++) for (int xs = xlow; xs < xlow+3; xs++){ 
                    if (!s.IsFixed(xs, ys)){
                        int newh = s.DetermineHeuristicChangeAfterSwap(x, y, xs, ys, h);
                        swaps.Add((newh, x, y, xs, ys));
                    }
                }
            } 
        }
        // 3. kies de beste indien die een verbetering opleverd
        (int bh, int bx, int by, int bxs, int bys) best = swaps.OrderByDescending(x => x.Item1).Last(); //sort and grab first one
        Console.WriteLine($"Best: ({best.bh}, {best.bx}, {best.by}) -> ({best.bxs}, {best.bys})");
        if (best.bh < h){
            s.Swap(best.bx, best.by, best.bxs, best.bys);
            return s;
        }
        return s;
    }
}