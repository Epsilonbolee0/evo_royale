using UnityEngine;
using AccidentalNoise;

public class Generator : MonoBehaviour {

	// Adjustable variables for Unity Inspector
	int Width = 150;
	int Height = 150;
	[SerializeField]
	int TerrainOctaves = 5;
	[SerializeField]
	double TerrainFrequency = 1.25;
	float Water = 0.14f;
    float Beach = 0.17f;
    float Lowland = 0.27f;
	float Plain = 0.45f;
	float Hill = 0.66f;

	[SerializeField]
	int MoistureOctaves = 4;
	[SerializeField]
	double MoistureFrequency = 3.0;
	float DryerValue = 0.1f;
	float DryValue = 0.2f;
	float WetValue = 0.3f;
	float WetterValue = 0.4f;
	float WettestValue = 0.5f;

    [SerializeField]
    int HeatOctaves = 4;
    [SerializeField]
    double HeatFrequency = 3.0;

    public GameObject[] BiomeTiles;
    public GameObject[] HeatTiles;
    public GameObject RiverTile;

    // private variables
    ImplicitFractal HeightMap, MoistureMap, HeatMap;
	MapData HeightData, MoistureData, HeatData;
	Tile[,] Tiles;
    HeatConst[] HeatConsts;

    // Our texture output gameobject
    MeshRenderer HeightMapRenderer;

	void Start()
	{
		Initialize ();
		GetData (HeightMap, HeightMap, ref HeightData);
        GetData (MoistureMap, MoistureMap, ref MoistureData);
        GetData (HeatMap, HeatMap, ref HeatData);
        LoadTiles ();
		TextureGenerator.SetGround(Height, Width, Tiles, BiomeTiles, HeatTiles);
	}

	private void Initialize()
	{
        // Initialize the HeightMap Generator
        HeightMap = new ImplicitFractal (FractalType.MULTI, 
		                               BasisType.SIMPLEX, 
		                               InterpolationType.QUINTIC, 
		                               TerrainOctaves, 
		                               TerrainFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));

		MoistureMap = new ImplicitFractal (FractalType.MULTI,
									   BasisType.SIMPLEX,
									   InterpolationType.QUINTIC,
									   MoistureOctaves,
									   MoistureFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));

        HeatMap = new ImplicitFractal(FractalType.MULTI,
                                       BasisType.SIMPLEX,
                                       InterpolationType.QUINTIC,
                                       MoistureOctaves,
                                       MoistureFrequency,
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

    // Build a Tile array from our data
    private void LoadTiles()
    {
        Tiles = new Tile[Width, Height];
        HeatConsts = new HeatConst[13];
        HeightType height_type;
        MoistureType moisture_type;

        for (var i = 0; i < 13; i++)
        {
            HeatConst HC = new HeatConst();
            HC.InitConsts(i + 1);

            HeatConsts[i] = HC;
        }


        for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				Tile t = new Tile();
				t.X = x;
				t.Y = y;

                float height = HeightData.Data[x, y];
				height = (height - HeightData.Min) / (HeightData.Max - HeightData.Min);

				//HeightMap Analyze
				if (height < Water)  {
					height_type = HeightType.Water;
				}
                else if (height < Beach)
                {
                    height_type = HeightType.Beach;
                }
                else if (height < Lowland) {
					height_type = HeightType.Lowland;
				}
				else if (height < Plain) {
					height_type = HeightType.Plain;
				}
				else if (height < Hill) {
					height_type = HeightType.Hill;
				}
				else
					height_type = HeightType.Mountain;

				float moisture = MoistureData.Data[x, y];
				moisture = (moisture - MoistureData.Min) / (MoistureData.Max - MoistureData.Min);

                if (moisture < DryerValue) moisture_type = MoistureType.Dryest;
				else if (moisture < DryValue) moisture_type = MoistureType.Dryer;
				else if (moisture < WetValue) moisture_type = MoistureType.Dry;
				else if (moisture < WetterValue) moisture_type = MoistureType.Wet;
				else if (moisture < WettestValue) moisture_type = MoistureType.Wetter;
				else moisture_type = MoistureType.Wettest;

                t.SetBiome(moisture_type, height_type);

                float heat = HeatData.Data[x, y];
                heat = (heat - HeatData.Min) / (HeatData.Max - HeatData.Min);

                t.SetHeat(heat, HeatConsts[(int)t.BiomeType]);

                Tiles[x, y] = t;
            }
		}
	}
}
