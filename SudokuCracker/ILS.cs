namespace SudokuCracker;

class ILS{ // ILS -> Iterated Local Search
    public Sudoku search(Sudoku s){
        //voer "Step" uit todat je een locale optima vind
        return s;
    }
    private Sudoku Step(Sudoku s){
        // 1. kies willekeurig 1 van de 9 blokken
        Random rnd = new Random();
        int randnum = rnd.Next(1, 9);
        
        // 2. probeer alle swaps
        // 3. kies de beste indien die een verbetering opleverd
        return s;
    }
}