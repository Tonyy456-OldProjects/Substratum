using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* SOURCE: https://www.youtube.com/watch?v=_XtOOhxRsWY */

public class test_world_gen : MonoBehaviour
{
    /* public changable fields */
    [SerializeField] GameObject[] Blocks;
    [SerializeField] GameObject player;
    [SerializeField] Tilemap tilemap;
    [SerializeField] RuleTile dirt;
    [SerializeField] RuleTile stone;
    public bool simple = true;

    private int worldSize = 100;
    private float freq = 0.05f;
    private float seed;
    private Texture2D noise;


    void Start()
    {
        if (!simple) {
            seed = Random.Range(-10000, 10000);
            GetNoise();
            GenerateBlocks();
            
        } 
        else
        {
            GenerateSimple();
        }
        player.transform.position = new Vector3(0.16f * worldSize / 2, 0.16f);
    }

    private void GenerateSimple()
    {
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                //GameObject newTile = Instantiate(Blocks[Random.Range(0, Blocks.Length)]);
                //newTile.transform.position = new Vector3(0.48f + 0.16f * i, -0.48f + -0.16f * j);

                if(Random.Range(-10000,10000) % 2 == 0)
                {
                    tilemap.SetTile(new Vector3Int(2 + i,-2 +  -1 * j), dirt);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(2 + i, -2 + -1 * j), stone);
                }
            }
        }
    }

    private void GenerateBlocks()
    {
        float value = .4f;
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                float pixelVal = noise.GetPixel(i, j).r;
                if (pixelVal > value) continue;
                GameObject newTile = Instantiate(Blocks[Random.Range(0, Blocks.Length)]);
                newTile.transform.position = new Vector3(0.48f + 0.16f*i,-0.48f+-0.16f* j);
            }
        }
    }

    private void GetNoise()
    {
        noise = new Texture2D(worldSize, worldSize);
        for(int i = 0; i < worldSize; i++)
        {
            for(int j = 0; j < worldSize; j++)
            {
                float v = Mathf.PerlinNoise((i+seed) * freq, (j + seed) * freq);
                noise.SetPixel(i, j, new Color(v, v, v));
            }
        }
        noise.Apply();
    }
}
