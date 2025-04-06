using UnityEngine;

public static class VoxelData 
{
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(0.0f, 0.0f, 0.0f), // Vert 0
        new Vector3(1.0f, 0.0f, 0.0f), // Vert 1
        new Vector3(1.0f, 1.0f, 0.0f), // Vert 2
        new Vector3(0.0f, 1.0f, 0.0f), // Vert 3
        new Vector3(0.0f, 0.0f, 1.0f), // Vert 4
        new Vector3(1.0f, 0.0f, 1.0f), // Vert 5 
        new Vector3(1.0f, 1.0f, 1.0f), // Vert 6
        new Vector3(0.0f, 1.0f, 1.0f)  // Vert 7
    };

    public static readonly int[,] voxelTri = new int[6, 6] // Six triangles per face on the voxel cube
    {
        {3, 7, 2, 2, 7, 6 }, // Top Face Index
        {5, 6, 4, 4, 6, 7 }, // Front Face Index
        {0, 3, 1, 1, 3, 2 }, // Back Face Index
        {1, 5, 0, 0, 5, 4 }, // Bottom Face Index
        {4, 7, 0, 0, 7, 3 }, // Left Face Index
        {1, 2, 5, 5, 2, 6 }  // Right Face Index
    };
}
