using UnityEngine;

public static class TerrainTextureGenerator
{
    public static Texture2D GenerateTexture(float[,] noiseMap)
    {
        return GenerateColorLerpTexture(noiseMap, Color.green, Color.black);
    }

    public static Texture2D GenerateBlackWhiteTexture(float[,] noiseMap)
    {
        return GenerateColorLerpTexture(noiseMap, Color.black, Color.white);
    }

    private static Texture2D GenerateColorLerpTexture(float[,] noiseMap, Color minColor, Color maxColor)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[z * width + x] = Color.Lerp(minColor, maxColor, noiseMap[x, z]);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}
