using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* SOURCE: https://www.youtube.com/watch?v=_XtOOhxRsWY */

public class test_world_gen : MonoBehaviour
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
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject[] treeParts;
    [SerializeField] GameObject player;

    private GameObject tileGroup;

    void Start()
    {
        tileGroup = new GameObject("Tiles");
        tileGroup.transform.position = Vector3.zero;
        if (!simple) {
            float seed = Random.Range(-10000, 10000);
            GetNoise(seed);
            CreateChunks();
            GeneratePerlinTerr(seed);        
        } 
        else
        {
            GenerateSimple();
        }
        player.transform.position = new Vector3(worldSize / 2, worldSize + 2);
    }  

    private void GeneratePerlinTerr(float seed)
    {
        //Place Blocks
        for (int x = 0; x < worldSize; x++)
        {
            float height = Mathf.PerlinNoise((x + seed) * terrFreq, seed * terrFreq) * heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++)
            {
                GameObject targetTile = GetTargetBlock(y, height);
                float pixelVal = noise.GetPixel(x, y).r;
                if (!generateCaves || pixelVal <= blockThreshold)
                {
                    PlaceTile(targetTile, x, y, 0f);
                    if (y > height - 1 && 0 == Random.Range(0, treeChance))
                    {
                        PlaceTree(x, y + 1);
                    }
                }
            }           
        }
    }

    private void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        chunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject(i.ToString());
            newChunk.transform.parent = tileGroup.transform;
            newChunk.transform.position = Vector3.zero;
            chunks[i] = newChunk;
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

    private GameObject GetRandomBlock()
    {
        int index = Random.Range(0, blocks.Length);
        GameObject newTile = Instantiate(blocks[index]);
        return newTile;
    }

    private GameObject GetTargetBlock(int y, float height)
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
        return blocks[index];

    }

    private int GetChunkCoor(int x)
    {
        if (chunks == null) return -2;
        int chunkCoor = (x / chunkSize);
        if (chunks.Length <= chunkCoor) return -1;
        return chunkCoor;
    }

    private void PlaceTile(GameObject target, int x, int y, float layerAddition = 0)
    {
        GameObject tile = Instantiate(target);

        //get chunk
        int chunkCoor = GetChunkCoor(x);
        if (chunkCoor < 0)
        {
            Destroy(tile);
            return;
        } 
        GameObject chunk = chunks[chunkCoor];

        //tile data
        tile.transform.name = tile.transform.name.Replace("(Clone)", "");
        tile.transform.position = new Vector3(.5f + x, .5f + y, layerAddition);

        //fix heiarchy
        tile.transform.parent = chunk.transform;
    }

    private void PlaceTree(int x, int y, GameObject parentObject = null)
    {
        //Place Tree
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
        PlaceTile(treeParts[3], x, y + treeHeight + 2, 0.05f);

    }

    private void GenerateSimple()
    {
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                GameObject newTile = GetRandomBlock();
                newTile.transform.parent = this.transform;
                new Vector3(.5f + i, .5f + j);
            }
        }
    }
}
