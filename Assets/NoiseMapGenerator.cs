using UnityEngine;

public static class NoiseMapGenerator 
{
    public static float[,] GeneratePerlinNoiseMap(int xSize, int zSize, int seed, int terrainResolution, float noiseScale, int octaves, float lacunarity, float persistance)
    {
        float[,] noiseMap = new float[xSize * terrainResolution, zSize * terrainResolution];

        Vector2[] octaveOffsets = GetOffsetSeed(seed, octaves);

        if (noiseScale <= 0)
            noiseScale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int z = 0; z < noiseMap.GetLength(1); z++)
            {
                float scaledXCoord = x / (float)terrainResolution;
                float scaledZCoord = z / (float)terrainResolution;

                // Assign and set height of each data point
                float noiseHeight = GenerateNoiseHeight(scaledXCoord, scaledZCoord, octaveOffsets, noiseScale, lacunarity, persistance);

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, z] = noiseHeight;

            }
        }

        //now normalize the NoiseMap
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int z = 0; z < noiseMap.GetLength(1); z++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }

        return noiseMap;
    }

    private static Vector2[] GetOffsetSeed(int seed, int octaves)
    {
        // changes area of map

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int o = 0; o < octaves; o++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private static float GenerateNoiseHeight(float x, float z, Vector2[] octaveOffsets, float noiseScale, float lacunarity, float persistance)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        // loop over octaves
        for (int i = 0; i < octaveOffsets.Length; i++)
        {
            float mapX = x / noiseScale * frequency + octaveOffsets[i].x;
            float mapZ = z / noiseScale * frequency + octaveOffsets[i].y;

            // Create perlinValues  
            // The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapX, mapZ)) * 2 - 1;
            //noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            noiseHeight += perlinValue * amplitude;
            frequency *= lacunarity;
            amplitude *= persistance;
        }
        return noiseHeight;
    }
}
