using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* SOURCE: https://www.youtube.com/watch?v=_XtOOhxRsWY */

public class test_world_gen : MonoBehaviour
{
    public bool simple = true;
    [SerializeField] GameObject[] Blocks;
    [SerializeField] GameObject player;
    public int worldSize = 100;      
    public int heightAddition = 20;
    public float heightMultiplier = 4f;
    public float caveFreq = 0.05f;
    public float terrFreq = 0.05f;
    public float blockThreshold = 0.6f;

    private Texture2D noise;
    void Start()
    {
        if (!simple) {
            float seed = Random.Range(-10000, 10000);
            GetNoise(seed);
            GeneratePerlinTerr(seed);        
        } 
        else
        {
            GenerateSimple();
        }
        player.transform.position = new Vector3(0.16f * worldSize / 2, 0.16f);
    }  

    //Generates land with caves. REQUIRES=<GetNoise()> called
    private void GeneratePerlinTerr(float seed)
    {
        for (int i = 0; i < worldSize; i++)
        {
            float height = Mathf.PerlinNoise((i + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int j = 0; j < height; j++)
            {
                float pixelVal = noise.GetPixel(i, j).r;
                if (pixelVal > blockThreshold) continue;
                GameObject newTile = Instantiate(Blocks[Random.Range(0, Blocks.Length)]);
                newTile.transform.parent = this.transform;
                newTile.transform.position = new Vector3(.08f + 0.16f*i,.08f + 0.16f* j);
            }
        }
    }

    // Generates noise texture2d for world generation
    private void GetNoise(float seed)
    {
        noise = new Texture2D(worldSize, worldSize);
        for(int i = 0; i < worldSize; i++)
        {
            for(int j = 0; j < worldSize; j++)
            {
                float v = Mathf.PerlinNoise((i+seed) * caveFreq, (j + seed) * caveFreq);
                noise.SetPixel(i, j, new Color(v, v, v));
            }
        }
        noise.Apply();
    }


    // This generates a flat caveless land
    private void GenerateSimple()
    {
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                GameObject newTile = Instantiate(Blocks[Random.Range(0, Blocks.Length)]);
                newTile.transform.position = new Vector3(0.48f + 0.16f * i, -0.48f + -0.16f * j);
                /*
                if (Random.Range(-10000, 10000) % 2 == 0)
                {
                    tilemap.SetTile(new Vector3Int(2 + i, -2 + -1 * j), dirt);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(2 + i, -2 + -1 * j), stone);
                }
                */
            }
        }
    }
}
