using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WaveFunction : MonoBehaviour
{

    public int gridSize = 10;
    public List<WeightedTile> tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    // Start is called before the first frame update
    void Start()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for (int y=0; y < gridSize; ++y)
        {
            for(int x=0; x<gridSize; ++x)
            {
                Cell cell = Instantiate(cellObj, new Vector2((x - gridSize / 2) * 0.32f, (y - gridSize / 2) * 0.32f), Quaternion.identity);
                cell.transform.SetParent(this.transform, false);
                cell.CreateCell(false, tileObjects);
                cell.id = x + y * gridSize;
                gridComponents.Add(cell);
            }
        }
/*        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        timer.Start();*/
        CheckEntropy();
/*        timer.Stop();
        print("Time passed: " + timer.ElapsedMilliseconds);*/
    }

    void CheckEntropy()
    {

        List<Cell> tempGrid = new List<Cell>(gridComponents);
        int index = UnityEngine.Random.Range(0, tempGrid.Count);
        CollapseCell(tempGrid[index]);
        tempGrid.RemoveAt(index);
        tempGrid.Sort((a, b) => { return a.tileOptions.Count - b.tileOptions.Count; });

        while (tempGrid.Count > 0)
        {
            CollapseCell(tempGrid[0]);

            tempGrid.RemoveAt(0);
            tempGrid.Sort((a, b) => { return a.tileOptions.Count - b.tileOptions.Count; });
        }
    }

    void CollapseCell(Cell cellToCollapse)
    {
        cellToCollapse.collapsed = true;
        WeightedTile selectedTile;

        //selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Count)];

        selectedTile = RandomWeightedTile(cellToCollapse);
        //int weight = cellToCollapse.tileWeights[Array.FindIndex(cellToCollapse.tileOptions.ToArray(), obj => obj == selectedTile)];

        //cellToCollapse.ResetCell();
        cellToCollapse.RecreateCell(new List<WeightedTile> { selectedTile });
        Instantiate(selectedTile.tile).transform.SetParent(cellToCollapse.transform, false);

        //UpdateGeneration();
        UpdateCells();
    }

    void UpdateCells()
    {
        foreach (Cell cellToUpdate in gridComponents)
        {
            if (cellToUpdate.collapsed) continue;

            //cellToUpdate.ResetCell();

            //up
            if (cellToUpdate.id >= gridSize)
            {
                List<WeightedTile> possibilities = new List<WeightedTile>();
                foreach (WeightedTile validOptions in gridComponents[cellToUpdate.id - gridSize].tileOptions)
                {
                    for(int i = 0; i < validOptions.tile.neighbours.Count; ++i)
                    {
                        possibilities.Add(new WeightedTile(validOptions.tile.neighbours[i], validOptions.tile.weights[i]));
                    }
                }
                Accumulate(possibilities);
                cellToUpdate.RecreateCell( CheckValidity(cellToUpdate.tileOptions, possibilities));
            }
            //down
            if (cellToUpdate.id + gridSize < gridSize * gridSize)
            {
                List<WeightedTile> possibilities = new List<WeightedTile>();
                foreach (WeightedTile validOptions in gridComponents[cellToUpdate.id + gridSize].tileOptions)
                {
                    for (int i = 0; i < validOptions.tile.neighbours.Count; ++i)
                    {
                        possibilities.Add(new WeightedTile(validOptions.tile.neighbours[i], validOptions.tile.weights[i]));
                    }
                }
                Accumulate(possibilities);
                cellToUpdate.RecreateCell(CheckValidity(cellToUpdate.tileOptions, possibilities));
            }
            //left
            if (cellToUpdate.id > 0)
            {
                List<WeightedTile> possibilities = new List<WeightedTile>();
                foreach (WeightedTile validOptions in gridComponents[cellToUpdate.id - 1].tileOptions)
                {
                    for (int i = 0; i < validOptions.tile.neighbours.Count; ++i)
                    {
                        possibilities.Add(new WeightedTile(validOptions.tile.neighbours[i], validOptions.tile.weights[i]));
                    }
                }
                Accumulate(possibilities);
                cellToUpdate.RecreateCell(CheckValidity(cellToUpdate.tileOptions, possibilities));
            }
            //right
            if (cellToUpdate.id < gridSize * gridSize - 1)
            {
                List<WeightedTile> possibilities = new List<WeightedTile>();
                foreach (WeightedTile validOptions in gridComponents[cellToUpdate.id + 1].tileOptions)
                {
                    for (int i = 0; i < validOptions.tile.neighbours.Count; ++i)
                    {
                        possibilities.Add(new WeightedTile(validOptions.tile.neighbours[i], validOptions.tile.weights[i]));
                    }
                }
                Accumulate(possibilities);
                cellToUpdate.RecreateCell(CheckValidity(cellToUpdate.tileOptions, possibilities));
            }
        }
    }

