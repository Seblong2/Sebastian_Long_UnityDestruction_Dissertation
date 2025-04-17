using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "BiomeManager", menuName = "VoxelAssets/Biome Manager")]
public class BiomeManager : ScriptableObject
{
    public string nameBiome;

    public int solidGroundHeight; // Below this value is to be assumed to be solid ground
    public int terrainHeight; // The height of the terrain from the solidgroundheight to the max ive set 
    public float terrainScale;
}
