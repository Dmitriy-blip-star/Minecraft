using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Terrain
{
    public class GameWorld : MonoBehaviour
    {
        public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
        public ChunkRenderer ChunkPrefab;

        Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    float xPos = x * ChunkRenderer.ChunkWeight * ChunkRenderer.BlockScale;
                    float zPos = y * ChunkRenderer.ChunkWeight * ChunkRenderer.BlockScale;

                    ChunkData chunkData = new ChunkData();
                    chunkData.ChunkPosition = new Vector2Int(x, y);

                    chunkData.Bloks = TerrainGenerate.GenerateTerrain(xPos, zPos);
                    ChunkDatas.Add(new Vector2Int(x,y), chunkData);

                    var chunk = Instantiate(ChunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                    chunk.ChunkData = chunkData;
                    chunk.ParentWorld = this;

                    chunkData.Renderer = chunk;
                }
            }
        }
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

                if (Physics.Raycast(ray, out var hitInfo))
                {
                    Vector3 blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
                    Vector2Int chunkPos = GetChungContainingBlock(blockWorldPos); 
                    if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.ChunkWeight;
                        chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                    }
                }
            }
        }

        public Vector2Int GetChungContainingBlock(Vector3Int blockWorldPos)
        {
            return new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWeight, blockWorldPos.z / ChunkRenderer.ChunkWeight);
        }
    }
}