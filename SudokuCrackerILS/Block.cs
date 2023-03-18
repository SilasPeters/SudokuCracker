using System.Diagnostics;
using System.Text;

namespace SudokuCrackerILS;
public class Block
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private readonly Tile[,] _tiles;

	public Block(Tile[,] values)
	{
		_tiles = values;
		_Fill(); // Replace all zeroes
	}

	public void Swap(int ax, int ay, int bx, int by)
	{
		// Get references to the tiles in question
		ref var a = ref _tiles[ax, ay];
		ref var b = ref _tiles[bx, by];

		// Check whether the swap is allowed
		if (a.IsFixed || b.IsFixed)
			throw new Exception("Tried to swap a fixed value");
		
		// Swap
		(a, b) = (b, a);
	}

	/// <returns> Returns whether the tile at (x,y) is a fixed tile</returns>
	public bool IsFixed(int x, int y) {
		// Check whether the request is valid
		if (x > 2 || y > 2 || x < 0 || y < 0){
			throw new Exception($"({x}, {y}) isn't a valid tile!\n");
		}
		return _tiles[x,y].IsFixed;
	}

	public IEnumerable<Tile> GetColumnEnumerable(int x)
	{
		for (var y = 0; y < 3; y++) // For every row
			yield return _tiles[x, y]; // Get the tile at x
	}
	public IEnumerable<Tile> GetRowEnumerable(int y)
	{
		for (var x = 0; x < 3; x++) // For every column
			yield return _tiles[x, y]; // Get the tile at y
	}
	
	public IEnumerable<Tile> GetAllTilesEnumerable ()
	{
		for (var y = 0; y < 3; y++) // For every row
			for (var x = 0; x < 3; x++) // For every column
				yield return _tiles[x,y]; // Get the tile
	}

	private void _Fill()
	{
		// Store the numbers which are present, using old-school LINQ
		var presentNumbers = (
			from tile in GetAllTilesEnumerable()
			where tile.Value is not 0
			select tile.Value
		).Distinct().ToList();

		for (var y = 0; y < 3; y++) for (var x = 0; x < 3; x++) // For every tile in this block
		{
			if (_tiles[x, y].Value != 0) continue;
			// Try to fill the 0 with a number not present yet
			for (byte i = 1; i <= 9; i++)
				if (!presentNumbers.Contains(i))
				{
					_tiles[x, y].Value = i; // Store the number
					presentNumbers.Add(i); // Remember that we added the number
					break;
				}
		}
	}

	public override string ToString() // For debugging purposes
	{
		StringBuilder sb = new();
		for (var i = 0; i < 3; i++)
		{
			sb.Append(", ");
			
			foreach (var tile in GetRowEnumerable(i))
				sb.Append($" {tile.ToString()}");
		}

		return sb.Remove(0, 2).ToString();
	}
}