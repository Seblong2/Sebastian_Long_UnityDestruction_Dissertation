using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct EditJob : IJob
{
    [Unity.Collections.ReadOnly] public int radiusEdit;
    [Unity.Collections.ReadOnly] public float3 centerEdit;
    [Unity.Collections.ReadOnly] public bool isPlacing; // True = place || false = dig

    public NativeArray<float> localTerrain;
    public NativeArray<int> flag; // Checking for changes made
    public int width;
    public int height;


    public void Execute()
    {
        int sizeX = width + 1;
        int sizeY = height + 1;
        int sizeZ = width + 1;

        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                {

                    int index = x * sizeY * sizeZ + y * sizeZ + z;

                    float3 worldPos = new float3(x, y, z);
                    float distance = math.distance(worldPos, centerEdit);

                    if (distance < radiusEdit)
                    {
                        
                        localTerrain[index] = isPlacing ? 0f : 1f;
                        flag[0] = 1;
                    }
                }
    }
}

  
