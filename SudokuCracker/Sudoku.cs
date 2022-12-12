using System.Diagnostics;
using System.Text;

namespace SudokuCracker;

public class Sudoku
{
	public Sudoku (IReadOnlyList<byte> numbers)
	{
		// Converts list of numbers to blocks
		for (var blockY = 0; blockY < 3; blockY++) for (var blockX = 0; blockX < 3; blockX++)
		{
			int x = 3 * blockX,
				y = 3 * blockY;

			var tiles = new Tile[3, 3];
			for (var by = 0; by < 3; by++) for (var bx = 0; bx < 3; bx++)
			{
				var tileValue = numbers[(x + bx) + (y + by) * 9];
				tiles[bx, by] = new Tile(tileValue, tileValue != 0);
			}
			
			_blocks[blockX, blockY] = new Block(tiles);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private readonly Block[,] _blocks = new Block[3, 3];

	public int CalculateHeuristicValue()
	{
		var h                         = 0;
		for (var y = 0; y < 9; y++) h += _CalculateHeuristicValueOfEnumerable(GetRowEnumerable(y));
		for (var x = 0; x < 9; x++) h += _CalculateHeuristicValueOfEnumerable(GetColumnEnumerable(x));

		return h;
	}

	private static int _CalculateHeuristicValueOfEnumerable(IEnumerable<Tile> tiles)
	{
		var h     = 0;
		var found = new List<byte>(9);
		
		foreach (var tile in tiles)
		{
			if (found.Contains(tile.Value)) h += 1;
			else found.Add(tile.Value);
		}

		return h;
	}
	
	/// <returns>New heuristic value after swapping</returns>
	/// <remarks>Assumes that both points, a and b, belong to the same block</remarks>
	public int Swap(byte ax, byte ay, byte bx, byte by, int currentHeuristicValue)
	{
		var oldH = calculateHeuristicValues(); // Can be way faster: just look if a duplicate is replaced by a unique, or reversed

		var (block, offset) = GetBlockContaining(ax, ay);
		block.Swap(ax - offset, // Assumes that both points are within the same block
			       ay - offset,
			       bx - offset,
			       by - offset);

		var newH = calculateHeuristicValues();
		return currentHeuristicValue + (oldH - newH);

		
		// Supporting method
		int calculateHeuristicValues()
		{
			var columnA = GetColumnEnumerable(ax);
			var rowA    = GetRowEnumerable(ay);
			var columnB = GetColumnEnumerable(ax);
			var rowB    = GetRowEnumerable(ay);
			
			return _CalculateHeuristicValueOfEnumerable(columnA) +
			       _CalculateHeuristicValueOfEnumerable(rowA) +
			       _CalculateHeuristicValueOfEnumerable(columnB) +
			       _CalculateHeuristicValueOfEnumerable(rowB);
		}
	}

	/// <returns>Returns the block containing (x,y), and the offset of that block</returns>
	public (Block, int) GetBlockContaining(int x, int y) =>
		(_blocks[x/3, y/3], x/3 * 3);

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
	{ // Does the same exact thing as GetColumnEnumerable(int x), but horizontally
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