using System.Diagnostics;

namespace SudokuCracker;
[DebuggerDisplay("{Value}, fixed: {IsFixed}")]
public class Tile
{
	public Tile(byte value, bool isFixed)
	{
		Value   = value;
		IsFixed = isFixed;
	}

	public byte Value   { get; set; }
	public bool IsFixed { get; set; }
	
	public void Deconstruct(out byte Value)
	{
		Value = this.Value;
	}

	public override string ToString() => Value > 0 ? Value.ToString() : "-";
}