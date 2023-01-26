namespace SudokuCracker2;
using System.Diagnostics;

[DebuggerDisplay("{Value}, fixed: {IsFixed}, domain: {Domain}")]
public struct Tile
{
	public Tile Clone(){
		Tile t = new Tile();
		t.Value = this.Value;
		t.IsFixed = this.IsFixed;
		t.Domain = new HashSet<byte>(this.Domain);
		return t;
	}
	public Tile(byte? value, bool isFixed, IEnumerable<byte> domain)
	{
		Value   = value;
		IsFixed = isFixed;
		Domain  = new HashSet<byte>(domain);
	}

	public byte?       Value   { get; set; }
	public bool        IsFixed { get; set; }
	public ISet<byte>  Domain  { get; set; }

	public override string ToString() => Value != null ? Value.ToString()! : "-";
}
