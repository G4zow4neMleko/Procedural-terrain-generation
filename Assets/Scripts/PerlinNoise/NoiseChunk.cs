using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NoiseChunk : MonoBehaviour
{
    public int gridSize;
    public float scale;
    public Vector2 offset;

    public Tile[] tiles;
    public List<Tile> gridComponents;

    // Start is called before the first frame update
    public void Create(int _gridSize, float _scale, Vector2 _offset, Tile[] _tiles)
    {
        gridSize = _gridSize;
        scale = _scale;
        offset = _offset;
        tiles = _tiles;
        gridComponents = new List<Tile>();
        InitializeGrid();
    }

    void Update()
    {
    }

    public void UpdateChunk()
    {
        int idx = 0;
        for (int y = 0; y < gridSize; ++y)
        {
            for (int x = 0; x < gridSize; ++x)
            {
                idx = Mathf.Clamp((int)(NoiseFunction((x + offset.x) * scale, (y + offset.y) * scale) * tiles.Length), 0, tiles.Length - 1);
                gridComponents[x + y * gridSize].GetComponent<SpriteRenderer>().sprite = tiles[idx].GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    public float NoiseFunction(float x, float y)
    {
       float result = 0.2f * Mathf.Clamp(Mathf.PerlinNoise(x * 0.5f, y * 0.5f) + 0.1f * Mathf.PerlinNoise(x * 2, y * 2) + 0.3f * Mathf.PerlinNoise(x, y), 0, 1);

       return result;
    }

    private void InitializeGrid()
    {
        int idx = 0;
        for (int y = 0; y < gridSize; ++y)
        {
            for (int x = 0; x < gridSize; ++x)
            {
                idx = Mathf.Clamp((int)(Mathf.PerlinNoise((x + offset.x) * scale, (y + offset.y) * scale) * tiles.Length), 0, tiles.Length - 1);
                Tile tile = Instantiate(tiles[idx], new Vector2((x - gridSize / 2) * 0.32f, (y - gridSize / 2) * 0.32f), Quaternion.identity);
                tile.transform.SetParent(this.transform, false);
                gridComponents.Add(tile);
            }
        }
    }
}
