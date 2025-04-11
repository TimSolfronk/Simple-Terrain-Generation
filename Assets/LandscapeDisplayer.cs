using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LandscapeDisplayer : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture, int terrainResolution)
    {
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(texture.width / (float)terrainResolution, 1, texture.height / (float)terrainResolution);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(1, 1, 1);
    }
}
