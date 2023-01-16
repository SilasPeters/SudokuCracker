using System.Diagnostics;

namespace SudokuCracker;
[DebuggerDisplay("{Value}, fixed: {IsFixed}")]
public struct Tile
{
	public Tile(byte value, bool isFixed)
	{
		Value   = value;
		IsFixed = isFixed;
	}

	// A custom equality comparer, used to compare tiles based on their contents
	private sealed class ValueEqualityComparer : IEqualityComparer<Tile>
	{
		public bool Equals(Tile x, Tile y) => x.Value == y.Value;
		public int GetHashCode(Tile obj) => obj.Value.GetHashCode();
	}
	public static IEqualityComparer<Tile> ValueComparer { get; } = new ValueEqualityComparer();

	public byte Value   { get; set; }
	public bool IsFixed { get; set; }

	public override string ToString() => Value > 0 ? Value.ToString() : "-";
}