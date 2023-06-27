using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public static class TerrainGenerate
    {
        public static BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
        {
            var result = new BlockType[ChunkRenderer.ChunkWeight, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWeight];

            for (int x = 0; x < ChunkRenderer.ChunkWeight; x++)
            {
                for (int z = 0; z < ChunkRenderer.ChunkWeight; z++)
                {
                    float height = Mathf.PerlinNoise((x / 4 + xOffset) * .2f, (z / 4 + zOffset) * .2f) * 25 + 10;

                    for (int y = 0; y < height; y++)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                }
            }
            return result;
        }
    }
}