using System.Diagnostics;
using System.Text;

namespace SudokuCracker;

public class Sudoku
{
	public Sudoku (IReadOnlyList<byte> numbers)
	{
		for (var blockY = 0; blockY < 3; blockY++) for (var blockX = 0; blockX < 3; blockX++)
		{
			int x = 3 * blockX,
				y = 3 * blockY;

			var tiles = new Tile[3, 3];
			for (var by = 0; by < 3; by++) for (var bx = 0; bx < 3; bx++)
				tiles[bx, by] = new Tile(numbers[(x + bx) + (y + by) * 9]);
			
			_blocks[blockX, blockY] = new Block(tiles);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private readonly Block[,] _blocks = new Block[3, 3];

	public int CalculateHeuristic()
	{
		var h = 0;
		for (var y = 0; y < 9; y++)
		{
			var found = new List<Tile>(9);
			foreach (var tile in GetRowEnumerable(y))
			{
				if (found.Contains(tile)) h += 1;
				else found[y] = tile;
			}
		}

		for (var x = 0; x < 9; x++)
		{
			var found = new List<Tile>(9);
			foreach (var tile in GetColumnEnumerable(x))
			{
				if (found.Contains(tile)) h += 1;
				else found[x] = tile;
			}
		}	

		return h;
	}

	private IEnumerable<Tile> GetColumnEnumerable(int x)
	{
		var blockX         = x / 3; // The x-index of the relevant blocks
		var blockRelativeX = x % 3; // The x-index of the relevant column inside the blocks
		for (var y = 0; y < 3; y++) // For every relevant block containing the specified column
			foreach (var tile in _blocks[blockX, y].GetColumnEnumerable(blockRelativeX))
				// For every tile in the specified column, within this block
				yield return tile;
	}

	public IEnumerable<Tile> GetRowEnumerable(int y)
	{	// Does the same exact thing as GetColumnEnumerable(int x), but horizontally
		var blockY         = y / 3;
		var blockRelativeY = y % 3;
		for (var x = 0; x < 3; x++)
			foreach (var tile in _blocks[x, blockY].GetRowEnumerable(blockRelativeY))
				yield return tile;
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		const int totalWidth = (9 * 2 - 1) + 4; // 9 numbers with spaces, minus the last space, plus 2 separating bars with 2 spaces

		for (var y = 0; y < 9; y++)
		{
			foreach (var number in GetRowEnumerable(y))
				sb.Append($"{number} ");

			insertVertically("│ ");
			
			if (y != 8 && (y + 1) % 3 == 0) // Add horizontal pipes
			{
				sb.AppendLine();
				sb.Append('─', totalWidth - 3);
				insertVertically("┼─");
			}
			
			sb.AppendLine();

			void insertVertically(string seperator)
			{
				sb.Insert(sb.Length - 6 * 2, seperator);
				sb.Insert(sb.Length - 3 * 2, seperator);
			}
		}
		
		return sb.ToString();
	}
}

public struct Block
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private readonly Tile[,] _tiles = new Tile[3, 3];

	public Block(Tile[,] values)
	{
		_tiles = values;
	}

	public void Swap(byte ax, byte ay, byte bx, byte by) // TODO: Maybe define this as a 0 - 8 range index?
	{
		(_tiles[ax, ay], _tiles[bx, by]) = (_tiles[bx, by], _tiles[ax, ay]);
	}

	/// <returns>New heuristic value after swapping</returns>
	public int SwapHeuristic(byte ax, byte ay, byte bx, byte by, int h)
	{
		int hNew;
		throw new NotImplementedException();
	}

	public bool IsValid()
	{
		var count = 0;
		for (var y = 0; y < 3; y++) for (var x = 0; x < 3; x++) // For every tile
				count += _tiles[x, y].Value;
		return count == 45; // TODO: This is too naive, and won't always work
	}

	public IEnumerable<Tile> GetColumnEnumerable(int x)
	{
		for (var y = 0; y < 3; y++)
			yield return _tiles[x, y];
	}
	public IEnumerable<Tile> GetRowEnumerable(int y)
	{
		for (var x = 0; x < 3; x++)
			yield return _tiles[x, y];
	}

	public override string ToString() // For debugging purposes
	{
		StringBuilder sb = new();
		for (int i = 0; i < 3; i++)
		{
			sb.Append(", ");
			
			foreach (var tile in GetRowEnumerable(i))
				sb.Append($" {tile.ToString()}");
		}

		return sb.Remove(0, 2).ToString();
	}
}

[DebuggerDisplay("{Value}")]
public struct Tile
{
	public Tile(byte value)
	{
		this.Value = value;
	}

	public byte Value { get; set; }

	public void Deconstruct(out byte Value)
	{
		Value = this.Value;
	}

	public override string ToString() => Value > 0 ? Value.ToString() : "-";
}