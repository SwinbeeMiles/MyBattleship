/// <summary>
/// The result of an attack.
/// </summary>
public enum ResultOfAttack
{
	/// <summary>
	/// The player hit something
	/// </summary>
	Hit,
	
	/// <summary>
	/// The player missed
	/// </summary>
	Miss,
	
	/// <summary>
	/// The player destroyed a ship
	/// </summary>
	Destroyed,
	
	/// <summary>
	/// That location was already shot.
	/// </summary>
	ShotAlready,
	
	/// <summary>
	/// The player killed all of the opponents ships
	/// </summary>
	GameOver
}