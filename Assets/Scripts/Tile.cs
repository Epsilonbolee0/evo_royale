﻿using UnityEngine;
using System;
using System.Collections.Generic;

public enum HeightType
{
	Water,
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
    Beach
}

public class Tile
{
	public BiomeType BiomeType;
	public float HeightValue { get; set; }
	public int X, Y;

	public Tile()
	{
	}

	public void SetBiome(MoistureType moisture, HeightType height)
    {
		switch (height)
        {
			case (HeightType.Water):
				this.BiomeType = BiomeType.Ocean;
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
				else if (moisture == MoistureType.Wet)
					this.BiomeType = BiomeType.Forest;
                else if (moisture == MoistureType.Dry)
                    this.BiomeType = BiomeType.Plains;
                else if (moisture == MoistureType.Dryer)
                    this.BiomeType = BiomeType.Savannah;
                else
                    this.BiomeType = BiomeType.Desert;
                break;
			case (HeightType.Mountain):
				if (moisture == MoistureType.Wettest || moisture == MoistureType.Wetter || moisture == MoistureType.Wet)
					this.BiomeType = BiomeType.Alpine;
				else
					this.BiomeType = BiomeType.Tundra;
				break;
		}

    }
}