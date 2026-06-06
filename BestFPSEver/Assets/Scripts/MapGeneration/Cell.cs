using System.Collections.Generic;

public class Cell
{
    public bool collapsed;

    public List<Tile> possibleTiles;

    public Cell(List<Tile> tiles)
    {
        collapsed = false;
        possibleTiles = new List<Tile>(tiles);
    }
}