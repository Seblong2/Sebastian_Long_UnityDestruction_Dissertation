using UnityEngine;

public class WorldVoxel : MonoBehaviour
{
    public Material material;
    public VoxelType[] voxelTypes;

    VoxelChunk[,] voxelChunks = new VoxelChunk[VoxelData.WorldSizeChunks, VoxelData.WorldSizeChunks];

    private void Start()
    {
        WorldGeneration();
    }

    void WorldGeneration()
    {
        for (int x = 0; x < VoxelData.WorldSizeChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldSizeChunks; z++)
            {
                CreateChunk(x, z);
            }
        }
    }

    void CreateChunk (int x, int z)
    {
        voxelChunks[x, z] = new VoxelChunk(new ChunkLocation(x, z), this);
    }
}



[System.Serializable]
public class VoxelType
{
    public string VoxelName;
    public bool isSolid;

    [Header("Texture Value")]
    public int backFace;
    public int frontFace;
    public int topFace;
    public int bottomFace;
    public int leftFace;
    public int rightFace;


    //Face Order - Back, Front, Top, Bottom, Left, Right

    public int GetFaceID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFace;

            case 1:
                return frontFace;

            case 2:
                return topFace;

            case 3:
                return bottomFace;

            case 4:
                return leftFace;

            case 5:
                return rightFace;

                default:
                Debug.Log("Error getting Face texture ID");
                    return 0;   

        }
    }
}
