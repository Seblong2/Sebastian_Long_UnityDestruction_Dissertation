using System.Collections.Generic;
using UnityEngine;

public class MarchingChunk
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    int width { get { return GameData.ChunkWidth; } }
    int height { get { return GameData.ChunkHeight; } }
    float terrainSurface { get { return GameData.terrainSurface; } }



    public GameObject chunkObject;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    MeshRenderer meshRenderer;
    public float DestructionRadius = 10f;
    public WorldGenMarching world;

    Vector3Int ChunkPos;


    float[,,] terrainMap;

    float Perlin3D(float x, float y, float z, float scale)
    {
        float xy = Mathf.PerlinNoise(x * scale, y * scale);
        float yz = Mathf.PerlinNoise(y * scale, z * scale);
        float xz = Mathf.PerlinNoise(x * scale, z * scale);
        float yx = Mathf.PerlinNoise(y * scale, x * scale);
        float zy = Mathf.PerlinNoise(z * scale, y * scale);
        float zx = Mathf.PerlinNoise(z * scale, x * scale);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }



    public MarchingChunk(Vector3Int _Position, float[,,] sharedmap)
    {
        chunkObject = new GameObject();
        chunkObject.name = string.Format("Chunk {0}, {1}", _Position.x, _Position.z);
        ChunkPos = _Position;
        chunkObject.transform.position = ChunkPos;
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshRenderer.material = Resources.Load<Material>("Materials/Chunk_M");
        chunkObject.transform.tag = "Terrain";
        terrainMap = sharedmap;

        PopulateTerrain();
        CreateMeshData();

    }


    void PopulateTerrain()
    {
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                   
                    float worldX = x + ChunkPos.x;
                    float worldY = y + ChunkPos.y;
                    float worldZ = z + ChunkPos.z;

                    

                    float surfaceHeight = GameData.GetTerrainHeight((int)worldX, (int)worldZ);
                    float surfaceDensity = worldY - surfaceHeight;
                   

                    float cavenoise = Perlin3D(worldX, worldY, worldZ, 0.05f);
                    float caveMask = Mathf.Clamp01(cavenoise - 0.6f) * 2f;

                    float density = surfaceDensity - caveMask;

                    density = Mathf.Clamp(density, -1f, 1f);

                    int gx = x + ChunkPos.x + GameData.TerrainMapOffset.x;
                    int gy = y + ChunkPos.y + GameData.TerrainMapOffset.y;
                    int gz = z + ChunkPos.z + GameData.TerrainMapOffset.z;

                    if (gx >= 0 && gx < GameData.GlobalTerrainMap.GetLength(0) &&
                        gy >= 0 && gy < GameData.GlobalTerrainMap.GetLength(1) &&
                        gz >= 0 && gz < GameData.GlobalTerrainMap.GetLength(2))
                    {
                        GameData.GlobalTerrainMap[gx, gy, gz] = density;
                    }

                    // GameData.GlobalTerrainMap[x + ChunkPos.x + GameData.TerrainMapOffset.x, y + ChunkPos.y + GameData.TerrainMapOffset.y, z + ChunkPos.z + GameData.TerrainMapOffset.z]  = density;

                }
            }
        }
    }

   public void CreateMeshData()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {

                    MarchCube(new Vector3Int(x, y, z));
                }
            }
        }
        BuildMesh();
    }

    int GetCubeConfig(float[] cube)
    {
        int configIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > terrainSurface)
                configIndex |= 1 << i;
        }

        return configIndex;
    }

    float SampleTerrain(Vector3Int point)
    {
        int gx = point.x + ChunkPos.x + GameData.TerrainMapOffset.x;
        int gy = point.y + ChunkPos.y + GameData.TerrainMapOffset.y;
        int gz = point.z + ChunkPos.z + GameData.TerrainMapOffset.z;

        if (gx >= 0 && gx < GameData.GlobalTerrainMap.GetLength(0) &&
                   gy >= 0 && gy < GameData.GlobalTerrainMap.GetLength(1) &&
                   gz >= 0 && gz < GameData.GlobalTerrainMap.GetLength(2))
        {
            return GameData.GlobalTerrainMap[gx, gy, gz];
        }
        else
        {
            return 1f;
        }

    }

    int VertForIndice(Vector3 vert)
    {
        //Loop through all vertices in the current vertices list
        for (int i = 0; i < vertices.Count; i++)
        {
            //If vert matches then return the index to avoid duplication of vertices
            if (vertices[i] == vert)
                return i;
        }

        //If no match is found add this vert to list and return last known index.
        vertices.Add(vert);
        return vertices.Count - 1;
    }

    void MarchCube(Vector3Int position)
    {
        //Sample terrain values at each corner for the cube
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            cube[i] = SampleTerrain(position + GameData.CornerTable[i]);
        }


        int configIndex = GetCubeConfig(cube);

        if (configIndex == 0 || configIndex == 255)
            return;

        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int p = 0; p < 3; p++)
            {
                int indice = GameData.TriangleTable[configIndex, edgeIndex];

                if (indice == -1)
                    return;

                Vector3 vert1 = position + GameData.CornerTable[GameData.EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + GameData.CornerTable[GameData.EdgeIndexes[indice, 1]];


                Vector3 vertPos;


                //Getting terrain values at the end of the current edge from the cube array that is created about
                float vert1Sample = cube[GameData.EdgeIndexes[indice, 0]];
                float vert2Sample = cube[GameData.EdgeIndexes[indice, 1]];

                //Calucations for the difference between terrain values
                float difference = vert2Sample - vert1Sample;

                //If the difference is 0 then pass terrain through middle
                if (difference == 0)
                    difference = terrainSurface;
                else
                    difference = (terrainSurface - vert1Sample) / difference;

                //Calculating the point along the cube edge that passes through
                vertPos = vert1 + ((vert2 - vert1) * difference);


                triangles.Add(VertForIndice(vertPos));



                edgeIndex++;

            }
        }
    }

    public void ClearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }

    void BuildMesh()
    {
        Debug.Log($"Vertices: {vertices.Count}, Triangles: {triangles.Count}");
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void TerrainDestroy(Vector3 pos)
    {
        Vector3 local = pos - ChunkPos;
        int radius = 3;
        bool touchedEdge = false;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3Int point = new Vector3Int(
                        Mathf.FloorToInt(local.x) + x,
                        Mathf.FloorToInt(local.y) + y,
                        Mathf.FloorToInt(local.z) + z
                        );

                    if (InBounds(point))
                    {
                        float distance = Vector3.Distance(point, local);
                        if (distance < radius)
                        {
                            int gx = point.x + ChunkPos.x + GameData.TerrainMapOffset.x;
                            int gy = point.y + ChunkPos.y + GameData.TerrainMapOffset.y;
                            int gz = point.z + ChunkPos.z + GameData.TerrainMapOffset.z;

                            if (gx >= 0 && gx < GameData.GlobalTerrainMap.GetLength(0) &&
                                 gy >= 0 && gy < GameData.GlobalTerrainMap.GetLength(1) &&
                                    gz >= 0 && gz < GameData.GlobalTerrainMap.GetLength(2))
                            {
                                GameData.GlobalTerrainMap[gx, gy, gz] = 1f;
                            }
                        }


                        if (point.x == 0 || point.x == width ||
                            point.y == 0 || point.y == height ||
                            point.z == 0 || point.z == width)
                        {
                            touchedEdge = true;
                        }
                    }
                }
            }
        }
       
        if (world != null)
        {
            world.RebuildNeighbourChunks(ChunkPos, includeSelf: true);
        }
    }
    public void TerrainPlace(Vector3 pos)
    {
        Vector3 local = pos - ChunkPos;
        int radius = 3;
        bool touchedEdge = false;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    Vector3Int point = new Vector3Int(
                        Mathf.FloorToInt(local.x) + x,
                        Mathf.FloorToInt(local.y) + y,
                        Mathf.FloorToInt(local.z) + z
                        );

                    if (InBounds(point))
                    {
                        float distance = Vector3.Distance(point, local);
                        if (distance < radius)
                        {
                            int gx = point.x + ChunkPos.x + GameData.TerrainMapOffset.x;
                            int gy = point.y + ChunkPos.y + GameData.TerrainMapOffset.y;
                            int gz = point.z + ChunkPos.z + GameData.TerrainMapOffset.z;

                            if (gx >= 0 && gx < GameData.GlobalTerrainMap.GetLength(0) &&
                                 gy >= 0 && gy < GameData.GlobalTerrainMap.GetLength(1) &&
                                    gz >= 0 && gz < GameData.GlobalTerrainMap.GetLength(2))
                            {
                                GameData.GlobalTerrainMap[gx, gy, gz] = 0f;
                            }
                        }

                        if (point.x == 0 || point.x == width ||
                          point.y == 0 || point.y == height ||
                          point.z == 0 || point.z == width)
                        {
                            touchedEdge = true;
                        }
                    }
                }
            }
        }

        

        if (world != null)
        {
            world.RebuildNeighbourChunks(ChunkPos, includeSelf: true);
        }
    }




    bool InBounds(Vector3Int point)
    {
        return point.x >= 0 && point.x < width + 1 &&
            point.y >= 0 && point.y < height + 1 &&
            point.z >= 0 && point.z < width + 1;
    }
}