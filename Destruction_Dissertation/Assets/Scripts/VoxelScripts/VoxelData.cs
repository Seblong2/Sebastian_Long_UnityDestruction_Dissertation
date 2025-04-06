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

    public static readonly int[,] voxelTri = new int[1, 6] // Six triangles per face on the voxel cube
    {
        {3, 7, 2, 2, 7, 6 } //Top Face index
    };
}
