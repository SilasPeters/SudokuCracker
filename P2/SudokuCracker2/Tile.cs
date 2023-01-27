namespace SudokuCracker2;
using System.Diagnostics;

[DebuggerDisplay("{Value}, fixed: {IsFixed}, domain: {Domain}")]
public struct Tile
{
	public Tile Clone(){
		Tile t = new Tile();
		t.Value = this.Value;
		t.IsFixed = this.IsFixed;
		// t.Domain = new List<byte>(this.Domain);
		t.Domain = CloneList(this.Domain);
		return t;
	}
	public List<byte> CloneList(List<byte> original){
		List<byte> output = new List<byte>();
		foreach (byte b in original){
			output.Add(b);
		}
		return output;
	}
	public Tile(byte? value, bool isFixed, IEnumerable<byte> domain)
	{
		Value   = value;
		IsFixed = isFixed;
		Domain  = new List<byte>(domain);
	}

	public byte?       Value   { get; set; }
	public bool        IsFixed { get; set; }
	public List<byte>  Domain  { get; set; }

	public override string ToString() => Value != null ? Value.ToString()! : "-";
}
