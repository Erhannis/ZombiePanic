using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

/**
    Infinite 3D world.  Centered at 0,0,0.
    Currently cubic grid, because easiest.
*/
public class World
{
    //TODO Woof.  Make a good caching mechanism.
    //TODO Save world!
    public Dictionary<Pos3, Tile> tiles = new Dictionary<Pos3, Tile>();
    private System.Random rand = new System.Random(); //TODO Use a seed or something

    public Tile genTile(Pos3 pos) {
        //TODO Make better world, haha
        
        if (rand.Next(0,6) == 0) {
        //if (pos.x == pos.z) {
            return makeTile(this, pos, new Air(null), new Rock(null)); //TODO It's kindof annoying that my addition of a bidirectional reference broke the tidy creation, here
        } else {
            return makeTile(this, pos, new Air(null));
        }
    }

    private static Tile makeTile(World parent, Pos3 pos, params Entity[] entities) { //TODO This seems much slower than the previous way.  :\
        Tile tile = new Tile(parent, pos, new Entity[0]);
        foreach (Entity entity in entities) {
            tile.addItem(entity);
        }
        return tile;
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
        //Debug.Log("world render " + center + " " + downWestSouth + "->" + upEastNorth);
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
