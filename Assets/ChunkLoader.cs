using UnityEngine;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour
{
    public static ChunkLoader instance;
    private int maxViewDst = 3;
    public Transform player;

    public static Vector2 viewerPosition;

    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> lastFrameChunks = new List<TerrainChunk>();

    private void Awake()
    {
        instance = (ChunkLoader)SingletonCreator.CreateSingleton(instance, this);
    }

    private void Start()
    {
        chunkSize = TerrainGenerationConfig.GetChunkSize();
        chunksVisibleInViewDst = maxViewDst;
    }

    private void Update()
    {
        viewerPosition = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for(int i = 0; i < lastFrameChunks.Count; i++)
        {
            lastFrameChunks[i].SetVisible(false);
        }
        lastFrameChunks.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
            for(int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].isVisible())
                    {
                        lastFrameChunks.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                } else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord));
                }
            }
        }

    }

    public int GetMaxViewDistance()
    {
        return maxViewDst;
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coord)
        {
            position = coord * TerrainGenerationConfig.GetChunkSize();
            bounds = new Bounds(position, Vector2.one * TerrainGenerationConfig.GetChunkSize());
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * TerrainGenerationConfig.GetChunkSize() / 10f;
            meshObject.transform.parent = instance.transform;
            SetVisible(false);
        }

        public void UpdateChunk()
        {
            float minViewerDst = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = minViewerDst <= instance.GetMaxViewDistance() * TerrainGenerationConfig.GetChunkSize();
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool isVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
