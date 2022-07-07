using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* SOURCE: https://www.youtube.com/watch?v=_XtOOhxRsWY */

public class WorldGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public bool generateCaves = true;
    public bool simple = true;
    public int worldSize = 200;
    public int dirtLayerHeight = 5;
    public int heightAddition = 20;
    public float heightMultiplier = 40f;

    [Header("Chunk Settings")]
    public int chunkSize = 16;
    private GameObject[] chunks;

    [Header("Noise Settings")]    
    public float caveFreq = 0.08f;
    public float terrFreq = 0.05f;
    public float blockThreshold = 0.6f;
    public Texture2D noise;

    [Header("Tree Settings")]
    public int treeChance = 10;
    public int minTreeHeight = 7;
    public int maxTreeHeight = 25;

    [Header("Game Objects")]
    [SerializeField] GameObject[] treeParts;
    [SerializeField] GameObject player;

    public Tilemap Tilemap;
    public Tile[] Tiles;
    public bool generate = true;

    void Start()
    {
        if (!generate) return;
        if (!simple)
        {
            float seed = Random.Range(-10000, 10000);
            GetNoise(seed);

            GeneratePerlinTerr(seed);
        }
        else
        {
            GenerateSimple();
        }
        player.transform.position = new Vector3(worldSize / 2, 225);
    }  

    private void GeneratePerlinTerr(float seed)
    {
        //Place Blocks
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++)
            {
                Tile targetTile = GetTargetBlock(y, height);
                float pixelVal = noise.GetPixel(x, y).r;
                if (!generateCaves || pixelVal <= blockThreshold)
                {
                    PlaceTile(targetTile, x, y, 0);
                    if (y > height - 1 && 0 == Random.Range(0, treeChance))
                    {
                        PlaceTree(x, y + 1);
                    }
                }
            }           
        }
    }

    private void GetNoise(float seed)
    {
        noise = new Texture2D(worldSize, worldSize);
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                float v = Mathf.PerlinNoise((i + seed) * caveFreq, (j + seed) * caveFreq);
                noise.SetPixel(i, j, new Color(v, v, v));
            }
        }
        noise.Apply();
    }

    private Tile GetRandomBlock()
    {
        int index = Random.Range(0, Tiles.Length - 1);
        Tile newTile = Tiles[index];
        return newTile;
    }
    private Tile GetTargetBlock(int y, float height)
    {
        int index;
        if(y < height - dirtLayerHeight) {
            index = 1;
        } else if ( y < height - 1)
        {
            index = 0;
        } else
        {
            index = 2;
        }
        return Tiles[index];

    }
    private void PlaceTile(Tile tile, int x, int y, int layerAddition = 0)
    {
        Tilemap.SetTile(new Vector3Int(x, y, layerAddition), tile);
    }

    private void PlaceTree(int x, int y)
    {
        /*//Place Tree
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(treeParts[0], x, y + i, 0.1f);
            if(Random.Range(0, 7) == 0 && i > 3)
            {
                int branch = Random.Range(0, 2);
                if(branch == 0)
                    PlaceTile(treeParts[1], x - 1, y + i, 0.06f);
                else
                    PlaceTile(treeParts[2], x + 1, y + i, 0.06f);
            }
        }
        PlaceTile(treeParts[3], x, y + treeHeight + 2, 0.05f);*/

    }

    private void GenerateSimple()
    {
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                Tile newTile = GetRandomBlock();
                PlaceTile(newTile, i, j, 0);
            }
        }
    }
}
