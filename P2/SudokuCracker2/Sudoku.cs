using System.Diagnostics;
using System.Text;

namespace SudokuCracker2;

public readonly struct Sudoku 
{
	public Sudoku Clone() {
		Tile[,] tarray = new Tile[9,9];
		for (byte i = 0; i < 81; ++i)
		{
			tarray[i % 9, i / 9] = this.Tiles[i % 9, i / 9].Clone();
		}
		Sudoku s = new Sudoku(tarray);
		return s;
	}
	public Sudoku (Tile[,] tiles){
		Tiles = tiles;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public readonly Tile[,] Tiles = new Tile[9, 9];

	public Sudoku (IReadOnlyList<byte> numbers)
	{
		for (var i = 0; i < 81; i++)
		{
			Tiles[i % 9, i / 9] = numbers[i] != 0
				? new Tile(numbers[i], true,  new[] { numbers[i] })
				: new Tile(null,  false, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		}
	}


	/// <param name="column">The x-index of every tile mapped over</param>
	/// <param name="f">A method that accepts a tile and the y-coordinate of the tile</param>
	public void MapOverColumn_(byte column, Action<Tile, byte> f)
	{
		for (byte row = 0; row < 9; row++)
			f(Tiles[column, row], row);
	}

	public override string ToString() // This is where the ASCII magic happens, which is hard to explain
	{
		StringBuilder sb = new();
		const int totalWidth = (9 * 2 - 1) + 4; // 9 numbers with spaces, minus the last space, plus 2 separating bars with 2 spaces

		for (var y = 0; y < 9; y++)
		{
			for (var x = 0; x < 9; x++)
				sb.Append($"{Tiles[x,y]} ");

			insertVertically("│ ");
			
			if (y != 8 && (y + 1) % 3 == 0) // Add horizontal pipes
			{
				sb.AppendLine();
				sb.Append('─', totalWidth - 3);
				insertVertically("┼─");
			}
			
			sb.AppendLine();

			void insertVertically(string separator)
			{
				sb.Insert(sb.Length - 6 * 2, separator);
				sb.Insert(sb.Length - 3 * 2, separator);
			}
		}
		
		return sb.ToString();
	}
}
