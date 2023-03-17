namespace SudokuCrackerCBT;
using System.Diagnostics;

[DebuggerDisplay($"{{Value}}, fixed: {{IsFixed}}, domain: {{Constraint}}, backing: {{domain}}")]
public struct Tile
{
	private short domain;
	// The 9 Most-Significant (first) bits represent the constraints of the tile.
	// The 1 bit after that resembles whether this tile was given/is fixed.
	// The 4 Least-Significant (last) bits represent the current value of the tile.
	//
	// Or, more graphically:
	// [1111111111001111] (Every bit marked with 0 is never utilised)
	//  ^-------^         Constraint: ith bit represents if i is a possible value for this tile
	//           ^        IsFixed: 1 if the tile is fixed, 0 otherwise
	//              ^--^  Value: the current value of the tile

	public byte Value
	{
		get => (byte) (domain & 0x000F);
		set => domain = (short) ((domain & 0xFFF0) | (value & 0x000F));
	}

	public bool IsFixed
	{
		get => (domain & 0x0040) != 0;
		private init => domain = (short) ((domain & 0xFFBF) | ((value ? 1 : 0) << 6));
	}
	
	public void ConstraintAdd(byte x) => domain |= (short) (1 << (16-x)); // Assumes that x < 10
	public void ConstraintRemove(byte x) => domain &= (short) ~(1 << (16-x));
	public bool ConstraintAny() => (domain & 0xFF80) != 0;
	public IEnumerable<byte> Constraint()
	{
		for (byte i = 1; i < 10; i++)
			if ((domain & (1 << (16 - i))) != 0)
				yield return i;
	}

	public Tile(byte value, bool isFixed, IEnumerable<byte> constraint)
	{
		Value   = value;
		IsFixed = isFixed;
		
		foreach (var c in constraint)
			ConstraintAdd(c);
	}

	public override string ToString() => Value != 0 ? Value.ToString() : "-";
}
