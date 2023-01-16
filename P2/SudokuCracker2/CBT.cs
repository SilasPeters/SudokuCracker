namespace SudokuCracker2;

public class CBT
{

    
    public static bool Search(ref Sudoku sudoku, byte i = 0)
    {
        /*
         * SetDomains(ref sudoku);
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
         *  tile.domain.remove(tile[x,y]) sometimes the domain will be empty if the tile is fixed, but thats fine
         *  if (!tile.domain.Any()) return false;
         *
         * return true;
         */
    }

    /// <summary> Checks if the given sudoku still is a partial answer after setting tile[x,y] </summary>
    private static bool CBTAllowsIt(ref Sudoku sudoku, byte x, byte y)
    {
        /*
         * De code hieronder kan veel code hergebruiken van Block.txt
         * Een sudoku wordt vanaf nu een 9x9 array aan Tiles
         * Een Tile is niks anders dan een byte (waarde) en een ByteSet (domein)
         * 
         * Voor elke waarde in rij y
         *  Check of er geen duplicates in zitten
         * Voor elke waarde in kolom x
         *  Check of er geen duplicates in zitten
         * Voor elke waarde in block van [x,y]
         *  Check of er geen duplicates in zitten
         */
    }

    /// <summary> Maakt alles knoop-consistent </summary>
    private static void SetDomains(ref Sudoku sudoku)
    {
        /*
         * For every tile that is empty
         *  tile.Domain = new { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
         * For every tile[x,y] that is NOT empty
         *  TryForwardCheck(sudoku, x, y, out sudoku) this will/should always succeed
         *
         * Ik heb geen idee of alles nu knoop-consistent is.
         * Een toevoeging zou dit zijn: Indien je singletons hebt in een domein van een tegel,
         * moeten alle buren die singelton dus niet in hun domein hebben. Je kunt ook meteen de tegel met een singelton-
         * domein zijn waarde geven (de singelton). Dit allemaal kan in één zweepslag meegenomen worden in de bestaande
         * ForwardCheck() methode, die dan PropagatedForwardCheck() kan heten. Dit kan alles sneller maken door onnodige
         * branching te voorkomen. Het is niet vereist, dus leg ik het hier uit, maar misschien toch weer wel nodig voor
         * hier, dus leg ik het hier uit :}. Ik heb t al uitgepluisd in mijn hoofd, dus vraag er vooral naar.
         */
    }
}