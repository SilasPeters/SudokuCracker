namespace SudokuCracker;

static class ILS{ // ILS -> Iterated Local Search
    public static Sudoku search(Sudoku s, int platStepNum){
        int kaas = 1000; //debug
        int currentH = s.CalculateHeuristicValue();
        int previousH;
        int platCount = 0;
        while (currentH > 0){ //stopconditie (h == 0)
            previousH = currentH;
            s = Step(s, currentH);
            currentH = s.CalculateHeuristicValue();
            if (currentH == previousH){ //klein plateau
                ++platCount;
                if (platCount >= 10){  //groot (genoeg) plateau
                    for (int i = 0; i < platStepNum; ++i){
                        s = plateauStep(s); //S keer willekeurig swappen
                    } 
                    platCount = 0;
                    currentH = s.CalculateHeuristicValue();
                }
            } else platCount = 0; //geen plateau (meer)
            if (currentH < kaas){ kaas = currentH; Console.WriteLine(kaas); } //debug
        }
        return s;
    }
    private static Sudoku plateauStep(Sudoku s){ //willekeurige swaps om uit een plateau te ontsnappen
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,3); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,3); //y lowerbound van random block
        (int x, int y) = (rnd.Next(0,3)+xlow, rnd.Next(0,3)+ylow);
        (int xs, int ys) = (rnd.Next(0,3)+xlow, rnd.Next(0,3)+ylow);
        if ((x,y) == (xs, ys) || s.IsFixed(x,y) || s.IsFixed(xs,ys)){ //als de random swap niks doet of niet kan
            return plateauStep(s); //nog ene keer
        }
        s.Swap(x, y, xs, ys);
        return s;
    }
    private static Sudoku Step(Sudoku s, int oldH){
        // 1. kies willekeurig 1 van de 9 blokken
        Random rnd = new Random();
        int xlow = 3* rnd.Next(0,3); //x lowerbound van random block
        int ylow = 3* rnd.Next(0,3); //y lowerbound van random block
        (int bh, int bx, int by, int bxs, int bys) best = (oldH,0,0,0,0);
        // 2. probeer alle swaps binnen block (die mogelijk zijn en niet niks doen)
        for (int y = ylow; y < ylow+3; y++) for (int x = xlow; x < xlow+3; x++){ 
            if (!s.IsFixed(x,y)){
                for (int ys = ylow; ys < ylow+3; ys++) for (int xs = xlow; xs < xlow+3; xs++){  //alle swaps bekijken
                    if (!s.IsFixed(xs, ys) && (x,y) != (xs,ys)){ //niet fixed en swap doet niet niks
                        int newH = s.DetermineHeuristicChangeAfterSwap(x, y, xs, ys, oldH);
                        if (newH < best.bh){
                            best = (newH, x, y, xs, ys);  //beste swap tot nu toe > best
                        }
                    }
                }
            } 
        }
        // 3. kies de beste indien die een verbetering opleverd
        // Console.WriteLine($"Best: ({best.bx}, {best.by}) -> ({best.bxs}, {best.bys}), h change = {best.bh-oldH}");
        if (oldH > best.bh) {
            s.Swap(best.bx, best.by, best.bxs, best.bys);
        }
        return s; //als de beste swap beter is swap je. 
    }
}