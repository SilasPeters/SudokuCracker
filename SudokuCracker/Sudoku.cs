namespace SudokuCracker;

public struct Sudoku
{
	private Group[,] groups = new Group[3, 3];


	public IEnumerator<Tile> GetRowEnumerator(int y)
	{
		var groupY = y / 3;
		for (int x = 0; x < 3; x++)
			foreach (var tile in groups[x, groupY].GetRowEnumerator(y % 3);)
		{
			yield return tile;
		}
	}
}

public struct Group
{
	private Tile[,] tiles = new Tile[3, 3];
	
	public void Swap(byte one, byte two)
	{
		
	}

	public bool IsValid()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<Tile> GetRowEnumerator(int y)
	{
		for (var x = 0; x < 3; x++)
			yield return tiles[x, y];
	}
}

public struct Tile
{
	public byte Value { get; set; }
}