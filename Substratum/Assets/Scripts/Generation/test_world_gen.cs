using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* SOURCE: https://www.youtube.com/watch?v=_XtOOhxRsWY */

public class test_world_gen : MonoBehaviour
{
    //User testing variables
    [Header("Options")]
    public bool simple = true;
    public bool generateCaves = true;

    [Header("World Heights")]
    public int worldSize = 200;
    public int dirtLayerHeight = 5;
    public int heightAddition = 20;

    [Header("Tree Generation")]
    public int treeChance = 10;
    public int minTreeHeight = 7;
    public int maxTreeHeight = 25;
    

    [Header("Perlin Noise")]
    public float heightMultiplier = 40f;
    public float caveFreq = 0.08f;
    public float terrFreq = 0.05f;
    public float blockThreshold = 0.6f;


    [Header("Game Objects")]
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject[] treeParts;
    [SerializeField] GameObject player;   

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
        player.transform.position = new Vector3(worldSize / 2, worldSize + 2);
    }  

    //Generates land with caves. REQUIRES=<GetNoise()> called
    private void GeneratePerlinTerr(float seed)
    {
        //Heiarchy objects
        GameObject blocks = new GameObject("Blocks");
        blocks.transform.parent = transform;
        blocks.transform.position = Vector3.zero;
        int numBlocksPlaced = 0;

        GameObject trees = new GameObject("Trees");
        trees.transform.position = Vector3.zero;
        trees.transform.parent = transform;
        int numTreesPlaces = 0;

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
                    PlaceTile(targetTile, x, y, 0f, blocks.transform);
                    numBlocksPlaced++;
                    if (y > height - 1 && 0 == Random.Range(0, treeChance))
                    {
                        PlaceTree(x, y + 1, trees);
                        numTreesPlaces++;
                    }
                }
            }           
        }
        blocks.name = blocks.name + $"_{numBlocksPlaced}";
        trees.name = trees.name + $"_{numTreesPlaces}";
    }

    // Generates noise texture2d for world generation
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

    // Get Random Block
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

    private void PlaceTile(GameObject target, float x, float y, float layerAddition = 0, Transform parentTransf = null)
    {
        GameObject tile = Instantiate(target);
        tile.transform.name = tile.transform.name.Replace("(Clone)", "");       
        tile.transform.position = new Vector3(.5f + x, .5f + y, layerAddition);

        if (parentTransf == null)
            tile.transform.parent = transform;
        else
            tile.transform.parent = parentTransf;
    }

    private void PlaceTree(float x, float y, GameObject parentObject = null)
    {
        //Heiarchy
        GameObject parentNode = new GameObject($"Tree_{x}_{y}");
        parentNode.transform.position = new Vector3(0, 0, 0);
        if (parentObject != null)
            parentNode.transform.parent = parentObject.transform;
        else
            parentNode.transform.parent = transform;

        //Place Tree
        int treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(treeParts[0], x, y + i, 0.1f, parentNode.transform);
            if(Random.Range(0, 7) == 0 && i > 3)
            {
                int branch = Random.Range(0, 2);
                if(branch == 0)
                    PlaceTile(treeParts[1], x - 1, y + i, 0.06f, parentNode.transform);
                else
                    PlaceTile(treeParts[2], x + 1, y + i, 0.06f, parentNode.transform);
            }
        }
        PlaceTile(treeParts[3], x, y + treeHeight + 2, 0.05f, parentNode.transform);

    }

    // This generates a flat caveless land
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
