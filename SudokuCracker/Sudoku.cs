namespace SudokuCracker;

public struct Sudoku
{
	public Sudoku () {
	}

	private Block[,] blocks = new Block[3, 3];

	public int CalculateHeuristic()
	{
		int h = 0;
		for (int y = 0; y < 9; y++)
		{
			var found = new List<Tile>(9);
			foreach (var tile in GetRowEnumerable(y))
			{
				if (found.Contains(tile)) h += 1;
				else found[y] = tile;
			}
		}

		for (int x = 0; x < 9; x++)
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
			foreach (var tile in blocks[blockX, y].GetColumnEnumerable(blockRelativeX))
				// For every tile in the specified column, within this block
				yield return tile;
	}

	public IEnumerable<Tile> GetRowEnumerable(int y)
	{	// Does the same exact thing as GetColumnEnumerable(int x), but horizontally
		var blockY         = y / 3;
		var blockRelativeY = y % 3;
		for (var x = 0; x < 3; x++)
			foreach (var tile in blocks[x, blockY].GetRowEnumerable(blockRelativeY))
				yield return tile;
	}
}

public struct Block {
	private Tile[,] tiles = new Tile[3, 3];

	public Block()
	{
	}

	public void Swap(byte ax, byte ay, byte bx, byte by) // TODO: Maybe define this as a 0 - 8 range index?
	{
		(tiles[ax, ay], tiles[bx, by]) = (tiles[bx, by], tiles[ax, ay]);
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
				count += tiles[x, y].Value;
		return count == 45;
	}

	public IEnumerable<Tile> GetColumnEnumerable(int x)
	{
		for (var y = 0; y < 3; y++)
			yield return tiles[x, y];
	}
	public IEnumerable<Tile> GetRowEnumerable(int y)
	{
		for (var x = 0; x < 3; x++)
			yield return tiles[x, y];
	}
}

public struct Tile
{
	public byte Value { get; set; }
}