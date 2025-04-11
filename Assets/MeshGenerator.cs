using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(float[,] heightMap, int terrainResolution, float heightMultiplier, AnimationCurve heightCurve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);

        float topLeftX = (width / (float)terrainResolution - 1) / 2f * -1;
        float topLeftZ = (height / (float)terrainResolution - 1) / 2f;

        int vert = 0;
        int tris = 0;


        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vert] = new Vector3(topLeftX + x / (float)terrainResolution, heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier, topLeftZ - z / (float)terrainResolution);
                meshData.uvs[vert] = new Vector2(x / (float)width, z / (float)height);

                //Add one square of the mesh (two triangles)
                if(x < width-1 && z < height-1)
                {
                    meshData.triangles[tris + 0] = vert;
                    meshData.triangles[tris + 1] = vert + width + 1;
                    meshData.triangles[tris + 2] = vert + width;
                    meshData.triangles[tris + 3] = vert + width + 1;
                    meshData.triangles[tris + 4] = vert;
                    meshData.triangles[tris + 5] = vert + 1;

                    tris += 6;
                }

                vert++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }
}
