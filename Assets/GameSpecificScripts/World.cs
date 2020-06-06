using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tiles;

/**
    Infinite 3D world.  Centered at 0,0,0.
    Currently cubic grid, because easiest.
*/
public class World
{
    //TODO Woof.  Make a good caching mechanism.
    //TODO Save world!
    public Dictionary<Pos3, Tile> tiles = new Dictionary<Pos3, Tile>();

    public Tile genTile(Pos3 pos) {
        return new Rock(pos); //TODO Make better world, haha
    }

    public Tile getTile(Pos3 pos) {
        Tile tile;
        if (!tiles.TryGetValue(pos, out tile)) {
            tile = genTile(pos);
            tiles[pos] = tile;
        }
        return tile;
    }

    public void render(Pos3 center, Pos3 downWestSouth, Pos3 upEastNorth) {
        //TODO Scale?
        for (long z = downWestSouth.z; z <= upEastNorth.z; z++) {
            for (long y = downWestSouth.y; y <= upEastNorth.y; y++) {
                for (long x = downWestSouth.x; x <= upEastNorth.x; x++) {
                    getTile(new Pos3(x,y,z)).render(center);
                    //TODO ???
                }
            }
        }
    }
}
