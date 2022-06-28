using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    private int surfaceDepth = 5;
    private int worldWidth = 500;
    private int worldDepth = 10;
    private float offset = 0.16f;

    [SerializeField] DirtBlock dirtFab;
    [SerializeField] StoneBlock stoneFab;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = new Vector3(dirtFab.transform.position.x - ((worldWidth / 2) * offset), dirtFab.transform.position.y, dirtFab.transform.position.z);

        for (int i = 0; i < worldDepth; i++)
        {
            if (i < surfaceDepth)
            {
                DirtBlock dirt;
                for (int j = 0; j < worldWidth; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        dirt = dirtFab;
                    }
                    else
                    {
                        dirt = Instantiate(dirtFab) as DirtBlock;
                    }

                    float posX = (offset * j) + startPos.x;
                    float posY = -(offset * i) + startPos.y;
                    dirt.transform.position = new Vector3(posX, posY, startPos.z);
                }
            } 
            else
            {
                StoneBlock stone;
                for (int k = 0; k < worldWidth; k++)
                {
                    if (i == surfaceDepth && k == 0)
                    {
                        stone = stoneFab;
                    }
                    else
                    {
                        stone = Instantiate(stoneFab) as StoneBlock;
                    }

                    float posX = (offset * k) + startPos.x;
                    float posY = -(offset * i) + startPos.y;
                    stone.transform.position = new Vector3(posX, posY, startPos.z);
                }
            }
        }
    }
}
