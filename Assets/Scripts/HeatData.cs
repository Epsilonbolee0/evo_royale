using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome
{
    public int Range;
    public int Offset;

    public float[] FloraChance;
    public float BiomeMulti = 0;

    public Biome()
    {
    }

    public void InitConsts(int Biome)
    {
        FloraChance = new float[5];
        switch (Biome)
        {
            case ((int)BiomeType.Tundra):
                Range = 15;
                Offset = -20;

                FloraChance[(int)FloraType.Grass] = 0.7f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0.3f;

                BiomeMulti = 0.1f;
                break;
            case ((int)BiomeType.Taiga):
                Range = 20;
                Offset = -5;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0.3f;
                FloraChance[(int)FloraType.Shrub] = 0.2f;
                FloraChance[(int)FloraType.Tree] = 0.5f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0.4f;
                break;
            case ((int)BiomeType.Swamp):
                Range = 20;
                Offset = 10;

                FloraChance[(int)FloraType.Grass] = 0.5f;
                FloraChance[(int)FloraType.Bush] = 0.3f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0.2f;

                BiomeMulti = 0.5f;
                break;
            case ((int)BiomeType.Plains):
                Range = 15;
                Offset = 10;

                FloraChance[(int)FloraType.Grass] = 0.4f;
                FloraChance[(int)FloraType.Bush] = 0.1f;
                FloraChance[(int)FloraType.Shrub] = 0.1f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0.4f;

                BiomeMulti = 0.4f;
                break;
            case ((int)BiomeType.Ocean):
                Range = 20;
                Offset = 0;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0f;
                break;
            case ((int)BiomeType.Jungle):
                Range = 20;
                Offset = 20;

                FloraChance[(int)FloraType.Grass] = 0.2f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0.2f;
                FloraChance[(int)FloraType.Tree] = 0.2f;
                FloraChance[(int)FloraType.Special] = 0.4f;

                BiomeMulti = 0.7f;
                break;
            case ((int)BiomeType.Forest):
                Range = 20;
                Offset = 0;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0.2f;
                FloraChance[(int)FloraType.Shrub] = 0.2f;
                FloraChance[(int)FloraType.Tree] = 0.3f;
                FloraChance[(int)FloraType.Special] = 0.3f;

                BiomeMulti = 0.5f;
                break;
            case ((int)BiomeType.Desert):
                Range = 25;
                Offset = 25;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0.5f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0.5f;

                BiomeMulti = 0.2f;
                break;
            case ((int)BiomeType.Savannah):
                Range = 20;
                Offset = 15;

                FloraChance[(int)FloraType.Grass] = 0.5f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0.3f;
                FloraChance[(int)FloraType.Special] = 0.2f;

                BiomeMulti = 0.3f;
                break;
            case ((int)BiomeType.Alpine):
                Range = 25;
                Offset = -35;

                FloraChance[(int)FloraType.Grass] = 1f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0.1f;
                break;
            case ((int)BiomeType.Beach):
                Range = 20;
                Offset = 0;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0f;
                break;
            case ((int)BiomeType.Steppe):
                Range = 20;
                Offset = 0;

                FloraChance[(int)FloraType.Grass] = 0.2f;
                FloraChance[(int)FloraType.Bush] = 0.3f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0.2f;
                FloraChance[(int)FloraType.Special] = 0.3f;

                BiomeMulti = 0.3f;
                break;
            case ((int)BiomeType.Hills):
                Range = 20;
                Offset = -5;

                FloraChance[(int)FloraType.Grass] = 0.25f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0.25f;
                FloraChance[(int)FloraType.Tree] = 0.5f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0.3f;
                break;
            case ((int)BiomeType.River):
                Range = 20;
                Offset = 0;

                FloraChance[(int)FloraType.Grass] = 0f;
                FloraChance[(int)FloraType.Bush] = 0f;
                FloraChance[(int)FloraType.Shrub] = 0f;
                FloraChance[(int)FloraType.Tree] = 0f;
                FloraChance[(int)FloraType.Special] = 0f;

                BiomeMulti = 0f;
                break;
            default:
                break;
        }
    }
}