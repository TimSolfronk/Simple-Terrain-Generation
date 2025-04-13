using UnityEngine;

public static class NoiseMapGenerator 
{
    const float BASE_AMPLITUDE = 1f;
    const float BASE_FREQUENCY = 1f;



    //This uses Unities default Perlin Noise
    //TODO: add improved perlin noise to not get grid artifacts when setting Offsets to high numbers, while having Noise Scale on something high
    public static float[,] GeneratePerlinNoiseMap(int xChunkSize, int zChunkSize, int seed, float noiseScale, int octaves, float lacunarity, float persistance, Vector2 chunkOffset, Vector2[] octaveOffsets, bool normalized)
    {
        float[,] noiseMap = new float[xChunkSize + 1, zChunkSize + 1];

        if (noiseScale <= 0)
            noiseScale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for (int z = 0; z < noiseMap.GetLength(1); z++)
            {

                // Assign and set height of each data point
                float noiseHeight = GenerateNoiseHeight(x, z, chunkOffset, octaveOffsets, noiseScale, lacunarity, persistance);

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

        if(!normalized)
        {
            maxNoiseHeight = MinOrMaxNoiseHeight(octaves, lacunarity, persistance, true);
            minNoiseHeight = MinOrMaxNoiseHeight(octaves, lacunarity, persistance, false);
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

    public static Vector2[] GetOffsetSeed(int seed, int octaves)
    {
        // changes area of map

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int o = 0; o < octaves; o++)
        {
            float offsetX = prng.Next(-1000, 1000);
            float offsetY = prng.Next(-1000, 1000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private static float GenerateNoiseHeight(float x, float z, Vector2 chunkOffset, Vector2[] octaveOffsets, float noiseScale, float lacunarity, float persistance)
    {
        float amplitude = BASE_AMPLITUDE;
        float frequency = BASE_FREQUENCY;
        float noiseHeight = 0;

        // loop over octaves
        for (int i = 0; i < octaveOffsets.Length; i++)
        {
            float mapX = (x + chunkOffset.x) / noiseScale * frequency + octaveOffsets[i].x;
            float mapZ = (z + chunkOffset.y) / noiseScale * frequency + octaveOffsets[i].y;

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

    private static float MinOrMaxNoiseHeight(int octaveAmount, float lacunarity, float persistance, bool max)
    {
        float amplitude = BASE_AMPLITUDE;
        float frequency = BASE_FREQUENCY;
        float extremeNoiseHeight = 0;

        // loop over octaves
        for (int i = 0; i < octaveAmount; i++)
        { 
            float perlinValue = max ? 1f : -1f;
            extremeNoiseHeight += perlinValue * amplitude;
            frequency *= lacunarity;
            amplitude *= persistance;
        }
        return extremeNoiseHeight;
    }

}
