using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public BiomeType BiomeType;

    public float MoistureValue { get; set; }
    public float HeatValue { get; set; }

    public float Fertility;
    public PlantType PlantType;
    public GameObject Plant;

    public bool isPlant;
    public bool isEmpty;

    public Tile() {
    }

    public void CopyTile(GenerationTile tile)
    {
        BiomeType = tile.BiomeType;
        MoistureValue = tile.MoistureValue;
        HeatValue = tile.HeatValue;
        PlantType = tile.PlantType;
        Fertility = tile.Fertility;
        SetEmptiness();
    }

    private void SetEmptiness()
    {
        if (BiomeType == BiomeType.Ocean)
            SetOccupied();
        else
            SetEmpty();
    }

    public void SetEmpty()
    {
        isEmpty = true;
    }

    public void SetOccupied()
    {
        isEmpty = false;
    }
}

public class TileMatrix
{
    public Tile[,] tiles;
    public int width;
    public int height;

    public TileMatrix(int width, int height)
    {
        this.width = width;
        this.height = height;
        tiles = new Tile[width, height];
    }

    public void CopyMatrix(GenerationTile[,] tiles)
    {
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                Tile tmp = new Tile();
                tmp.CopyTile(tiles[x, y]);
                this.tiles[x, y] = tmp;
            }
    }

    public Vector2 ConvertDirection(int x, int y, DirectionType dt)
    {
        //Nikita Garasev - Это гениальная идея, что если креатуре не двигается, то он остается на месте
        Vector2 vector = new Vector2(x, y);
        switch (dt)
        {
            case DirectionType.Top:
                if (y < height - 1)
                {
                    vector.x = x;
                    vector.y = y + 1;
                }
                    break;
            case DirectionType.TopRight:
                if (x < width - 1 && y < height - 1)
                {
                    vector.x = x + 1;
                    vector.y = y + 1;
                }
                break;
            case DirectionType.Right:
                if (x < width - 1)
                {
                    vector.x = x + 1;
                    vector.y = y;
                }
                break;
            case DirectionType.BottomRight:
                if (x < width - 1 && y > 0)
                {
                    vector.x = x + 1;
                    vector.y = y - 1;
                }
                break;
            case DirectionType.Bottom:
                if (y > 0)
                {
                    vector.x = x;
                    vector.y = y - 1;
                }
                break;
            case DirectionType.BottomLeft:
                if (x > 0 && y > 0)
                {
                    vector.x = x - 1;
                    vector.y = y - 1;
                }
                break;
            case DirectionType.Left:
                if (x > 0)
                {
                    vector.x = x - 1;
                    vector.y = y;
                }
                break;
            case DirectionType.TopLeft:
                if (x > 0 && y < height - 1)
                {
                    vector.x = x - 1;
                    vector.y = y + 1;
                }
                break;
        }
        return vector;
    }
}