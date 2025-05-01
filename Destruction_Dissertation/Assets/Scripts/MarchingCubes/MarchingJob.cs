using System.ComponentModel;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct MarchingJob : IJob
{
    [Unity.Collections.ReadOnly] public NativeArray<float> localData;
    [Unity.Collections.ReadOnly] public int width;
    [Unity.Collections.ReadOnly] public int height;
    [Unity.Collections.ReadOnly] public float terrainSurface;

    public NativeList<Vector3> vertices;
    public NativeList<int> triangles;

    [Unity.Collections.ReadOnly] public NativeArray<int3> CornerTable;
    [Unity.Collections.ReadOnly] public NativeArray<int2> EdgeIndexes;
    [Unity.Collections.ReadOnly] public NativeArray<int> TriangleTableFlat; //Flat 2D array

    public void Execute()
    {
        int sizeX = width + 1;
        int sizeY = height + 1;
        int sizeZ = width + 1;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)

                {

                    float4 cubeA = default;
                    float4 cubeB = default;
                    for (int i = 0; i < 8; i++)
                    {
                        int3 corner = CornerTable[i];
                        int XI = x + corner.x;
                        int YI = y + corner.y;
                        int ZI = z + corner.z;

                        if (XI < 0 || YI < 0 || ZI < 0 || XI >= sizeX || YI >= sizeY || ZI >= sizeZ)
                        {
                            if (i < 4) cubeA[i] = 1f;
                            else cubeB[i - 4] = 1f;
                            continue;
                        }

                        int flatindex = XI * sizeY * sizeZ + YI * sizeZ + ZI;
                        float val = localData[flatindex];

                        if (i < 4) cubeA[i] = val;
                        else cubeB[i - 4] = val;

                    }

                    int configIndex = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        float value = i < 4 ? cubeA[i] : cubeB[i - 4];
                        if (value > terrainSurface)
                            configIndex |= 1 << i;
                    }

                    if (configIndex == 0 || configIndex == 255)
                        continue;

                    int triangleOffset = configIndex * 15;
                    if (triangleOffset + 14 >= TriangleTableFlat.Length)
                        return;
                    for(int i = 0; i < 15; i += 3)
                    {
                        int edgeA = TriangleTableFlat[triangleOffset + i];
                        int edgeB = TriangleTableFlat[triangleOffset + i + 1];
                        int edgeC = TriangleTableFlat[triangleOffset + i + 2];


                        if (edgeA == -1 || edgeB == -1 || edgeC == -1)
                            break;

                        Vector3 vertA = InterpolateEdge(x, y, z, cubeA, cubeB, edgeA);
                        Vector3 vertB = InterpolateEdge(x, y, z, cubeA, cubeB, edgeB);
                        Vector3 vertC = InterpolateEdge(x, y, z, cubeA, cubeB, edgeC);

                        int baseIndex = vertices.Length;
                        vertices.Add(vertA);
                        vertices.Add(vertB);
                        vertices.Add(vertC);

                        triangles.Add(baseIndex);
                        triangles.Add(baseIndex + 1);
                        triangles.Add(baseIndex + 2);


                    }
                }

            }
        }
    }




    private Vector3 InterpolateEdge(int x, int y, int z, float4 cubeA, float4 cubeB, int edgeIndex)
    {
        int2 edge = EdgeIndexes[edgeIndex];
        int3 cornerA = CornerTable[edge.x];
        int3 cornerB = CornerTable[edge.y];

        float valA = edge.x < 4 ? cubeA[edge.x] : cubeB[edge.x - 4];
        float valB = edge.y < 4 ? cubeA[edge.y] : cubeB[edge.y - 4];

        float delta = valB - valA;
        if (math.abs(delta) < 1e-6f)
            return new float3(x + cornerA.x, y + cornerA.y, z + cornerA.z);

        float ter = math.clamp((terrainSurface - valA) / delta, 0f, 1f);

        float3 posA = new float3(x + cornerA.x, y + cornerA.y, z + cornerA.z);
        float3 posB = new float3(x + cornerB.x, y + cornerB.y, z + cornerB.z);

        return math.lerp(posA, posB, ter);
    }
}
