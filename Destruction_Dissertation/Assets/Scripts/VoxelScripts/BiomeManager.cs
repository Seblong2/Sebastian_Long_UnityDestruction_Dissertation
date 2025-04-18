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

    public Lode[] lodes;
}

[System.Serializable]
public class Lode // This class is a Lode that can determine the generation of things such as ores and give them attributes on how they would generate in the chunks
{
    public string nodeName;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
