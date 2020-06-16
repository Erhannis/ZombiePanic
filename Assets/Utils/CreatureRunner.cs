using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jibu;
using Entities;
using System;
using System.Runtime.InteropServices;

/*
Ideas for interface
Ugh, making this "safe" is gonna be hard - maybe infeasible?  Embrace it?

params?

//TODO MAKE SURE TO SYNC READ/WRITE LOCK ON THESE ACTIONS!

move(dir) -> bool
get(dir) -> bool
put(dir) -> bool
canMove(dir) -> bool
feel(dir) -> ???
look(dir) -> ???
smell(???)??? -> ??? // See `pheromone`
inventory() -> ???
invCount() -> int
sleep(turns) -> bool?
getAge()??? -> int
getTime()??? -> int
speak(???)??? -> ??? //TODO Now, surely this must be "chitter" or "pheromone" or something.  :)
broadcast(???)??? -> ??? //TODO Ditto
listen(???)??? -> ??? //TODO Ditto
pheromone(???)??? -> ??? // This could be for marking tiles
home()??? -> pos3
die()???

debugging?
getError()
getOrders()
getState()
getLogs()
reset()
resorb() // decommission, kill, eat, etc.  ..."filicide" acheivement, horrible crunching noise???  0_0
carry()

*/
public class CreatureRunner : JintRunner {
    private Creature creature;
    private ChannelWriter<int> syncA;
    private ChannelReader<int> syncB;

    public CreatureRunner(Creature creature, ChannelWriter<int> syncA, ChannelReader<int> syncB, string program) : base(program) {
        this.creature = creature;
        this.syncA = syncA;
        this.syncB = syncB;
    }

    override protected (string, Delegate)[] getFunctions() {
        var x = base.getFunctions();
        var y = new (string, Delegate)[] {
            ("move", new Func<Pos3, bool>(move))
        };
        var z = new (string, Delegate)[x.Length + y.Length];
        x.CopyTo(z, 0);
        y.CopyTo(z, x.Length);
        return z;
    }

    override protected void handleException(Exception e) {
        base.handleException(e);
        Debug.LogError("run error! " + e);
        syncA.Poison();
        syncB.Poison();
    }

    private bool move(Pos3 dir) {
        if (checkDead()) { //TODO ???
            return false;
        }
        syncA.Write(0);
        try {
            if (dir.normLInf() > 1) {
                return false;
            }

            var parent = creature.parent as Tile;
            if (parent != null && parent.parent != null) {
                Tile from = parent;
                Tile to = parent.parent.getTile(parent.pos + dir);
                foreach (Entity e in to.getInventory()) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                from.removeItem(creature);
                to.addItem(creature);
            }
            return true;
        } finally {
            int val = syncB.Read();
        }
    }

    private bool checkDead() { //TODO ???
        return false;
    }
}
