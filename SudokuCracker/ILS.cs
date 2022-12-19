namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum = 5){
        int currenth = s.CalculateHeuristicValue();
        (Sudoku, int)current = (s, s.CalculateHeuristicValue()); //stored as (sudoku, heuristic value)
        (Sudoku, int)next = Step(s, false); 
        int platCount = 0;

        //voer "Step" uit todat je een locale optima vind
        while(current.Item2 > 26){  //ga door tot je deze heuristische waarde vind 
            if (next.Item2 == current.Item2) { //deze stap is evengoed als de laatste
                ++platCount;
                if(platCount > 9){//je zit echt op een plateau:
                    // Console.WriteLine("we've hit a plateau; stepping "+ platStepNum +"  times, currenth: " + current.Item2);
                    Console.Clear();
                    Console.Write(current.Item2);
                    for (int i = 0; i < platStepNum; i++) {
                        current = Step(current.Item1, true);  //ff stappen
                    }
                    platCount = 0;
                } 
            } else{  //deze stap is beter dan de laatste
                platCount = 0;
            }
            current = next;
            next = Step(s, false); 
        } 
        return current.Item1;
    }
    private static (Sudoku, int) Step(Sudoku s, bool forced){ 
        // 1. kies willekeurig 1 van de 9 blokken
        int h = s.CalculateHeuristicValue();
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,2); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,2); //y lowerbound van random block
        (int bh, int bx, int by, int bxs, int bys)best = (0,0,0,0,0);
        // 2. probeer alle swaps (tenzij je een fixed tile tegenkomt)
        List<(int, int, int, int, int)> swaps = new List<(int, int, int, int, int)>{}; //heuristic, x,y, xs,xy
        for (int y = ylow; y < ylow+3; y++) for (int x = xlow; x < xlow+3; x++){ 
            if (!s.IsFixed(x,y)){
                for (int ys = ylow; ys < ylow+3; ys++) for (int xs = xlow; xs < xlow+3; xs++){  //2 for loops om door alle swaps te iteraten
                    if (!s.IsFixed(xs, ys) && !(y == ys && x == xs)){ //we bekijken alleen een optie als we geen fixed values swappen, en we niet dezelfde values swappen
                        int newh = s.DetermineHeuristicChangeAfterSwap(x, y, xs, ys, h);
                        swaps.Add((newh, x, y, xs, ys));
                        if (forced && rnd.Next(1,5) == 1){  
                            //als we in forced modus zitten; pak niet de beste optie maar een willekeurige optie
                            //"willekeurig" in de zin van elke evaluatie is er een 1/5 kans dat je deze optie kiest.
                            best = (newh, x, y, xs, ys);
                        }
                    }
                }
            } 
        }
        // 3. kies de beste indien die een verbetering opleverd
        //als we in forced modus zitten, dan pakken we gewoon de random swap. tenzij die toevallig niks is.
        if (!forced || best == (0,0,0,0,0)) 
            best = swaps.OrderByDescending(x => x.Item1).Last(); //sorteer op heuristische waarde en pak eerste
        // Console.WriteLine($"Best: ({best.bh}, {best.bx}, {best.by}) -> ({best.bxs}, {best.bys})");
        if (best.bh < h || forced){
            s.Swap(best.bx, best.by, best.bxs, best.bys);
            return (s, best.bh); //als de beste swap beter is swap je. of als best geregeld is door forced.
        }
        return (s, h);
    }
}