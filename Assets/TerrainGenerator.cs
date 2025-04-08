using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;
    public AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;

    [Range(1,10000)]
    public int xSize = 500;
    [Range(1, 10000)]
    public int zSize = 500;

    [Range(1,10)]
    public int terrainResolution = 1;

    public float perlinScale = 50;

    [Tooltip("An octave is one layer of perlin Noise")]
    [Range(1,10)]
    public int octaves;
    [Tooltip("How much smaller the next octave will be")]
    [Range(1.5f,10f)]
    public float lacunarity;
    public int seed;

    [Range(0.01f,50f)]
    public float heightMultiplier = 1;

    [Tooltip("How the different octaves of perlin noise are translated in x and y direction")]
    private Vector2[] octaveOffsets;


    [Space(10)]
    public bool changeSeedOnReload = true;

    private void Awake()
    {
        CreateMeshVar();
        CreateNewMap();
    }

    private void CreateMeshVar()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        octaveOffsets = GetOffsetSeed();
    }

    public void CreateNewMap()
    {
        if(!mesh)
        {
            CreateMeshVar();
        }
        CreateMeshShape();
        CreateTriangles();
        UpdateMesh();
    }

    private void CreateMeshShape()
    {
        // Creates seed
        if(changeSeedOnReload)
        {
            octaveOffsets = GetOffsetSeed();
        }

        if (perlinScale <= 0)
            perlinScale = 0.0001f;

        // Create vertices array
        vertices = new Vector3[(xSize + 1) * (zSize + 1) * terrainResolution * terrainResolution];

        for (int i = 0, z = 0; z <= zSize * terrainResolution; z++)
        {
            for (int x = 0; x <= xSize * terrainResolution; x++)
            {
                float scaledXCoord = x / (float)terrainResolution;
                float scaledZCoord = z / (float)terrainResolution;
                // Assign and set height of each vertices
                float noiseHeight = GenerateNoiseHeight(scaledZCoord, scaledXCoord, octaveOffsets);
                vertices[i] = new Vector3(scaledXCoord, noiseHeight * heightMultiplier, scaledZCoord) ;
                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        if(changeSeedOnReload)
        {
            seed = Random.Range(0, 1000);
        }
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

    private float GenerateNoiseHeight(float z, float x, Vector2[] octaveOffsets)
    {
        float amplitude = 12;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / perlinScale * frequency + octaveOffsets[y].y;
            float mapX = x / perlinScale * frequency + octaveOffsets[y].x;

            // Create perlinValues  
            // The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }

    private void CreateTriangles()
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6 * terrainResolution * terrainResolution];
        int vert = 0;
        int tris = 0;

        // loop through rows
        for (int z = 0; z < zSize * terrainResolution; z++)
        {
            // fill all columns in row
            for (int x = 0; x < xSize * terrainResolution; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize * terrainResolution + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize * terrainResolution + 1;
                triangles[tris + 5] = vert + xSize * terrainResolution + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
