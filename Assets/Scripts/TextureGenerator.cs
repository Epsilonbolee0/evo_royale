using UnityEngine;

public class TextureGenerator : MonoBehaviour {		

	public static void SetGround(int width, int height, Tile[,] tiles, GameObject[] BiomeTiles, GameObject[] HeatTiles)
	{
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				BiomeType biome_type = tiles[x, y].BiomeType;
				Instantiate (BiomeTiles[(int) biome_type], new Vector3(x * 1.28f, y * 1.28f, 0f), Quaternion.identity);

                HeatType heat_type = tiles[x, y].HeatType;
                Instantiate(HeatTiles[(int) heat_type], new Vector3(x * 1.28f, y * 1.28f, -0.1f), Quaternion.identity);
            }
		}
	}

}