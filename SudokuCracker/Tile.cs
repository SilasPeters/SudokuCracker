﻿using System.Diagnostics;

namespace SudokuCracker;
[DebuggerDisplay("{Value}, fixed: {IsFixed}")]
public class Tile
{
	public Tile(byte value, bool isFixed)
	{
		Value   = value;
		IsFixed = isFixed;
	}

	private sealed class ValueEqualityComparer : IEqualityComparer<Tile>
	{
		public bool Equals(Tile x, Tile y)
		{
			return x.Value == y.Value;
		}

		public int GetHashCode(Tile obj)
		{
			return obj.Value.GetHashCode();
		}
	}

	public static IEqualityComparer<Tile> ValueComparer { get; } = new ValueEqualityComparer();

	public byte Value   { get; set; }
	public bool IsFixed { get; set; }
	
	public void Deconstruct(out byte Value)
	{
		Value = this.Value;
	}

	public override string ToString() => Value > 0 ? Value.ToString() : "-";
}