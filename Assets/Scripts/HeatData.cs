using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatConst
{
    public int Range;
    public int Offset;

    public HeatConst()
    {
    }

    public void InitConsts(int Biome)
    {
        switch (Biome)
        {
            case ((int)BiomeType.Tundra):
                this.Range = 15;
                this.Offset = -20;
                break;
            case ((int)BiomeType.Taiga):
                this.Range = 20;
                this.Offset = -5;
                break;
            case ((int)BiomeType.Swamp):
                this.Range = 20;
                this.Offset = 10;
                break;
            case ((int)BiomeType.Plains):
                this.Range = 15;
                this.Offset = 10;
                break;
            case ((int)BiomeType.Ocean):
                this.Range = 20;
                this.Offset = 0;
                break;
            case ((int)BiomeType.Jungle):
                this.Range = 20;
                this.Offset = 20;
                break;
            case ((int)BiomeType.Forest):
                this.Range = 20;
                this.Offset = 0;
                break;
            case ((int)BiomeType.Desert):
                this.Range = 25;
                this.Offset = 25;
                break;
            case ((int)BiomeType.Savannah):
                this.Range = 20;
                this.Offset = 15;
                break;
            case ((int)BiomeType.Alpine):
                this.Range = 25;
                this.Offset = -35;
                break;
            case ((int)BiomeType.Beach):
                this.Range = 20;
                this.Offset = 0;
                break;
            case ((int)BiomeType.Steppe):
                this.Range = 20;
                this.Offset = 0;
                break;
            case ((int)BiomeType.Hills):
                this.Range = 20;
                this.Offset = -5;
                break;
            default:
                break;
        }
    }
}