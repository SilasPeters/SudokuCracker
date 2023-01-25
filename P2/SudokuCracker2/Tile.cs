namespace SudokuCracker2;
using System.Diagnostics;

[DebuggerDisplay("{Value}, fixed: {IsFixed}, domain: {Domain}")]
public struct Tile
{
	public Tile(byte? value, bool isFixed, IEnumerable<byte> domain)
	{
		Value   = value;
		IsFixed = isFixed;
		Domain  = domain.Aggregate("", (s, i) => s + i);
	}

	public byte?       Value   { get; set; }
	public bool        IsFixed { get; set; }
	public string      Domain  { get; set; }

	public override string ToString() => Value != null ? Value.ToString()! : "-";
}