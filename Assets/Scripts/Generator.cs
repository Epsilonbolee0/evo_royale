using UnityEngine;
using AccidentalNoise;
using System.Collections.Generic;
public class Generator
{
    // Adjustable variables for Unity Inspector
    int Width = 1;
    int Height = 1;
    int TerrainOctaves = 5;
    double TerrainFrequency = 1.25;
    float Water = 0.14f;
    float Beach = 0.17f;
    float Lowland = 0.27f;
    float Plain = 0.45f;
    float Hill = 0.66f;

    int MoistureOctaves = 4;
    double MoistureFrequency = 3.0;
    float DryerValue = 0.1f;
    float DryValue = 0.2f;
    float WetValue = 0.3f;
    float WetterValue = 0.4f;
    float WettestValue = 0.5f;

    int RiverCount = 10;
    float MinRiverHeight = 0.6f;
    int MaxRiverAttempts = 1000;
    int MinRiverTurns = 20;
    int MinRiverLength = 20;
    int MaxRiverIntersections = 1;

    int HeatOctaves = 4;
    double HeatFrequency = 3.0;

    public TileMatrix tileMatrix;
    public Biome[] Biomes;

    // private variables
    ImplicitFractal HeightMap, MoistureMap, HeatMap;
    MapData HeightData, MoistureData, HeatData;
    GenerationTile[,] Tiles;

    List<TileGroup> Waters = new List<TileGroup>();
    List<TileGroup> Lands = new List<TileGroup>();

    List<River> Rivers = new List<River>();
    List<RiverGroup> RiverGroups = new List<RiverGroup>();

    public Generator(GameObject[] BiomeTiles, GameObject[] FloraTiles)
    {
        Initialize();
        GetData (HeightMap, HeightMap, ref HeightData);
        GetData (MoistureMap, MoistureMap, ref MoistureData);
        GetData (HeatMap, HeatMap, ref HeatData);
        GenerateMap();

        tileMatrix = TransferData();

        TextureGenerator.SetGround(tileMatrix, BiomeTiles);
        TextureGenerator.SetFlora(tileMatrix, FloraTiles, Biomes);
    }

    private TileMatrix TransferData()
    {
        TileMatrix tileMatrix = new TileMatrix(Width, Height);
        tileMatrix.CopyMatrix(Tiles);

        HeightData = null;
        MoistureData = null;
        HeatData = null;

        HeightMap = null;
        MoistureMap = null;
        HeatMap = null;

        Tiles = null;
        return tileMatrix;
    }

    private void Initialize()
    {
        // Initialize the HeightMap Generator
        HeightMap = new ImplicitFractal(FractalType.MULTI,
                                       BasisType.SIMPLEX,
                                       InterpolationType.QUINTIC,
                                       TerrainOctaves,
                                       TerrainFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));

