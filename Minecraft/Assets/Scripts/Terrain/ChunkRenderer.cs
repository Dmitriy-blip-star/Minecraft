using Assets.Scripts.Terrain;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWeight = 32;
    public const int ChunkHeight = 128;
    public const float BlockScale = .125f;

    public ChunkData ChunkData;
    public GameWorld ParentWorld;

    private Mesh chunkMesh;

    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    private void Start()
    {
        chunkMesh = new Mesh();
        RegenerateMesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Bloks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Wood;
        RegenerateMesh();
    }
    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Bloks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void RegenerateMesh()
    {
        verticies.Clear();
        uvs.Clear();
        triangles.Clear();

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWeight; x++)
            {
                for (int z = 0; z < ChunkWeight; z++)
                {
                    GenerateBlock(x, y, z);
                }
            }
        }

        chunkMesh.triangles = Array.Empty<int>();
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.uv= uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateBounds(); // для лучшего взаимодействия с освещением
        chunkMesh.RecalculateNormals(); // для лучшей работы с коллайдерами и пр.

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPosition);

        if (blockType == BlockType.Air) return;



        if(GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(blockType, Vector2Int.right);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(blockType, Vector2Int.left);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(blockType, Vector2Int.zero);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(blockType, Vector2Int.zero);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(blockType, Vector2Int.down);
        }
        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(blockType, Vector2Int.up);
        }

    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if(blockPosition.x >= 0 && blockPosition.x < ChunkWeight &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWeight)
        {
            return ChunkData.Bloks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else 
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight) return BlockType.Air;

            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;

            if (blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWeight;
            }
            else if (blockPosition.x >= ChunkWeight)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWeight;
            }

            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWeight;
            }
            else if (blockPosition.z >= ChunkWeight)
            { 
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWeight;
            }

            if (ParentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Bloks[blockPosition.x, blockPosition.y, blockPosition.z];
            }

            else
            {
                return BlockType.Air;
            }
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddLastVerticiesSquare();
    }

    private void AddLastVerticiesSquare()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }

    private void AddUvs(BlockType blockType, Vector2Int normal)
    {
        Vector2 uv;

        Vector2 grass = new Vector2(32f / 256, 96f / 256);
        Vector2 dirty = new Vector2(32f / 256, 240f / 256);
        Vector2 side = new Vector2(48f / 256, 240f / 256);

        if (blockType == BlockType.Grass)
        {
            uv = normal == Vector2Int.up ? grass :
                normal == Vector2Int.down ? dirty :
                side;
        }

        else if (blockType == BlockType.Stone)
        {
            uv = new Vector2(16f / 256, 240f / 256);
        }

        else if (blockType == BlockType.Wood)
        {
            uv = new Vector2(64f / 256, 240f / 256);
        }

        else
        {
            uv = new Vector2(160f / 256, 224f / 256);
        }

        for (int i = 0; i < 4; i++)
        {
            uvs.Add(uv);
        }
    }
}