/*    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for(int y=0; y < gridSize; ++y)
        {
            for(int x=0; x < gridSize; ++x) 
            {
                var index = x + y * gridSize;
                if (gridComponents[index].collapsed) 
                {
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = tileObjects.ToList();
                    
                    if(y>0) 
                    {
                        Cell up = gridComponents[x+(y-1)*gridSize];
                        List<Tile> validOptions = new List<Tile>();
                        foreach(Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = System.Array.FindIndex(tileObjects.ToArray(), obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].neighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        options = CheckValidity(options, validOptions);
                    }

                    if (x < gridSize-1)
                    {
                        Cell up = gridComponents[x + 1 + y * gridSize];
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = System.Array.FindIndex(tileObjects.ToArray(), obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].neighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        options = CheckValidity(options, validOptions);
                    }


                    if (y < gridSize - 1)
                    {
                        Cell up = gridComponents[x + (y + 1) * gridSize];
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = System.Array.FindIndex(tileObjects.ToArray(), obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].neighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        options = CheckValidity(options, validOptions);
                    }

                    if (x > 0)
                    {
                        Cell up = gridComponents[x - 1 + y * gridSize];
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOptions in up.tileOptions)
                        {
                            var valOption = System.Array.FindIndex(tileObjects.ToArray(), obj => obj == possibleOptions);
                            var valid = tileObjects[valOption].neighbours;

                            validOptions = validOptions.Concat(valid).ToList();
                        }

                        options = CheckValidity(options, validOptions);
                    }

                    List<Tile> newTileList = options;

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }
        gridComponents = newGenerationCell;
    }*/

    WeightedTile RandomWeightedTile(Cell cell)
    {
        List<WeightedTile> tmpList = new List<WeightedTile>();
        for(int i=0; i<cell.tileOptions.Count; ++i)
        {
            for(int j=0; j < cell.tileOptions[i].weight; ++j)
            {
                tmpList.Add(cell.tileOptions[i]);
            }
        }

        return tmpList[UnityEngine.Random.Range(0, tmpList.Count)];
    }

    void Accumulate(List<WeightedTile> list)
    {
        List<WeightedTile> result = new List<WeightedTile>();
        foreach(var l in list)
        {
            if (result.Contains(l))
            {
                int idx = Array.FindIndex(result.ToArray(), obj => obj.tile == l.tile);
                int w = result[idx].weight + l.weight; 
                result[idx] = new WeightedTile(l.tile, w);
            }
            else
            {
                result.Add(l);
            }
        }
        list = result;
    }
    
    List<WeightedTile> CheckValidity(List<WeightedTile> optionList, List<WeightedTile> validOptions)
    {
        List<WeightedTile> result = new List<WeightedTile>();
        foreach(WeightedTile tile in optionList) 
        {
            if (validOptions.Contains(tile))
            {
                int idx = Array.FindIndex(validOptions.ToArray(), obj => obj.tile == tile.tile);
                result.Add(new WeightedTile(tile.tile, tile.weight + validOptions[idx].weight));
            }
        }
        return result;
    }
}
