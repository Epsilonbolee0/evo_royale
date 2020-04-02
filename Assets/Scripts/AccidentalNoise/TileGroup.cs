using UnityEngine;
using System.Collections.Generic;

public enum TileGroupType
{
	Water,
	Land
}

public class TileGroup
{

	public TileGroupType Type;
	public List<Tile> Tiles;

	public TileGroup()
	{
		Tiles = new List<Tile>();
	}

	public List<River> Rivers = new List<River>();
	public int RiverSize { get; set; }
}