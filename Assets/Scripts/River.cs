using UnityEngine;
using System.Collections.Generic;

public enum Direction
{
	Left,
	Right,
	Top,
	Bottom
}

public class River
{
	public int Length;
	public List<GenerationTile> Tiles;
	public int ID;

	public int Intersections;
	public float TurnCount;
	public Direction CurrentDirection;

	public River(int id)
	{
		ID = id;
		Tiles = new List<GenerationTile>();
	}

	public void AddTile(GenerationTile tile)
	{
		tile.SetRiverPath(this);
		Tiles.Add(tile);
	}
}

public class RiverGroup
{
	public List<River> Rivers = new List<River>();
}