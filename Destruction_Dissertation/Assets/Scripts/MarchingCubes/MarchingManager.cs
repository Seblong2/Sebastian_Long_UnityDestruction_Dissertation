using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private float noiseReduction = 1;

    [SerializeField] private bool visualizeNoise; // If i want to visual noise i will use this bool

    private float[,,] heights; // height grid Array containing the table values
    void Start()
    {
        StartCoroutine(UpdateAll());
    }

    private IEnumerator UpdateAll() 
    {
        while (true) 
        {
            SetHeights();
            yield return new WaitForSeconds(1);
        }
    }

    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1]; // Init array for heightmaps

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; x < height + 1; y++)
            {
                for (int z = 0; x < width + 1; z++)
                {
                    float currentHeight = height * Mathf.PerlinNoise(x * noiseReduction, z * noiseReduction);
                    float newHeight;

                    if (y > currentHeight) // Checking y heights and taking away the current height and assign a new height based on size
                    {
                        newHeight = y - currentHeight;
                    }
                    else
                    {
                        newHeight = currentHeight - y;
                    }
                    heights[x, y, z] = currentHeight;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying) //Checking if application is playing or if noise is visualizing 
        {
            return;
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; x < height + 1; y++)
            {
                for (int z = 0; x < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1); // Setting colour based on height values
                    Gizmos.DrawSphere(new Vector3(x, y, z), 0.2f); //Drawing spheres with range in correct positions
                }
            }
        }
    }
}
