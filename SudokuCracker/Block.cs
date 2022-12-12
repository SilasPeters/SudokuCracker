﻿using System.Diagnostics;
using System.Text;

namespace SudokuCracker;
public struct Block
{
	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	private readonly Tile[,] _tiles = new Tile[3, 3];

	public Block(Tile[,] values)
	{
		_tiles = values;
	}

	public void Swap(int ax, int ay, int bx, int by)
	{
		ref var a = ref _tiles[ax, ay];
		ref var b = ref _tiles[bx, by];

		// Logics
		if (a.IsFixed || b.IsFixed)
			throw new Exception("Tried to swap a fixed value");
		
		// Swap
		(a, b) = (b, a);
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