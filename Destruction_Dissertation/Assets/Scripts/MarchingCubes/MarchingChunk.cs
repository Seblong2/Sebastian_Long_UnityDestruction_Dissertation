using System.Collections;
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

    Vector3Int ChunkPos;


    float[,,] terrainMap;

   

    

    public MarchingChunk(Vector3Int _Position)
    {
        chunkObject = new GameObject();
        chunkObject.name = string.Format("Chunk {0}, {1}", _Position.x, _Position.z);
        ChunkPos = _Position;
        chunkObject.transform.position = ChunkPos;
        meshFilter = chunkObject.AddComponent< MeshFilter>();
        meshRenderer = chunkObject.AddComponent< MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshRenderer.material = Resources.Load<Material>("Materials/Chunk_M");
        chunkObject.transform.tag = "Terrain";
        terrainMap = new float[width + 1, height + 1, width + 1];

        PopulateTerrain();
        CreateMeshData();
      
    }


    void PopulateTerrain()
    {
        for(int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float thisHeight;

                    thisHeight = GameData.GetTerrainHeight(x + ChunkPos.x, z + ChunkPos.z);

                    terrainMap[x, y, z] = (float)y - thisHeight;
                }
            }
        }
    }

    void CreateMeshData()
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
        for(int i = 0; i < 8; i++)
        {
            if (cube[i] > terrainSurface)
                configIndex |= 1 << i;
        }

        return configIndex;
    }

    float SampleTerrain(Vector3Int point)
    {
        return terrainMap[point.x, point.y, point.z];
    }

    int VertForIndice (Vector3 vert)
    {
        //Loop through all vertices in the current vertices list
        for (int i = 0; i < vertices.Count; i ++)
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



             
                edgeIndex++;

            }
        }
    }

    void ClearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }

    void BuildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void DestroyTerrain(Vector3 pos)
    {
        Vector3Int v3Int = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        v3Int -= ChunkPos;
        terrainMap[v3Int.x, v3Int.y, v3Int.z] = 1f;
        CreateMeshData();
    }

    public void PlaceTerrain(Vector3 pos)
    {
        Vector3Int v3Int = new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        v3Int -= ChunkPos;
        terrainMap[v3Int.x, v3Int.y, v3Int.z] = 0f;
        CreateMeshData();
    }

   

}