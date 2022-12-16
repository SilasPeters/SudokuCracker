using System.Collections;
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
		var found = new bool[10];
		
		foreach (var tile in tiles)
		{
			if (found[tile.Value]) h += 1;
			else found[tile.Value] = true;
		}

		return h;
	}
	
	/// <summary>Determines the new heuristic value after a potential swap</summary>
	/// <remarks>Assumes that both points, a and b, belong to the same block.</remarks>
	public int DetermineHeuristicChangeAfterSwap(int ax, int ay, int bx, int by, int currentH)
	{
		// Gather relevant data
		var columnA = GetColumnEnumerable(ax).ToList();
		var rowA    = GetRowEnumerable(ay)	 .ToList();
		var columnB = GetColumnEnumerable(bx).ToList();
		var rowB    = GetRowEnumerable(by)	 .ToList();

		var oldA = columnA[ay];
		var oldB = columnB[by];
		
		// Prepare data: remove the value to be swapped
		columnA.RemoveAt(ay);
		rowA.	RemoveAt(ax);
		columnB.RemoveAt(by);
		rowB.	RemoveAt(bx);

		return currentH
		     + getHeuristicChangeOf(columnA, oldA, oldB)
		     + getHeuristicChangeOf(rowA,    oldA, oldB)
		     + getHeuristicChangeOf(columnB, oldB, oldA)
		     + getHeuristicChangeOf(rowB,    oldB, oldA);
		
		int getHeuristicChangeOf(IEnumerable<Tile> enumerable, Tile old, Tile swapped)
		{
			bool oldValueWasDuplicate = enumerable.Contains(old,     Tile.ValueComparer);
			bool newValueIsDuplicate  = enumerable.Contains(swapped, Tile.ValueComparer);

			if (oldValueWasDuplicate)
			{
				return newValueIsDuplicate ? 0 : -1;
			}
			else
			{
				return newValueIsDuplicate ? 1 : 0;
			}
		}
	}
	///<remarks> swaps two tiles in the sudoku, assumes the two tiles are in the same block</remarks>
	public void Swap(int ax, int ay, int bx, int by) {
		var (block, xOffset, yOffset) = GetBlockContaining(ax, ay);
		block.Swap(ax - xOffset, // Assumes that both points are within the same block
			ay - yOffset,
			bx - xOffset,
			by - yOffset);
	}
	
	/// <returns> Returns whether the tile at (x,y) is a fixed tile</returns>
	public bool IsFixed(int x, int y) {
		var (block, _, _) = GetBlockContaining(x, y);
		return block.IsFixed(x % 3, y % 3);
	}

	/// <returns>Returns the block containing (x,y), and the offset of that block</returns>
	public (Block, int, int) GetBlockContaining(int x, int y) =>
		(_blocks[x/3, y/3], x/3 * 3, y/3 * 3); 

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