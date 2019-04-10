using System;
using System.Collections.Generic;

/// <summary>
/// A Ship has all the details about itself. For example the shipname,
/// size, number of hits taken and the location. Its able to add tiles,
/// remove, hits taken and if its deployed and destroyed.
/// </summary>
/// <remarks>
/// Deployment information is supplied to allow ships to be drawn.
/// </remarks>
public class Ship
{
	private ShipName _shipName;
	private int _sizeOfShip;
	private int _hitsTaken = 0;
	private List<Tile> _tiles;
	private int _row;
	private int _col;
	private Direction _direction;
	
	/// <summary>
	/// The type of ship
	/// </summary>
	/// <value>The type of ship</value>
	/// <returns>The type of ship</returns>
	public string Name
	{
		get
		{
			if (_shipName == ShipName.AircraftCarrier)
			{
				return "Aircraft Carrier";
			}
			
			return _shipName.ToString();
		}
	}
	
	/// <summary>
	/// The number of cells that this ship occupies.
	/// </summary>
	/// <value>The number of hits the ship can take</value>
	/// <returns>The number of hits the ship can take</returns>
	public int Size
	{
		get
		{
			return _sizeOfShip;
		}
	}
	
	/// <summary>
	/// The number of hits that the ship has taken.
	/// </summary>
	/// <value>The number of hits the ship has taken.</value>
	/// <returns>The number of hits the ship has taken</returns>
	/// <remarks>When this equals Size the ship is sunk</remarks>
	public int Hits
	{
		get
		{
			return _hitsTaken;
		}
	}
	
	/// <summary>
	/// The row location of the ship
	/// </summary>
	/// <value>The topmost location of the ship</value>
	/// <returns>the row of the ship</returns>
	public int Row
	{
		get
		{
			return _row;
		}
	}

	/// <summary>
	/// Gets the column location of the ship.
	/// </summary>
	/// <value>The column.</value>
	public int Column
	{
		get
		{
			return _col;
		}
	}

	/// <summary>
	/// Gets the direction of the ship.
	/// </summary>
	/// <value>The direction.</value>
	public Direction Direction
	{
		get
		{
			return _direction;
		}
	}

	/// <summary>
	/// Initializes a new instance of the Ship class.
	/// </summary>
	/// <param name="ship">Ship.</param>
	public Ship(ShipName ship)
	{
		_shipName = ship;
		_tiles = new List<Tile>();
		
		//gets the ship size from the enumarator
		_sizeOfShip = (int)_shipName;
	}
	
	/// <summary>
	/// Add tile adds the ship tile
	/// </summary>
	/// <param name="tile">one of the tiles the ship is on</param>
	public void AddTile(Tile tile)
	{
		_tiles.Add(tile);
	}
	
	/// <summary>
	/// Remove clears the tile back to a sea tile
	/// </summary>
	public void Remove()
	{
		foreach (Tile tile in _tiles)
		{
			tile.ClearShip();
		}
		_tiles.Clear();
	}

	/// <summary>
	/// Increases the hits taken counter if a shot hits the enemy's ship.
	/// </summary>
	public void Hit()
	{
		_hitsTaken++;
	}
	
	/// <summary>
	/// IsDeployed returns if the ships is deployed, if its deplyed it has more than
	/// 0 tiles
	/// </summary>
	public bool IsDeployed
	{
		get
		{
			return _tiles.Count > 0;
		}
	}

	/// <summary>
	/// Gets a value indicating whether a ship is destroyed.
	/// </summary>
	/// <value><c>true</c> if is destroyed; otherwise, <c>false</c>.</value>
	public bool IsDestroyed
	{
		get
		{
			return Hits == Size;
		}
	}
	
	/// <summary>
	/// Record that the ship is now deployed.
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="row"></param>
	/// <param name="col"></param>
	internal void Deployed(Direction direction, int row, int col)
	{
		_row = row;
		_col = col;
		_direction = direction;
	}
}