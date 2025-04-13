using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    public enum GeneratingType {LayeredPerlinNoise};
    public GeneratingType generatingType;
    public enum DisplayMode {Plane,Terrain};
    public DisplayMode displayMode;
    public enum TextureMode {HeightMap, ColoredLerp};
    public TextureMode textureMode;

    Mesh mesh;
    public AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;

    const int CHUNK_SIZE = 240;

    [Range(0,7)]
    public int levelOfDetail = 0;

    [Range(20,10000)]
    public float noiseScale = 400;

    [Tooltip("An octave is one layer of perlin Noise")]
    [Range(1,10)]
    public int octaves;
    [Tooltip("How much smaller the next octave will be")]
    [Range(1.5f,10f)]
    public float lacunarity;
    public float persistance = 0.5f;
    public int seed;

    [Range(1f,1000f)]
    public float heightMultiplier = 1;
    public bool normalizeChunk = true;

    [Space(10)]
    public bool changeSeedOnReload = true;

    private void Awake()
    {
        CreateNewMap();
    }


    public void CreateNewMap()
    {
        if(changeSeedOnReload)
        {
            seed = Random.Range(0, 1000);
        }

        float[,] noiseMap = NoiseMapGenerator.GeneratePerlinNoiseMap(CHUNK_SIZE, CHUNK_SIZE, seed, noiseScale, octaves, lacunarity, persistance, normalizeChunk);
        Texture2D texture = null;
        MeshData meshData = null;

        switch(displayMode)
        {
            case DisplayMode.Plane:
                meshData = MeshGenerator.GenerateMeshData(noiseMap, levelOfDetail, 0, heightCurve);
                break;
            case DisplayMode.Terrain:
                meshData = MeshGenerator.GenerateMeshData(noiseMap, levelOfDetail, heightMultiplier, heightCurve);
                break;
        }

        switch(textureMode)
        {
            case TextureMode.HeightMap:
                texture = TerrainTextureGenerator.GenerateBlackWhiteTexture(noiseMap);
                break;
            case TextureMode.ColoredLerp:
                texture = TerrainTextureGenerator.GenerateTexture(noiseMap);
                break;
        }

        GetComponentInChildren<LandscapeDisplayer>().DrawMesh(meshData, texture);
    }




}
