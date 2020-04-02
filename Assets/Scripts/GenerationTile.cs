using UnityEngine;
using System;
using System.Collections.Generic;

public enum FloraType
{
    Grass,
    Bush,
    Shrub,
    Tree,
    Special
}

public enum HeatType
{
    Colder,
    Cold,
    Average,
    Warm,
    Warmer
}

public enum HeightType
{
	Ocean,
    River,
    Beach,
	Lowland,
	Plain,
	Hill,
	Mountain
}

public enum MoistureType
{
	Wettest,
	Wetter,
	Wet,
	Dry,
	Dryer,
	Dryest
}

public enum BiomeType
{
	Tundra,
	Taiga,
	Swamp,
	Plains,
	Ocean,
	Jungle,
	Forest,
	Desert,
    Savannah,
    Alpine,
    Beach,
    Steppe,
    Hills,
    River
}

public enum PlantType
{
    Grass,
    Berries,
    DryBush,
    Pine,
    Prickle,
    YoungBush,
    HighGrass,
    Acacia,
    Cactus,
    Birch,
    Flower,
    Oak,
    Palm,
    Citrus,
    Willow,
    Baobab
}

public class GenerationTile
{
    public BiomeType BiomeType;
    public MoistureType MoistureType;
    public HeightType HeightType;
    public HeatType HeatType;

    public float MoistureValue { get; set; }
    public float HeatValue { get; set; }
    public float HeightValue { get; set; }

    public float Fertility;
    public FloraType FloraType;
    public PlantType PlantType;

    public int X, Y;
    public GenerationTile Left;
    public GenerationTile Right;
    public GenerationTile Top;
    public GenerationTile Bottom;

    public bool Collidable;
    public List<River> Rivers = new List<River>();
    public int RiverSize { get; set; }

    public GenerationTile()
    { }

    public float CalcFertility()
    {
        return 0.05f * MoistureValue + 0.05f * HeatValue;
    }

    public void SetBiome()
    {
        MoistureType moisture = this.MoistureType;
        HeightType height = this.HeightType;
        switch (height)
        {
            case (HeightType.Ocean):
                this.BiomeType = BiomeType.Ocean;
                break;
            case (HeightType.River):
				this.BiomeType = BiomeType.River;
				break;
            case (HeightType.Beach):
                this.BiomeType = BiomeType.Beach;
                break;
            case (HeightType.Lowland):
				if (moisture == MoistureType.Wettest || moisture == MoistureType.Wetter)
					this.BiomeType = BiomeType.Swamp;
				else if (moisture == MoistureType.Wet)
					this.BiomeType = BiomeType.Forest;
                else if (moisture == MoistureType.Dry)
                    this.BiomeType = BiomeType.Plains;
                else if (moisture == MoistureType.Dryer)
					this.BiomeType = BiomeType.Savannah;
				else
					this.BiomeType = BiomeType.Desert;
				break;
			case (HeightType.Plain):
				if (moisture == MoistureType.Wettest || moisture == MoistureType.Wetter)
					this.BiomeType = BiomeType.Jungle;
				else if (moisture == MoistureType.Wet)
					this.BiomeType = BiomeType.Forest;
				else if (moisture == MoistureType.Dry)
					this.BiomeType = BiomeType.Plains;
                else if (moisture == MoistureType.Dryer)
                    this.BiomeType = BiomeType.Savannah;
                else
					this.BiomeType = BiomeType.Desert;
				break;
			case (HeightType.Hill):
				if (moisture == MoistureType.Wettest || moisture == MoistureType.Wetter)
					this.BiomeType = BiomeType.Taiga;
				else if (moisture == MoistureType.Wet || moisture == MoistureType.Dry)
					this.BiomeType = BiomeType.Hills;
                else
                    this.BiomeType = BiomeType.Steppe;
                break;
			case (HeightType.Mountain):
				if (moisture == MoistureType.Wettest || moisture == MoistureType.Wetter || moisture == MoistureType.Wet)
					this.BiomeType = BiomeType.Alpine;
				else
					this.BiomeType = BiomeType.Tundra;
				break;
		}
    }

    //Heat
    public void SetHeat(float heat, Biome heatConst)
    {
        if (heat < 0.2f)
            this.HeatType = HeatType.Colder;
        else if (heat < 0.3f)
            this.HeatType = HeatType.Cold;
        else if (heat < 0.5f)
            this.HeatType = HeatType.Average;
        else if (heat < 0.6f)
            this.HeatType = HeatType.Warm;
        else
            this.HeatType = HeatType.Warmer;

        this.HeatValue = heat;// * heatConst.Range + heatConst.Offset;
    }


    //River
    public int GetRiverNeighborCount(River river)
    {
        int count = 0;
        if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
            count++;
        if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
            count++;
        if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
            count++;
        if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
            count++;
        return count;
    }

    public Direction GetLowestNeighbor()
    {
        if (Left.HeightValue < Right.HeightValue && Left.HeightValue < Top.HeightValue && Left.HeightValue < Bottom.HeightValue)
            return Direction.Left;
        else if (Right.HeightValue < Left.HeightValue && Right.HeightValue < Top.HeightValue && Right.HeightValue < Bottom.HeightValue)
            return Direction.Right;
        else if (Top.HeightValue < Left.HeightValue && Top.HeightValue < Right.HeightValue && Top.HeightValue < Bottom.HeightValue)
            return Direction.Right;
        else if (Bottom.HeightValue < Left.HeightValue && Bottom.HeightValue < Top.HeightValue && Bottom.HeightValue < Right.HeightValue)
            return Direction.Right;
        else
            return Direction.Bottom;
    }

    public void SetRiverPath(River river)
    {
        if (!Collidable)
            return;

        if (!Rivers.Contains(river))
        {
            Rivers.Add(river);
        }
    }

    private void SetRiverTile(River river)
    {
        SetRiverPath(river);
        HeightType = HeightType.River;
        HeightValue = 0;
        Collidable = false;
    }

    public void DigRiver(River river, int size)
    {
        SetRiverTile(river);
        RiverSize = size;

        if (size == 1)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
        }

        if (size == 2)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Left.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
        }

        if (size == 3)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Left.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
            Right.Right.SetRiverTile(river);
            Right.Right.Bottom.SetRiverTile(river);
            Bottom.Bottom.SetRiverTile(river);
            Bottom.Bottom.Right.SetRiverTile(river);
        }

        if (size == 4)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
            Right.Right.SetRiverTile(river);
            Right.Right.Bottom.SetRiverTile(river);
            Bottom.Bottom.SetRiverTile(river);
            Bottom.Bottom.Right.SetRiverTile(river);
            Left.Bottom.Bottom.SetRiverTile(river);
            Left.Left.Bottom.SetRiverTile(river);
            Left.Left.SetRiverTile(river);
            Left.Left.Top.SetRiverTile(river);
            Left.Top.SetRiverTile(river);
            Left.Top.Top.SetRiverTile(river);
            Top.Top.SetRiverTile(river);
            Top.Top.Right.SetRiverTile(river);
            Top.Right.Right.SetRiverTile(river);
        }
    }
}
