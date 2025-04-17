using System.Collections.Generic;
using UnityEngine;

public class WorldVoxel : MonoBehaviour
{
    public Transform player;
    public Vector3 spawnPos;


    public Material material;
    public VoxelType[] voxelTypes;

    VoxelChunk[,] voxelChunks = new VoxelChunk[VoxelData.WorldSizeChunks, VoxelData.WorldSizeChunks]; // Array for chunks and voxels that will generate in the world depending on size that is set

    List<ChunkLocation> activeChunk = new List<ChunkLocation> ();
    ChunkLocation playerChunkLocation;
    ChunkLocation LastKnownChunkLocation;


    private void Start()
    {
        spawnPos = new Vector3((VoxelData.WorldSizeChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 5f, (VoxelData.WorldSizeChunks * VoxelData.ChunkWidth) / 2f); // Spawns the player in the center of the amount of chunks
        WorldGeneration();
        LastKnownChunkLocation = GetChunkFromPlayerPos(player.position);
        
    }

    private void Update()
    {
       playerChunkLocation = GetChunkFromPlayerPos(player.position); // Checking for player location to optimise the view distance updating 

        if (!playerChunkLocation.ChunkCheck(LastKnownChunkLocation))
            CheckingForViewDistance();
    }

    void WorldGeneration()// This has been updated to work with view distance for optimisation purposes
    {
        for (int x = (VoxelData.WorldSizeChunks /2) - VoxelData.ViewDistanceInChunks; x < (VoxelData.WorldSizeChunks / 2) + VoxelData.ViewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.WorldSizeChunks / 2) - VoxelData.ViewDistanceInChunks; z < (VoxelData.WorldSizeChunks / 2) + VoxelData.ViewDistanceInChunks; z++)
            {
                CreateChunk(x, z);
            }
        }

        player.position = spawnPos;
    }


    ChunkLocation GetChunkFromPlayerPos (Vector3 pos) // Getting the player location reference based on what chunk they are in 
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkLocation(x, z);
    }
    void CheckingForViewDistance()//Enable and disable chunks based on player location and the distance from them
    {
        ChunkLocation location = GetChunkFromPlayerPos(player.position);

        List<ChunkLocation> previousActiveChunk = new List<ChunkLocation>(activeChunk);//Currently active chunks on screen

        for (int x = location.x - VoxelData.ViewDistanceInChunks; x < location.x + VoxelData.ViewDistanceInChunks; x++) //Checking which chunks are in the view distance from the player
        {
            for (int z = location.z - VoxelData.ViewDistanceInChunks; z < location.z + VoxelData.ViewDistanceInChunks; z++) //Checking which chunks are in the view distance from the player
            {
                if (isChunkInWorld(new ChunkLocation(x, z)))
                {
                    if (voxelChunks[x, z] == null)
                        CreateChunk(x, z);
                    else if (!voxelChunks[x, z].isChunkActive)
                    {
                        voxelChunks[x, z].isChunkActive = true;
                        activeChunk.Add(new ChunkLocation(x, z));
                    }
                }

                for (int i = 0; i < previousActiveChunk.Count; i++)
                {
                    if (previousActiveChunk[i].ChunkCheck(new ChunkLocation(x, z))) // Any chunks outside the view distance will be removed from the chunk viewdistance list
                        previousActiveChunk.RemoveAt(i);
                }
            }
        }

        foreach (ChunkLocation c in previousActiveChunk) // Setting any chunks outside of the list inactive until put back into the list via the view distance
            voxelChunks[c.x, c.z].isChunkActive = false;    
    }

    public byte GetVoxel (Vector3 pos) // This is the new voxel map population function, just works more effectively and optimised to work with world chunk generation
    {
        if (!isVoxelInWorld(pos))//just checking if voxel is outside of the world array
            return 0;
        if (pos.y < 1)
            return 1;

        else if (pos.y == VoxelData.ChunkHeight - 1)
            return 3;

        else
            return 2;
    }

    void CreateChunk (int x, int z)
    {
        voxelChunks[x, z] = new VoxelChunk(new ChunkLocation(x, z), this);
        activeChunk.Add(new ChunkLocation(x, z));
    }

    bool isChunkInWorld(ChunkLocation location) // Checking if chunks are in the world and in the correct location based on the voxeldata arrays 
    {
        if (location.x > 0 && location.x < VoxelData.WorldSizeChunks - 1 && location.z > 0 && location.z < VoxelData.WorldSizeChunks - 1)
            return true;
        else
            return false;
    }

    bool isVoxelInWorld (Vector3 pos) // Another check for if voxels within the chunks are inside the world
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;
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
