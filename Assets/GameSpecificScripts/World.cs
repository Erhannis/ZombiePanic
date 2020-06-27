using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Jibu;

/**
    Infinite 3D world.  Centered at 0,0,0.
    Currently cubic grid, because easiest.
*/
public class World {
    //TODO Woof.  Make a good caching mechanism.
    //TODO Save world!
    public Dictionary<Pos3, Tile> tiles = new Dictionary<Pos3, Tile>();
    private System.Random rand = new System.Random(); //TODO Use a seed or something
    private Stack<(JintRunner, ChannelReader<int>, ChannelWriter<int>)> pendingRunners = new Stack<(JintRunner, ChannelReader<int>, ChannelWriter<int>)>(); // Why stack?  Dunno!
    public List<(JintRunner, ChannelReader<int>, ChannelWriter<int>)> runners = new List<(JintRunner, ChannelReader<int>, ChannelWriter<int>)>();

    private readonly float rockDensity;
    private readonly float zombieDensity;

    public World(float rockDensity, float zombieDensity) {
        this.rockDensity = rockDensity;
        this.zombieDensity = zombieDensity;
    }

    public Tile genTile(Pos3 pos) {
        if (pos.z != 0) {
            return makeTile(this, pos, new Air(null), new Rock(null));
        }

        if (rand.NextDouble() <= zombieDensity) { // Zombies trump rock
            return makeTile(this, pos, new Air(null), new Zombie(null)); //TODO It's kindof annoying that my addition of a bidirectional reference broke the tidy creation, here
        } else if (rand.NextDouble() <= rockDensity) {
            return makeTile(this, pos, new Air(null), new Rock(null));
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
    public void addRunner(Creature creature, string program) {
        Channel<int> syncA = new Channel<int>();
        Channel<int> syncB = new Channel<int>();
        if (runners.Count + pendingRunners.Count < Settings.MAX_RUNNERS) {
            CreatureRunner cr = new CreatureRunner(creature, syncA.ChannelWriter, syncB.ChannelReader, program);
            pendingRunners.Push((cr, syncA.ChannelReader, syncB.ChannelWriter));
            cr.Start();
        } else {
            //TODO Notify failure?
        }
    }

    public void stepRunners() {
        if (pendingRunners.Count > 0) { // Dunno if this saves any time or not
            runners.AddRange(pendingRunners);
            pendingRunners.Clear();
        }
        runners.RemoveAll(p => { // This is a bit misleading - we're only removing things if they break
            try {
                p.Item2.Read();
                p.Item3.Write(0);
                return false;
            } catch (PoisonException e) {
                Debug.LogError(e);
                return true;
            }
        });
    }

    public void render(Pos3 center, Pos3 downWestSouth, Pos3 upEastNorth) {
        //TODO Scale?
        //Debug.Log("world render " + center + " " + downWestSouth + "->" + upEastNorth);
        for (long z = downWestSouth.z; z <= upEastNorth.z; z++) {
            for (long y = downWestSouth.y; y <= upEastNorth.y; y++) {
                for (long x = downWestSouth.x; x <= upEastNorth.x; x++) {
                    getTile(new Pos3(x, y, z)).render(center);
                    //TODO ???
                }
            }
        }
    }
}

/*
Ideas:
    looping world
    non-cubic tiles
    hyperbolic geometry
 */
