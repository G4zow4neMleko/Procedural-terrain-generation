using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UIElements;

public class NoiseGenerator : MonoBehaviour
{
    public int RenderDistance;
    public int chunkSize;
    public float chunkScale;
    public Vector2 initialOffset;
    public Tile[] tiles;

    [SerializeField]
    private GameObject[,] chunks;

    // Start is called before the first frame update
    void Start()
    {
        if (initialOffset.x == 0) initialOffset.x = UnityEngine.Random.Range(-10000000, 10000000);
        if (initialOffset.y == 0) initialOffset.y = UnityEngine.Random.Range(-10000000, 10000000);
        initializeGrid();
    }

    private void Update()
    {

        int xshift = 0;
        int yshift = 0;
        float maxDistToCamera = 0.32f * chunkSize * (RenderDistance/2 + 1);

        if ((chunks[RenderDistance / 2, 0].transform.position - Camera.main.transform.position).magnitude > maxDistToCamera)
        {
            xshift = -1;
        }
        else if ((chunks[RenderDistance / 2, RenderDistance - 1].transform.position - Camera.main.transform.position).magnitude > maxDistToCamera)
        {
            xshift = 1;
        }

        if ((chunks[0, RenderDistance / 2].transform.position - Camera.main.transform.position).magnitude > maxDistToCamera)
        {
            yshift = -1;
        }
        else if ((chunks[RenderDistance - 1, RenderDistance / 2].transform.position - Camera.main.transform.position).magnitude > maxDistToCamera)
        {
            yshift = 1;
        }

        if (yshift == 0 && xshift == 0)
            return;

        GameObject[,] tmpChunks = new GameObject[RenderDistance, RenderDistance];

        for (int y = 1; y < RenderDistance-1; ++y)
        {
            for (int x = 1; x < RenderDistance-1; ++x)
            {
                tmpChunks[y + yshift, x + xshift] = chunks[y, x]; 
            }
        }

        if(xshift != 0)
        {
            int side = 0;
            if (xshift < 0) side = RenderDistance - 1;
            
            for(int x=1; x<RenderDistance-1; ++x)
            {
                tmpChunks[0,x + xshift] = chunks[0, x];
                tmpChunks[RenderDistance-1, x + xshift] = chunks[RenderDistance - 1, x];
            }
            
            for (int y=0; y<RenderDistance; ++y)
            {
                GameObject newChunk = new GameObject();
                GameObject sideChunk = chunks[y, side];
                tmpChunks[y, xshift + side] = chunks[y, side];

                newChunk.AddComponent<NoiseChunk>();
                Vector2 offset = sideChunk.GetComponent<NoiseChunk>().offset;

                newChunk.GetComponent<NoiseChunk>().Create(chunkSize, chunkScale, new Vector2(offset.x - (xshift * chunkSize), offset.y), tiles);
                newChunk.transform.position = new Vector3(sideChunk.transform.position.x - (xshift * chunkSize * 0.32f), sideChunk.transform.position.y, 0);
                newChunk.transform.SetParent(transform);
                tmpChunks[y, side] = newChunk;

                Destroy(chunks[y, RenderDistance - 1 - side]);
            }
        }

        if (yshift != 0)
        {
            int side = 0;
            if (yshift < 0) side = RenderDistance - 1;

            for (int y = 1; y < RenderDistance - 1; ++y)
            {
                tmpChunks[y+yshift, 0] = chunks[y, 0];
                tmpChunks[y+yshift, RenderDistance - 1] = chunks[y, RenderDistance - 1];
            }

            for (int x = 0; x < RenderDistance; ++x)
            {
                if (tmpChunks[side, x]) continue;

                GameObject obj = new GameObject();
                GameObject sideChunk = chunks[side, x];
                tmpChunks[yshift + side, x] = sideChunk;

                obj.AddComponent<NoiseChunk>();
                Vector2 offset = sideChunk.GetComponent<NoiseChunk>().offset;

                obj.GetComponent<NoiseChunk>().Create(chunkSize, chunkScale, new Vector2(offset.x, offset.y - (yshift * chunkSize)), tiles);
                obj.transform.position = new Vector3(sideChunk.transform.position.x, sideChunk.transform.position.y - (yshift * chunkSize * 0.32f), 0);
                obj.transform.SetParent(transform);
                tmpChunks[side, x] = obj;

                Destroy(chunks[RenderDistance - 1 - side, x]);
            }
        }

        chunks = tmpChunks;
    }

    void initializeGrid()
    {
        chunks = new GameObject[RenderDistance, RenderDistance];
        for (int y = 0; y < RenderDistance; y++)
        {
            for (int x = 0; x < RenderDistance; x++)
            {
                GameObject obj = new GameObject();
                obj.AddComponent<NoiseChunk>();

                Vector2 offset = initialOffset;
                offset.x = offset.x + chunkSize * (x - RenderDistance / 2);
                offset.y = offset.y + chunkSize * (y - RenderDistance / 2);

                obj.GetComponent<NoiseChunk>().Create(chunkSize, chunkScale, offset, tiles);
                obj.transform.position = new Vector3(0.32f * chunkSize * (x - RenderDistance / 2), 0.32f * chunkSize * (y - RenderDistance / 2), 0);
                chunks[y,x] = obj;
                obj.transform.SetParent(transform);
            }
        }
    }

}
