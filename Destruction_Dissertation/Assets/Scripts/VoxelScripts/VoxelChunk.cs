using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class VoxelChunk 
{
    public ChunkLocation chunkLocation;

    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
   


    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();

    public byte[,,] VoxelFaceMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth]; // Using a byte instead of a bool for memory optimisation

    WorldVoxel worldVoxel;

 public VoxelChunk (ChunkLocation _location, WorldVoxel worldChunkConstructor)
    {

        chunkLocation = _location;
        worldVoxel = worldChunkConstructor;
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter> ();
        meshRenderer = chunkObject.AddComponent<MeshRenderer> ();

        

        meshRenderer.material = worldVoxel.material;
        chunkObject.transform.SetParent(worldVoxel.transform);
        chunkObject.transform.position = new Vector3(chunkLocation.x * VoxelData.ChunkWidth, 0f, chunkLocation.z * VoxelData.ChunkWidth);
        chunkObject.name = "VoxelChunk" + chunkLocation.x + ", " + chunkLocation.z;

        //worldVoxel = GameObject.Find("WorldVoxel").GetComponent<WorldVoxel>();

        PopulateFaceMap();
        CreateChunkData();
        MeshCreation();

        
    }


    void PopulateFaceMap()// Populating the correct faces on the voxel cubes with meshes (This is so the inside faces dont get a mesh until exposed)
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    VoxelFaceMap[x, y, z] = worldVoxel.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
    }

    void CreateChunkData() //Takes data from VoxelData and builds the voxels in a chunk in the set positions
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    if (worldVoxel.voxelTypes[VoxelFaceMap[x, y, z]].isSolid)
                        AddVoxelDataToChunks(new Vector3(x, y, z));
                }
            }
        }
    }

    public bool isChunkActive // Getter for checking if chunk is active or not
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 position // Getter for chunk positions
    {
        get { return chunkObject.transform.position; }
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
        
            return false;
            else
            return true;
        
    }

    bool checkVoxels(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z)) // A clamp on the array size when checking faces to make sure its doesnt try to check outside of the set array bounds
            return worldVoxel.voxelTypes[worldVoxel.GetVoxel(pos + position)].isSolid; // Looking for ID of block types and is inside the chunk to remove uneeded faces inside the chunk 

        return worldVoxel.voxelTypes[VoxelFaceMap  [x, y, z]].isSolid; //Converted to work with worldvoxel script and using a byte instead of bool
    }

    void AddVoxelDataToChunks(Vector3 pos)// This is basically just the function for creating a single voxel
    {
        for (int p = 0; p < 6; p++)
        {
            if (!checkVoxels(pos + VoxelData.checkingForExposedFaces[p])) // Checking for offset voxels to make sure faces are being drawn in correct locations
            {
                byte voxelID = VoxelFaceMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTri[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTri[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTri[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTri[p, 3]]);

                AddTexture(worldVoxel.voxelTypes[voxelID].GetFaceID(p));

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }
    

    void MeshCreation()// Mesh being added to the voxel
    {
        //Adding a mesh to the arrays when they are genrated
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray(); // Just a simple material adder

        mesh.RecalculateNormals(); // This just sorts the normal textures out to make sure they are facing the correct way and are placed correctly

        meshFilter.mesh = mesh;
    }

    void AddTexture (int textureFaceID)
    {
        float y = textureFaceID / VoxelData.TextureAtlasSizeInVoxels;
        float x = textureFaceID - (y * VoxelData.TextureAtlasSizeInVoxels);

        x *= VoxelData.NormalizedVoxelTextureSize;
        y *= VoxelData.NormalizedVoxelTextureSize;

        y = 1f - y - VoxelData.NormalizedVoxelTextureSize; //Texture atlas starts top left (0,1) instead of bottom left (0,0)

        uvs.Add(new Vector2(x, y)); //These are replacing the uvs in voxel data x and y are representing the 4 vertices this one is 0,0 (Bottom Left)
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedVoxelTextureSize)); // 0,1 (Top Left)
        uvs.Add(new Vector2(x + VoxelData.NormalizedVoxelTextureSize, y)); // 1,0 (Bottom Right)
        uvs.Add(new Vector2(x + VoxelData.NormalizedVoxelTextureSize, y + VoxelData.NormalizedVoxelTextureSize)); // 1,1 (Top Right)
    }
}


public class ChunkLocation // Position for chunk however, not in world space, this is chunk position in the chunk mapping array
{
    public int x;
    public int z;

    public ChunkLocation(int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public bool ChunkCheck (ChunkLocation other) // Checking for active chunks based on location to the player
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;
    }
}