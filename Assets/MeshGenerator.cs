using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshData(float[,] heightMap, int levelOfDetail, float heightMultiplier, AnimationCurve heightCurve)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        if(width != height)
        {
            Debug.LogError("Height Map is not square, returning null as MeshData");
            return null;
        }

        int verticesStepSize = (levelOfDetail == 0) ? 1 : ((levelOfDetail == 7) ? 16 : levelOfDetail * 2);
        int verticesPerLine = (width - 1) / verticesStepSize + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);

        float topLeftX = (width - 1) / 2f * -1;
        float topLeftZ = (height - 1) / 2f;

        int vert = 0;
        int tris = 0;


        for (int z = 0; z < height; z += verticesStepSize)
        {
            for (int x = 0; x < width; x += verticesStepSize)
            {
                meshData.vertices[vert] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier, topLeftZ - z);
                meshData.uvs[vert] = new Vector2(x / (float)width, z / (float)height);

                //Add one square of the mesh (two triangles)
                if(x < width-1 && z < height-1)
                {
                    meshData.triangles[tris + 0] = vert;
                    meshData.triangles[tris + 1] = vert + verticesPerLine + 1;
                    meshData.triangles[tris + 2] = vert + verticesPerLine;
                    meshData.triangles[tris + 3] = vert + verticesPerLine + 1;
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
