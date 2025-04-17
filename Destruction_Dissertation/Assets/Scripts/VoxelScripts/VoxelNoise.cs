using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VoxelNoise 
{
    public static float GetPerlin2D (Vector2 pos, float offset, float scale)
    {
        return Mathf.PerlinNoise((pos.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (pos.y + 0.1f) / VoxelData.ChunkWidth * scale + offset); // Using a 2D perlin noise to make a basic version of a heightmap for the terrain
    }

    public static bool GetPerlin3D (Vector3 position, float offset, float scale, float threshhold)
    {
        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(x, y);
        float AB = Mathf.PerlinNoise(x, y);
        float AB = Mathf.PerlinNoise(x, y);
        float AB = Mathf.PerlinNoise(x, y);
        float AB = Mathf.PerlinNoise(x, y);

    }
}
