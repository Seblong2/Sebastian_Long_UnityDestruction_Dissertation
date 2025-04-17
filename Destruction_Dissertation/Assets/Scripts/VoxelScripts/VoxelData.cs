using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VoxelData 
{

    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeChunks = 10;

    public static int WorldSizeInVoxels
    {
        get { return WorldSizeChunks * ChunkWidth; }
    }

    public static readonly int ViewDistanceInChunks = 5;

    public static readonly int TextureAtlasSizeInVoxels = 4; 
    public static float NormalizedVoxelTextureSize
    {
        get
        {return 1f / (float)TextureAtlasSizeInVoxels; } 
    }

    public static readonly Vector3[] voxelVerts = new Vector3[8]// Getting all the vertices to draw the voxel
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

    public static readonly Vector3[] checkingForExposedFaces = new Vector3[6] // This data table will check for inside faces on each voxel cube and make sure they arent being loaded for maximum optimisation
    {
         new Vector3(0.0f, 0.0f, -1.0f), // Back Face
         new Vector3(0.0f, 0.0f, 1.0f), // Front Face
         new Vector3(0.0f, 1.0f, 0.0f), // Top Face
         new Vector3(0.0f, -1.0f, 0.0f), // Bottom Face
         new Vector3(-1.0f, 0.0f, 0.0f), // Left Face
         new Vector3(1.0f, 0.0f, 0.0f)  // Right Face
    };

    public static readonly int[,] voxelTri = new int[6, 4] // Six triangles per face on the voxel cube
    {

        // 0 1 2 2 1 3 Vertex index pattern
        {0, 3, 1, 2 }, // Back Face Index
        {5, 6, 4, 7 }, // Front Face Index
        {3, 7, 2, 6 }, // Top Face Index
        {1, 5, 0, 4 }, // Bottom Face Index
        {4, 7, 0, 3 }, // Left Face Index
        {1, 2, 5, 6 }  // Right Face Index
    };

    public static readonly Vector2[] voxelUvs = new Vector2[4] //Mapping the Uvs for texture use on voxels
    {
        new Vector2(0.0f,0.0f), // Bottom Left Vertice
        new Vector2(0.0f,1.0f), // Top left Vertice
        new Vector2(1.0f,0.0f), // Bottom Right Vertice
        new Vector2(1.0f,1.0f) //  Top Right Vertice
    };
}
