using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool collapsed;
    public int id;
    public List<WeightedTile> tileOptions;

    public void CreateCell(bool collapseState, List<WeightedTile> tiles)
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

    public void ResetCell()
    {
        tileOptions.Clear();
    }

    public void RecreateCell(List<WeightedTile> tiles)
    {
        tileOptions = tiles;
    }

}
