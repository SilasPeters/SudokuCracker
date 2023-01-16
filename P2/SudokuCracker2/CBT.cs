namespace SudokuCracker2;

public class CBT
{

    
    public static bool Search(ref Sudoku sudoku, byte i = 0)
    {
        /*
         * Stel domeinen in, wat IntSets zijn
         *
         * if (i == 82) return true; ALL SET!
         * int x = i % 9, y = i / 9 of zo
         * 
         * for every value in domain tile[x,y]
         *  tile[x,y] = iterated value
         *  if (not CBT allows it) break;
         * 
         *  if (TryForwardCheck(sudoku, x, y, out var sdk))
         *      if (Search(sdk, i + 1)) return true;
         * 
         * tile[x,y] = null
         * return false
         */
    }

    private static bool TryForwardCheck(Sudoku sudoku, byte x, byte y, out Sudoku res)
    {
        /*
         * res = sudoku
         * 
         * Voor elke waarde met y=y XOR x=x uit res
         *  tile.domain.remove(tile[x,y])
         *  if (!tile.domain.Any()) return false;
         *
         * return true;
         */
    }
}