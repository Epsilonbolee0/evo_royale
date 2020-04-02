using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public List<Creature> CreatureGroup;
    public TileMatrix tileMatrix;
    public Biome[] Biomes;

    public GameObject[] BiomeTiles;
    public GameObject[] FloraTiles;
    public GameObject Obsos;
    public GameObject[] ExtraTiles;

    public const int CreatureNumbers = 1;

    public int Time = 1;

    void Start()
    {
        Generator Generator;
        Generator = new Generator(BiomeTiles, FloraTiles);

        tileMatrix = Generator.tileMatrix;
        Biomes = Generator.Biomes;

        Generator = null;

        GenerateCreatures();
    }

    void Update()
    {
        Time++;
        int DrawTime = 250;
        int SpawnTime = (int) (750 / CreatureNumbers);

        if (Time % DrawTime == 0)
        {
            ManageCreatures(); //AI
            DrawingObsos();
        }

        if (Time % SpawnTime == 0)
        {
            SpawnPlant();
        }
    }

    public void SpawnPlant()
    {
        int counter = 0;
        const int counter_limit = 5;
        while (true)
        {
            counter++;
            var x = UnityEngine.Random.Range(0, tileMatrix.width);
            var y = UnityEngine.Random.Range(0, tileMatrix.height);
            if (tileMatrix.tiles[x, y].Plant == null && tileMatrix.tiles[x, y].isEmpty &&
                tileMatrix.tiles[x, y].BiomeType != BiomeType.Ocean &&
                tileMatrix.tiles[x, y].BiomeType != BiomeType.River &&
                tileMatrix.tiles[x, y].BiomeType != BiomeType.Beach)
            {
                TextureGenerator.SetPlant(tileMatrix.tiles[x, y], FloraTiles, x, y);
                break;
            }
            else if (counter == counter_limit)
                break;
        }
    }

    public void DrawingObsos()
    {
        TextureGenerator.DestroyCreatures(CreatureGroup);
        TextureGenerator.SetCreatures(CreatureGroup, Obsos);
    }

    public void GenerateCreatures()
    {
        const int counter_limit = 5;
        CreatureGroup = new List<Creature>();
        for (var i = 0; i < CreatureNumbers; i++)
        {
            int counter = 0;
            while (true)
            {
                counter++;
                var x = UnityEngine.Random.Range(0, tileMatrix.width);
                var y = UnityEngine.Random.Range(0, tileMatrix.height);
                if (tileMatrix.tiles[x, y].isEmpty)
                {
                    CreatureGroup.Add(new Creature(x, y));
                    EatPlant(CreatureGroup[i], tileMatrix.tiles[x, y]);
                    break;
                }
                else if (counter == counter_limit)
                    break;
            }
        }
    }

    public void ManageCreatures()
    {
        for (var i = 0; i < CreatureGroup.Count; i++)
        {
            ManageMove(CreatureGroup[i]);
        }

        int j = 0;
        while (j < CreatureGroup.Count)
        {
            if (CreatureGroup[j].Hunger > 1 || CreatureGroup[j].Thirst > 1)
            {
                tileMatrix.tiles[CreatureGroup[j].GetX(), CreatureGroup[j].GetY()].SetEmpty();
                TextureGenerator.DestroyCreature(CreatureGroup[j]);
                TextureGenerator.DestroyExtra(CreatureGroup[j]);
                TextureGenerator.DestroyVision(CreatureGroup[j]);
                CreatureGroup.RemoveAt(j);
                j--;
            }
            j++;
        }
    }

    public void ManageMove(Creature Creature)
    {
        Vector2 vector;
        
        Creature.Health += 0.01f;
        Creature.Hunger += 0.1f;
        Creature.Thirst += 0.05f;

        if (Creature.Health > 1f) Creature.Health = 1f;

        Creature.SetGoal();
        print("0 - Food, 1 - Drink, 2 - Fight");
        print((int) Creature.GoalType);

        TextureGenerator.DestroyExtra(Creature);

        vector = FindGoalTile(Creature);

        TextureGenerator.SetGoal(Creature, (int)vector.x, (int)vector.y, ExtraTiles[0]);

        vector = FindPath(Creature, vector);

        TextureGenerator.SetPath(Creature, (int)Creature.GetX() + (int)vector.x, (int)Creature.GetY() + (int)vector.y, ExtraTiles[1]);

        tileMatrix.tiles[(int)Creature.GetX(), (int)Creature.GetY()].SetEmpty();

        Creature.SetX((int)Creature.GetX() + (int)vector.x);
        Creature.SetY((int)Creature.GetY() + (int)vector.y);

        tileMatrix.tiles[(int)Creature.GetX(), (int)Creature.GetY()].SetOccupied();
        EatPlant(Creature, tileMatrix.tiles[(int)Creature.GetX(), (int)Creature.GetY()]);
        Drink(Creature, tileMatrix.tiles[(int)Creature.GetX(), (int)Creature.GetY()]);

    }

    public Vector2 FindPath(Creature Creature, Vector2 vector)
    {
        double[,] D = new double[3, 3];
        Vector2 MinV = new Vector2(-1f, -1f);

        // Игра падает при нахождении пацана в крайнем состоянии, так что здесь я поправлю

        int left = (Creature.GetX() > 0) ? 0 : 1;
        int right = (Creature.GetX() < tileMatrix.width - 1) ? 3 : 2;
        int top = (Creature.GetY() < tileMatrix.height -  1) ? 3 : 2;
        int bottom = (Creature.GetY() > 0) ? 0 : 1;
        Tile tile;
        int curr_x;
        int curr_y;

        for (var i = left; i < right; i++)
            for (var j = bottom; j < top; j++)
            {
                curr_x = Creature.GetX() + i - 1;
                curr_y = Creature.GetY() + j - 1;
                tile = tileMatrix.tiles[curr_x, curr_y];

                if (i == 1 && j == 1)
                    D[i, j] = -1;
                else
                    D[i, j] = (tile.isEmpty) ?  Distance(vector.x, vector.y, curr_x, curr_y): -1;
            }

        double MinD = -1;

        for (var i = left; i < right; i++)
            for (var j = bottom; j < top; j++)
                if (MinD > D[i, j] && D[i, j] != -1 || MinD == -1)
                {
                    MinD = D[i, j];
                    MinV.x = i - 1;
                    MinV.y = j - 1;
                }
        return MinV;
    }

    public Vector2 FindGoalTile(Creature Creature)
    {
        int xc = Creature.GetX(), yc = Creature.GetY();

        int left = (Creature.VisionRange > xc) ? 0 : xc - Creature.VisionRange;
        int right = (xc + Creature.VisionRange > tileMatrix.width - 1) ? tileMatrix.width - 1 : xc + Creature.VisionRange;
        int top = (yc + Creature.VisionRange > tileMatrix.height - 1) ? tileMatrix.height - 1 : yc + Creature.VisionRange;
        int bottom = (Creature.VisionRange > yc) ? 0 : yc - Creature.VisionRange;

        double MinD = Creature.VisionRange, MaxD = 0, D;
        Vector2 MinV = new Vector2(xc, yc), MaxV = new Vector2(xc, yc);

        TextureGenerator.DestroyVision(Creature);
        TextureGenerator.SetVision(Creature, left, bottom, right, top, ExtraTiles[2]);

        switch (Creature.GoalType)
        {
            case GoalType.Food:
                for (var x = left; x <= right; x++)
                    for (var y = bottom; y <= top; y++)
                        if (tileMatrix.tiles[x, y].isPlant)
                        {
                            D = Distance(x, y, xc, yc);
                            if (D > MaxD)
                            {
                                MaxD = D;
                                MaxV.x = x;
                                MaxV.y = y;
                            }
                            if (D < MinD)
                            {
                                MinD = D;
                                MinV.x = x;
                                MinV.y = y;
                            }
                        }
                break;
            case GoalType.Water:
                for (var x = left; x <= right; x++)
                    for (var y = bottom; y <= top; y++)
                        if (tileMatrix.tiles[x, y].BiomeType == BiomeType.River)
                        {
                        D = Distance(x, y, xc, yc);
                        if (D > MaxD)
                            {
                                MaxD = D;
                                MaxV.x = x;
                                MaxV.y = y;
                            }
                            if (D < MinD)
                            {
                                MinD = D;
                                MinV.x = x;
                                MinV.y = y;
                            }
                        }
                if (MaxD == 0) // Если воды нет, то можно поискать растения, чтобы хоть немного напиться
                {
                    for (var x = left; x <= right; x++)
                        for (var y = bottom; y <= top; y++)
                            if (tileMatrix.tiles[x, y].isPlant)
                            {
                                D = Distance(x, y, xc, yc);
                                if (D > MaxD)
                                {
                                    MaxD = D;
                                    MaxV.x = x;
                                    MaxV.y = y;
                                }
                                if (D < MinD)
                                {
                                    MinD = D;
                                    MinV.x = x;
                                    MinV.y = y;
                                }
                            }
                }
                break;
            case GoalType.Fight:
                for (var x = left; x <= right; x++)
                    for (var y = bottom; y <= top; y++)
                        if (!tileMatrix.tiles[x, y].isEmpty && tileMatrix.tiles[x, y].BiomeType == BiomeType.Ocean)
                        {
                            D = Distance(x, y, xc, yc);
                            if (D > MaxD)
                            {
                                MaxD = D;
                                MaxV.x = x;
                                MaxV.y = y;
                            }
                            if (D < MinD)
                            {
                                MinD = D;
                                MinV.x = x;
                                MinV.y = y;
                            }
                        }
                break;
        }


        if (Distance(MaxV.x, MaxV.y, xc, yc) == 0)
        {
            DirectionType dt;
            int x = Creature.GetX();
            int y = Creature.GetY();
            int counter = 0;
            const int counter_limit = 20;

            while (true)
            {
                counter++;
                dt = Creature.ChooseDirection();
                MaxV = tileMatrix.ConvertDirection(x, y, dt);
                if (tileMatrix.tiles[(int)MaxV.x, (int)MaxV.y].isEmpty)
                    break;
                else if (counter == counter_limit)
                {
                    return new Vector2(0, 0);
                }
            }
            return MaxV;
        }
        if (Creature.Intelligence > UnityEngine.Random.value + 0.3f) // Пускай шанс будет низок.
            return MaxV;
        else
            return MinV;
    }
    

    public double Distance(int x1, int y1, int x2, int y2)
    {
        return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    public double Distance(double x1, double y1, int x2, int y2)
    {
        return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }

    public void EatPlant(Creature creature, Tile tile)
    {
        if (tile.isPlant)
        {
            TextureGenerator.DestroyPlant(tile);
            creature.Hunger -= ((int)tile.PlantType + 1) * (UnityEngine.Random.value * 0.02f + 0.04f);
            creature.Thirst -= ((int)tile.PlantType + 1) * (UnityEngine.Random.value * 0.01f + 0.01f);
            tile.isPlant = false;
        }

        if (creature.Hunger < 0) creature.Hunger = 0f;
        if (creature.Thirst < 0) creature.Thirst = 0f;
    }

    public void Drink(Creature creature, Tile tile)
    {
        if (tile.BiomeType == BiomeType.River)
            creature.Thirst -= 0.5f;
        if (creature.Thirst < 0) creature.Thirst = 0f;
    }
}
