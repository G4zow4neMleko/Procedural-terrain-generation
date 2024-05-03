using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public struct WeightedTile{
    public WeightedTile(Tile t, int w) { tile = t; weight = w; }
    public static bool operator ==(WeightedTile left, WeightedTile right)
    {
        return left.tile == right.tile;
    }
    public static bool operator !=(WeightedTile left, WeightedTile right)
    {
        return left.tile != right.tile;
    }

    public Tile tile;
    public int weight;

    public override bool Equals(object obj)
    {
        return obj is WeightedTile tile &&
               EqualityComparer<Tile>.Default.Equals(this.tile, tile.tile);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(tile);
    }
}

public class Tile : MonoBehaviour
{   
    public List<Tile> neighbours;
    public List<int> weights;
}
