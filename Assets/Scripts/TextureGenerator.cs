using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour {

    //Ground
    public static void SetGround(TileMatrix tiles, GameObject[] BiomeTiles)
    {
        for (var x = 0; x < tiles.width; x++)
        {
            for (var y = 0; y < tiles.height; y++)
            {
                BiomeType biome_type = tiles.tiles[x, y].BiomeType;
                Instantiate(BiomeTiles[(int)biome_type], new Vector3(x * 0.64f, y * 0.64f, 0f), Quaternion.identity);
            }
        }
    }

    //Flora
    public static void SetFlora(TileMatrix tiles, GameObject[] FloraTiles, Biome[] Biomes)
    {
        for (var x = 0; x < tiles.width; x++)
        {
            for (var y = 0; y < tiles.height; y++)
            {
                SetBiomePlant(tiles, FloraTiles, Biomes, x, y);
            }
        }
    }

    public static void SetBiomePlant(TileMatrix tiles, GameObject[] FloraTiles, Biome[] Biomes, int x, int y)
    {
        if (Biomes[(int)tiles.tiles[x, y].BiomeType].BiomeMulti > UnityEngine.Random.value)
            SetPlant(tiles.tiles[x, y], FloraTiles, x, y);
    }

    public static void SetPlant(Tile tile, GameObject[] FloraTiles, int x, int y)
    {
        PlantType plant_type = tile.PlantType;
        tile.isPlant = true;
        tile.Plant = Instantiate(FloraTiles[(int)plant_type], new Vector3(x * 0.64f, y * 0.64f, -1f), Quaternion.identity);
    }

    public static void DestroyPlant(Tile tile)
    {
        Destroy(tile.Plant);
    }

    //Creature
    public static void SetCreatures(List<Creature> CreatureGroup, GameObject Obsos)
    {
        for (var i = 0; i < CreatureGroup.Count; i++)
            SetCreature(CreatureGroup[i], Obsos);
    }

    public static void SetCreature(Creature creature, GameObject Obsos)
    {
        creature.Skin = Instantiate(Obsos, new Vector3(creature.GetX() * 0.64f, creature.GetY() * 0.64f, -2f), Quaternion.identity);
    }

    public static void DestroyCreatures(List<Creature> CreatureGroup)
    {
        for (var i = 0; i < CreatureGroup.Count; i++)
            DestroyCreature(CreatureGroup[i]);
    }

    public static void DestroyCreature(Creature creature)
    {
        Destroy(creature.Skin);
    }

    //Additional
    public static void SetGoal(Creature creature, int x, int y, GameObject Extra)
    {
        creature.Goal = Instantiate(Extra, new Vector3(x * 0.64f, y * 0.64f, -0.5f), Quaternion.identity);
    }
    public static void SetPath(Creature creature, int x, int y, GameObject Extra)
    {
        creature.Path = Instantiate(Extra, new Vector3(x * 0.64f, y * 0.64f, -0.5f), Quaternion.identity);
    }

    public static void DestroyExtra(Creature creature)
    {
        Destroy(creature.Goal);
        Destroy(creature.Path);
    }

    public static void SetVision(Creature creature, int x1, int y1, int x2, int y2, GameObject Extra)
    {
        creature.Vision = null;
        creature.Vision = new GameObject[(x2 - x1 + 1) * (y2 - y1 + 1)];
        int count = 0;
        for (var i = x1; i <= x2; i++)
            for (var j = y1; j <= y2; j++)
            {
                creature.Vision[count++] = Instantiate(Extra, new Vector3(i * 0.64f, j * 0.64f, -0.5f), Quaternion.identity);
            }
    }

    public static void DestroyVision(Creature creature)
    {
        for (var i = 0; i < creature.Vision.Length; i++)
            Destroy(creature.Vision[i]);
        
    }
}