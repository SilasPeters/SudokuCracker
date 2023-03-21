namespace SudokuCrackerCBT;
using System.Diagnostics;

[DebuggerDisplay($"{{Debug}}")]
public struct Tile
{
	private ushort _backing; // ushort is a 16-bit value-type variable which stores:
	// The 9 Most-Significant (first) bits represent the domain of the tile.
	// The 1 bit after that resembles whether this tile was given/is fixed.
	// The 4 Least-Significant (last) bits represent the current value of the tile.
	//
	// Or, more graphically:
	// [1111 1111 1100 1111] (Every bit marked with 0 is never utilised)
	//  ===========          Domain: i-th bit represents if i is a in this tile's domain
	//             =         IsFixed: 1 if the tile is fixed, 0 otherwise
	//                 ====  Value: the current value of the tile

	// Bitwise operations are used to manipulate the contents of the tile.
	public byte Value // Wraps the value of the tile
	{
		get => (byte) (_backing & 0x000F);
		set => _backing = (ushort) ((_backing & 0xFFF0) | (value & 0x000F));
	}
	public bool IsFixed
	{
		get => (_backing & 0x0040) != 0;
		private init => _backing = (ushort) ((_backing & 0xFFBF) | ((value ? 1 : 0) << 6));
	}
	public void ConstraintAdd (int x) => _backing |= (ushort) (1 << (16-x)); // Assumes that x < 10
	public void DomainRemove (int x) => _backing &= (ushort) ~(1 << (16-x)); // Assumes that x < 10
	public bool DomainNotEmpty () => (_backing & 0xFF80) != 0;
	public IEnumerable<byte> Domain ()
	{
		for (byte i = 1; i < 10; i++) // For every bit in the domain
			if ((_backing & (1 << (16 - i))) != 0) // If the bit is set
				yield return i; // Return i, which is thus part of this tile's domain
	}

	public Tile (byte value, bool isFixed, IEnumerable<int> domain)
	{
		Value   = value;
		IsFixed = isFixed;
		
		foreach (var c in domain)
			ConstraintAdd(c);
	}

	public override string ToString () => Value != 0 ? Value.ToString() : "-";
	// Method used to print every property of this tile
	public string Debug => $"Value: {Value}, IsFixed: {IsFixed}, Domain: {string.Join(", ", Domain())}"; 
}
