using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoxelChunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    void Start()
    {
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int p = 0; p < 6; p++)
        {
            for (int i = 0; i < 6; i++)
            {
                int triangleIndex = VoxelData.voxelTri[p, i]; // Pulling a number out of voxel data
                vertices.Add(VoxelData.voxelVerts[triangleIndex]); // using the number to add one of the vertices from the data table 
                triangles.Add(vertexIndex);//Adding to the list 

                uvs.Add(Vector2.zero);

                vertexIndex++; //Incrementation
            }
        }
        //Adding a mesh to the arrays when they are genrated
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices.ToArray ();
        mesh.triangles = triangles.ToArray ();  
        mesh.uv = uvs.ToArray (); // Just a simple material adder

        mesh.RecalculateNormals (); // This just sorts the normal textures out to make sure they are facing the correct way and are placed correctly

        meshFilter.mesh = mesh;

    }

    
}