        MoistureMap = new ImplicitFractal(FractalType.MULTI,
                                       BasisType.SIMPLEX,
                                       InterpolationType.QUINTIC,
                                       MoistureOctaves,
                                       MoistureFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));
        HeatMap = new ImplicitFractal(FractalType.MULTI,
                                       BasisType.SIMPLEX,
                                       InterpolationType.QUINTIC,
                                       HeatOctaves,
                                       HeatFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));
    }

    // Extract data from a noise module
    private void GetData(ImplicitModuleBase module, ImplicitFractal Map, ref MapData mapData)
    {
        mapData = new MapData(Width, Height);

        // циклично проходим по каждой точке x,y - получаем значение высоты
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                //Сэмплируем шум с небольшими интервалами
                float x1 = x / (float)Width;
                float y1 = y / (float)Height;

                float value = (float)Map.Get(x1, y1);

                //отслеживаем максимальные и минимальные найденные значения
                if (value > mapData.Max) mapData.Max = value;
                if (value < mapData.Min) mapData.Min = value;

                mapData.Data[x, y] = value;
            }
        }
    }

    private GenerationTile GetTop(GenerationTile t)
    {
        return Tiles[t.X, MathHelper.Mod(t.Y - 1, Height)];
    }
    private GenerationTile GetBottom(GenerationTile t)
    {
        return Tiles[t.X, MathHelper.Mod(t.Y + 1, Height)];
    }
    private GenerationTile GetLeft(GenerationTile t)
    {
        return Tiles[MathHelper.Mod(t.X - 1, Width), t.Y];
    }
    private GenerationTile GetRight(GenerationTile t)
    {
        return Tiles[MathHelper.Mod(t.X + 1, Width), t.Y];
    }

    private void GenerateMap()
    {
        Tiles = new GenerationTile[Width, Height];
        GenerateTiles();

        GenerateHeatMap();
        GenerateHeightMap();

        UpdateNeighbors();
        GenerateRivers();
        BuildRiverGroups();
        DigRiverGroups();

        GenerateMoistureMap();
        AdjustMoistureMap();
        SetMoistureTypes();
        SetBiomes();

        GenerateFlora();
    }

    private void GenerateFlora()
    {
        float sum;
        float rand;

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = Tiles[x, y];
                t.Fertility = t.CalcFertility();

                rand = UnityEngine.Random.value;

                if (t.Fertility > 0.5f)
                    rand += 0.05f * t.Fertility;
                else 
                    rand -= 0.05f * t.Fertility;

                if (rand > 1) rand = 1;
                if (rand < 0) rand = 0;

                sum = 0f;

                for (var i = 0; i < 5; i++)
                {
                    sum += Biomes[(int)t.BiomeType].FloraChance[i];
                    if (rand < sum)
                    { 
                        t.FloraType = (FloraType)(i);
                        break;
                    }
                }

                Tiles[x, y] = t;

                switch(t.FloraType)
                {
                    case (FloraType.Grass):
                        if (t.BiomeType == BiomeType.Swamp || t.BiomeType == BiomeType.Savannah)
                            t.PlantType = PlantType.HighGrass;
                        else
                            t.PlantType = PlantType.Grass;
                        break;
                    case (FloraType.Bush):
                        if (t.BiomeType == BiomeType.Desert || t.BiomeType == BiomeType.Steppe)
                            t.PlantType = PlantType.Prickle;
                        else
                            t.PlantType = PlantType.Berries;
                        break;
                    case (FloraType.Shrub):
                        if (t.BiomeType == BiomeType.Taiga)
                            t.PlantType = PlantType.DryBush;
                        else 
                            t.PlantType = PlantType.YoungBush;
                        break;
                    case (FloraType.Tree):
                        if (t.BiomeType == BiomeType.Taiga || t.BiomeType == BiomeType.Hills)
                            t.PlantType = PlantType.Pine;
                        else if (t.BiomeType == BiomeType.Forest)
                            t.PlantType = PlantType.Birch;
                        else if (t.BiomeType == BiomeType.Jungle)
                            t.PlantType = PlantType.Citrus;
                        else
                            t.PlantType = PlantType.Acacia;
                        break;
                    case (FloraType.Special):
                        if (t.BiomeType == BiomeType.Tundra)
                            t.PlantType = PlantType.Birch;
                        else if (t.BiomeType == BiomeType.Plains)
                            t.PlantType = PlantType.Flower;
                        else if (t.BiomeType == BiomeType.Forest)
                            t.PlantType = PlantType.Oak;
                        else if (t.BiomeType == BiomeType.Jungle)
                            t.PlantType = PlantType.Palm;
                        else if (t.BiomeType == BiomeType.Swamp)
                            t.PlantType = PlantType.Willow;
                        else
                            t.PlantType = PlantType.Cactus;
                        break;
                }
                Tiles[x, y] = t;
            }
    }

    private void GenerateTiles()
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = new GenerationTile();
                t.X = x;
                t.Y = y;
                Tiles[x, y] = t;
            }
    }

    private void GenerateHeatMap()
    {
        Biomes = new Biome[14];

        for (var i = 0; i < 14; i++)
        {
            Biome tmp = new Biome();
            tmp.InitConsts(i);

            Biomes[i] = tmp;
        }

        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                float heat = HeatData.Data[x, y];
                heat = (heat - HeatData.Min) / (HeatData.Max - HeatData.Min);

                Tiles[x, y].SetHeat(heat, Biomes[(int)(Tiles[x, y].BiomeType)]);
            }
    }

    // Build a Tile array from our data
    private void GenerateHeightMap()
    {
        HeightType height_type;

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                float height = HeightData.Data[x, y];
                height = (height - HeightData.Min) / (HeightData.Max - HeightData.Min);

                //HeightMap Analyze
                if (height < Water)
                {
                    height_type = HeightType.Ocean;
                    Tiles[x, y].Collidable = false;
                }
                else if (height < Beach)
                {
                    height_type = HeightType.Beach;
                    Tiles[x, y].Collidable = true;
                }
                else if (height < Lowland)
                {
                    height_type = HeightType.Lowland;
                    Tiles[x, y].Collidable = true;
                }
                else if (height < Plain)
                {
                    height_type = HeightType.Plain;
                    Tiles[x, y].Collidable = true;
                }
                else if (height < Hill)
                {
                    height_type = HeightType.Hill;
                    Tiles[x, y].Collidable = true;
                }
                else
                {
                    height_type = HeightType.Mountain;
                    Tiles[x, y].Collidable = true;
                }

                Tiles[x, y].HeightType = height_type;
                Tiles[x, y].HeightValue = height;
            }
        }
    }
    private void GenerateMoistureMap()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                float moisture = MoistureData.Data[x, y];
                moisture = (moisture - MoistureData.Min) / (MoistureData.Max - MoistureData.Min);
                Tiles[x, y].MoistureValue = moisture;
            }
        }
    }
    private void AddMoisture(GenerationTile t, int radius)
    {
        int startx = MathHelper.Mod(t.X - radius, Width);
        int endx = MathHelper.Mod(t.X + radius, Width);
        Vector2 center = new Vector2(t.X, t.Y);
        int curr = radius;

        while (curr > 0)
        {

            int x1 = MathHelper.Mod(t.X - curr, Width);
            int x2 = MathHelper.Mod(t.X + curr, Width);
            int y = t.Y;

            AddMoisture(Tiles[x1, y], 0.02f / (center - new Vector2(x1, y)).magnitude);

            for (int i = 0; i < curr; i++)
            {
                AddMoisture(Tiles[x1, MathHelper.Mod(y + i + 1, Height)], 0.02f / (center - new Vector2(x1, MathHelper.Mod(y + i + 1, Height))).magnitude);
                AddMoisture(Tiles[x1, MathHelper.Mod(y - (i + 1), Height)], 0.02f / (center - new Vector2(x1, MathHelper.Mod(y - (i + 1), Height))).magnitude);

                AddMoisture(Tiles[x2, MathHelper.Mod(y + i + 1, Height)], 0.02f / (center - new Vector2(x2, MathHelper.Mod(y + i + 1, Height))).magnitude);
                AddMoisture(Tiles[x2, MathHelper.Mod(y - (i + 1), Height)], 0.02f / (center - new Vector2(x2, MathHelper.Mod(y - (i + 1), Height))).magnitude);
            }
            curr--;
        }
    }

    private void AddMoisture(GenerationTile t, float amount)
    {
        t.MoistureValue += amount;
        if (t.MoistureValue > 1)
            t.MoistureValue = 1;
    }
    private void SetMoistureTypes()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = Tiles[x, y];
                if (t.MoistureValue < DryerValue) t.MoistureType = MoistureType.Dryest;
                else if (t.MoistureValue < DryValue) t.MoistureType = MoistureType.Dryer;
                else if (t.MoistureValue < WetValue) t.MoistureType = MoistureType.Dry;
                else if (t.MoistureValue < WetterValue) t.MoistureType = MoistureType.Wet;
                else if (t.MoistureValue < WettestValue) t.MoistureType = MoistureType.Wetter;
                else t.MoistureType = MoistureType.Wettest;
                Tiles[x, y] = t;
            }
        }
    }

    private void AdjustMoistureMap()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = Tiles[x, y];
                if (t.HeightType == HeightType.River)
                {
                    AddMoisture(t, (int)3);
                }
            }
        }
    }

    private void DigRiverGroups()
    {
        for (int i = 0; i < RiverGroups.Count; i++)
        {
            RiverGroup group = RiverGroups[i];
            River longest = null;

            //Find longest river in this group
            for (int j = 0; j < group.Rivers.Count; j++)
            {
                River river = group.Rivers[j];
                if (longest == null)
                    longest = river;
                else if (longest.Tiles.Count < river.Tiles.Count)
                    longest = river;
            }

            if (longest != null)
            {
                //Dig out longest path first
                DigRiver(longest);

                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (river != longest)
                    {
                        DigRiver(river, longest);
                    }
                }
            }
        }
    }

    private void BuildRiverGroups()
    {
        //loop each tile, checking if it belongs to multiple rivers
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = Tiles[x, y];

                if (t.Rivers.Count > 1)
                {
                    // multiple rivers == intersection
                    RiverGroup group = null;

                    // Does a rivergroup already exist for this group?
                    for (int n = 0; n < t.Rivers.Count; n++)
                    {
                        River tileriver = t.Rivers[n];
                        for (int i = 0; i < RiverGroups.Count; i++)
                        {
                            for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
                            {
                                River river = RiverGroups[i].Rivers[j];
                                if (river.ID == tileriver.ID)
                                {
                                    group = RiverGroups[i];
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }
                        if (group != null) break;
                    }

                    // existing group found -- add to it
                    if (group != null)
                    {
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            if (!group.Rivers.Contains(t.Rivers[n]))
                                group.Rivers.Add(t.Rivers[n]);
                        }
                    }
                    else   //No existing group found - create a new one
                    {
                        group = new RiverGroup();
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            group.Rivers.Add(t.Rivers[n]);
                        }
                        RiverGroups.Add(group);
                    }
                }
            }
        }
    }

    private void GenerateRivers()
    {
        int attempts = 0;
        int rivercount = RiverCount;
        Rivers = new List<River>();

        // Generate some rivers
        while (rivercount > 0 && attempts < MaxRiverAttempts)
        {

            // Get a random tile
            int x = UnityEngine.Random.Range(0, Width);
            int y = UnityEngine.Random.Range(0, Height);
            GenerationTile tile = Tiles[x, y];

            // validate the tile
            if (!tile.Collidable) continue;
            if (tile.Rivers.Count > 0) continue;

            if (tile.HeightValue > MinRiverHeight)
            {
                // Tile is good to start river from
                River river = new River(rivercount);

                // Figure out the direction this river will try to flow
                river.CurrentDirection = tile.GetLowestNeighbor();

                // Recursively find a path to water
                FindPathToWater(tile, river.CurrentDirection, ref river);

                // Validate the generated river 
                if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
                {
                    //Validation failed - remove this river
                    for (int i = 0; i < river.Tiles.Count; i++)
                    {
                        GenerationTile t = river.Tiles[i];
                        t.Rivers.Remove(river);
                    }
                }
                else if (river.Tiles.Count >= MinRiverLength)
                {
                    //Validation passed - Add river to list
                    Rivers.Add(river);
                    tile.Rivers.Add(river);
                    rivercount--;
                }
            }
            attempts++;
        }
    }

    // Dig river based on a parent river vein
    private void DigRiver(River river, River parent)
    {
        int intersectionID = 0;
        int intersectionSize = 0;

        // determine point of intersection
        for (int i = 0; i < river.Tiles.Count; i++)
        {
            GenerationTile t1 = river.Tiles[i];
            for (int j = 0; j < parent.Tiles.Count; j++)
            {
                GenerationTile t2 = parent.Tiles[j];
                if (t1 == t2)
                {
                    intersectionID = i;
                    intersectionSize = t2.RiverSize;
                }
            }
        }

        int counter = 0;
        int intersectionCount = river.Tiles.Count - intersectionID;
        int size = UnityEngine.Random.Range(intersectionSize, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize length of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }

        // adjust size of river at intersection point
        if (intersectionSize == 1)
        {
            count4 = intersectionCount;
            count1 = 0;
            count2 = 0;
            count3 = 0;
        }
        else if (intersectionSize == 2)
        {
            count3 = intersectionCount;
            count1 = 0;
            count2 = 0;
        }
        else if (intersectionSize == 3)
        {
            count2 = intersectionCount;
            count1 = 0;
        }
        else if (intersectionSize == 4)
        {
            count1 = intersectionCount;
        }
        else
        {
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
        }

        // dig out the river
        for (int i = river.Tiles.Count - 1; i >= 0; i--)
        {

            GenerationTile t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }

    // Dig river
    private void DigRiver(River river)
    {
        int counter = 0;

        // How wide are we digging this river?
        int size = UnityEngine.Random.Range(1, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize lenght of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }

        // Dig it out
        for (int i = river.Tiles.Count - 1; i >= 0; i--)
        {
            GenerationTile t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }

    private void FindPathToWater(GenerationTile tile, Direction direction, ref River river)
    {
        if (tile.Rivers.Contains(river))
            return;

        // check if there is already a river on this tile
        if (tile.Rivers.Count > 0)
            river.Intersections++;

        river.AddTile(tile);

        // get neighbors
        GenerationTile left = GetLeft(tile);
        GenerationTile right = GetRight(tile);
        GenerationTile top = GetTop(tile);
        GenerationTile bottom = GetBottom(tile);

        float leftValue = int.MaxValue;
        float rightValue = int.MaxValue;
        float topValue = int.MaxValue;
        float bottomValue = int.MaxValue;

        // query height values of neighbors
        if (left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
            leftValue = left.HeightValue;
        if (right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
            rightValue = right.HeightValue;
        if (top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
            topValue = top.HeightValue;
        if (bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
            bottomValue = bottom.HeightValue;

        // if neighbor is existing river that is not this one, flow into it
        if (bottom.Rivers.Count == 0 && !bottom.Collidable)
            bottomValue = 0;
        if (top.Rivers.Count == 0 && !top.Collidable)
            topValue = 0;
        if (left.Rivers.Count == 0 && !left.Collidable)
            leftValue = 0;
        if (right.Rivers.Count == 0 && !right.Collidable)
            rightValue = 0;

        // override flow direction if a tile is significantly lower
        if (direction == Direction.Left)
            if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                rightValue = int.MaxValue;
        if (direction == Direction.Right)
            if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                leftValue = int.MaxValue;
        if (direction == Direction.Top)
            if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                bottomValue = int.MaxValue;
        if (direction == Direction.Bottom)
            if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                topValue = int.MaxValue;

        // find mininum
        float min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

        // if no minimum found - exit
        if (min == int.MaxValue)
            return;

        //Move to next neighbor
        if (min == leftValue)
        {
            if (left.Collidable)
            {
                if (river.CurrentDirection != Direction.Left)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Left;
                }
                FindPathToWater(left, direction, ref river);
            }
        }
        else if (min == rightValue)
        {
            if (right.Collidable)
            {
                if (river.CurrentDirection != Direction.Right)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Right;
                }
                FindPathToWater(right, direction, ref river);
            }
        }
        else if (min == bottomValue)
        {
            if (bottom.Collidable)
            {
                if (river.CurrentDirection != Direction.Bottom)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Bottom;
                }
                FindPathToWater(bottom, direction, ref river);
            }
        }
        else if (min == topValue)
        {
            if (top.Collidable)
            {
                if (river.CurrentDirection != Direction.Top)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Top;
                }
                FindPathToWater(top, direction, ref river);
            }
        }
    }
    private void UpdateNeighbors()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                GenerationTile t = Tiles[x, y];

                t.Top = GetTop(t);
                t.Bottom = GetBottom(t);
                t.Left = GetLeft(t);
                t.Right = GetRight(t);
            }
        }
    }

    private void SetBiomes()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
                Tiles[x, y].SetBiome();
        }
    }
}
