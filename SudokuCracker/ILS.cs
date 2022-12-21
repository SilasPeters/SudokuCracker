namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum){
        int currentH = s.CalculateHeuristicValue();
        (Sudoku, int)current = (s, currentH); //stored as (sudoku, heuristic value)
        (Sudoku, int)next; 
        int platCount = 0;
        while (current.Item2 > 0){ //stopconditie (h == 0)
            next = Step(current.Item1, current.Item2);
            Console.WriteLine($"step: {current.Item2} -> {next.Item2}");
            if (current.Item2 == next.Item2){ //klein plateau
                ++platCount;
                if (platCount >= 10){  //groot (genoeg) plateau
                    Console.Write($"plateau!: {next.Item2}");
                    for (int i = 0; i < platStepNum; i++){
                        current = next;
                        next = plateauStep(current.Item1); //S keer willekeurig swappen
                        Console.Write($"-> {next.Item2}");
                    }
                    Console.Write("\n");
                    platCount = 0;
                }
            } else platCount = 0; //geen plateau (meer)
            current = next; //daadwerkelijk de stap uitvoeren
        }
        return current.Item1;
    }
    private static (Sudoku, int) plateauStep(Sudoku s){ //willekeurige swaps om uit een plateau te ontsnappen
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,2); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,2); //y lowerbound van random block
        (int x, int y) = (rnd.Next(0,2)+xlow, rnd.Next(0,2)+ylow);
        (int xs, int ys) = (rnd.Next(0,2)+xlow, rnd.Next(0,2)+ylow);
        if ((x,y) == (xs, ys) || s.IsFixed(x,y) || s.IsFixed(xs,ys)){ //als de random swap niks doet of niet kan
            return plateauStep(s);
        }
        s.Swap(x, y, xs, ys);
        int h = s.CalculateHeuristicValue();
        return (s, h);
    }
    private static (Sudoku, int) Step(Sudoku s, int oldH){
        // 1. kies willekeurig 1 van de 9 blokken
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,2); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,2); //y lowerbound van random block
        (int bh, int bx, int by, int bxs, int bys) best = (oldH,0,0,0,0);
        // 2. probeer alle swaps binnen block (die mogelijk zijn en niet niks doen)
        for (int y = ylow; y < ylow+3; y++) for (int x = xlow; x < xlow+3; x++){ 
            if (!s.IsFixed(x,y)){
                for (int ys = ylow; ys < ylow+3; ys++) for (int xs = xlow; xs < xlow+3; xs++){  //alle swaps bekijken
                    if (!s.IsFixed(xs, ys) && !(y == ys && x == xs)){ //niet fixed en swap doet niet niks
                        int newH = s.DetermineHeuristicChangeAfterSwap(x, y, xs, ys, oldH);
                        if (newH < best.bh){
                            best = (newH, x, y, xs, ys);  //beste swap tot nu toe > best
                        }
                    }
                }
            } 
        }
        // 3. kies de beste indien die een verbetering opleverd
        // if (best.bx == -1) { //als we geen enkele mogelijke swap hebben
        //     return Step(s, oldH); //probeer opnieuw voor een ander block
        // } =====> Dit moet niet nodig zijn, misschien?

        string better = "";
        if (best.bh < oldH) better = "<<<<<<< ";
        Console.WriteLine($"Best: ({best.bx}, {best.by}) -> ({best.bxs}, {best.bys}), h change = {best.bh-oldH}" + better);

        if (best.bh < oldH) {
            s.Swap(best.bx, best.by, best.bxs, best.bys);
        }
        return (s, best.bh); //als de beste swap beter is swap je. 
    }
}