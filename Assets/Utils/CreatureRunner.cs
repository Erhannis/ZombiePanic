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

//TODO Make the names cooler :P (second difficulty level is "slightly obfuscated"?)

^move(dir) -> bool
^get(dir) -> bool
^put(dir) -> bool
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
^getPos() -> pos3 //TODO Could be relative to a set point, or the hatching point
destroy(???)??? -> bool // (from inventory?) //TODO Health, something?
save(???)??? -> ??? // For saving state, if we can't figure out a better way
load(???)??? -> ??? // Ditto
die()???

Probation:
^replicate(pos3) -> bool //TODO Ehh, maybe for some circumstances, but it doesn't quite fit the narrative of Broodmother, for drones

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
    private readonly Creature creature;
    private readonly ChannelWriter<int> syncA;
    private readonly ChannelReader<int> syncB;

    private bool dead = false;

    public CreatureRunner(Creature creature, ChannelWriter<int> syncA, ChannelReader<int> syncB, string program) : base(program) {
        this.creature = creature;
        this.syncA = syncA;
        this.syncB = syncB;
    }

    override protected (string, Delegate)[] getFunctions() {
        var x = base.getFunctions();
        var y = new (string, Delegate)[] {
            ("move", new Func<Pos3, bool>(move)),
            ("get", new Func<Pos3, bool>(get)),
            ("put", new Func<Pos3, bool>(put)),
            ("getPos", new Func<Pos3>(getPos)),
            ("replicate", new Func<Pos3, bool>(replicate)),
            ("die", new Action(die)) // :(
        };
        var z = new (string, Delegate)[x.Length + y.Length];
        x.CopyTo(z, 0);
        y.CopyTo(z, x.Length);
        return z;
    }

    override public void Run() {
        base.Run();
        syncA.Poison();
        syncB.Poison();
    }

    override protected void handleException(Exception e) {
        base.handleException(e);
        Debug.LogError("run error! " + e);
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
                return Inventories.move(creature, from, to);
            }
            return false;
        } finally {
            int val = syncB.Read();
        }
    }

    //TODO Oh - don't forget this targets blocksMovement() entities.
    private bool get(Pos3 dir) { //TODO Permissions?  Fights?  Health?  Max space?
        if (checkDead()) { //TODO ???
            return false;
        }
        syncA.Write(0);
        try {
            if (dir.normLInf() > 1) {
                return false;
            }

            if (creature.inventory.Count >= Settings.MAX_INVENTORY) {
                return false;
            }

            var parent = creature.parent as Tile;
            if (parent != null && parent.parent != null) {
                Tile target = parent.parent.getTile(parent.pos + dir);
                Entity digged = null;
                foreach (Entity e in target.getInventory()) {
                    if (e.blocksMovement() && e != creature) {
                        digged = e;
                        break;
                    }
                }
                if (digged != null) {
                    return Inventories.move(digged, target, creature);
                } else {
                    return false;
                }
            }
            return false;
        } finally {
            int val = syncB.Read();
        }
    }

    private bool put(Pos3 dir) { //TODO Permissions?  Fights?  Health?  Max space?
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
                if (creature.inventory.Count == 0) {
                    return false;
                }
                Entity placed = creature.inventory[creature.inventory.Count - 1];

                Tile target = parent.parent.getTile(parent.pos + dir);
                if (placed.blocksMovement()) { // Currently preventing two blocking entities from colocating
                    foreach (Entity e in target.getInventory()) {
                        if (e.blocksMovement()) {
                            return false;
                        }
                    }
                }
                return Inventories.move(placed, creature, target);
            }
            return false;
        } finally {
            int val = syncB.Read();
        }
    }

    //TODO Could be relative to a set point, or the hatching point
    private Pos3 getPos() { //TODO READ LOCK
        syncA.Write(0);
        try {
            Tile tile = creature.parent as Tile;
            if (tile != null) {
                return tile.pos;
            }
            //TODO Should return null if captured?
            Inventoried p = creature.parent;
            while (true) {
                if (p is Tile) {
                    return (p as Tile).pos;
                }
                if (p == null) {
                    return null; //TODO ???
                }
                if (p is Entity) {
                    p = (p as Entity).parent;
                } else {
                    return null; //TODO ???
                }
            }
        } finally {
            int val = syncB.Read();
        }
    }

    private bool replicate(Pos3 dir) { //TODO For some reason these cause an almost immediate crash on windows, even with low MAX_RUNNERS?
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
                Tile target = parent.parent.getTile(parent.pos + dir);
                if (creature.blocksMovement()) { // Currently preventing two blocking entities from colocating
                    foreach (Entity e in target.getInventory()) {
                        if (e.blocksMovement()) {
                            return false;
                        }
                    }
                }

                var world = parent.parent;

                //TODO For now I'm just restricting this to drones
                var mother = creature as Drone;
                if (mother == null) {
                    return false;
                }

                var larva = new Drone(null); //TODO Copy, not Drone
                world.getTile(target.pos).addItem(larva);
                world.addRunner(larva, this.program); //TODO Concurrent modification

                return true;
            }
            return false;
        } finally {
            int val = syncB.Read();
        }
    }

    private void die() {
        dead = true;
        syncA.Write(0);
        try {
            creature.parent.removeItem(creature);
        } finally {
            int val = syncB.Read();
        }
    }

    private bool checkDead() { //TODO ???
        return dead;
    }
}

/*

// Spinner
@"let n = Pos3(0,1,0);
let e = Pos3(1,0,0);
let s = Pos3(0,-1,0);
let w = Pos3(-1,0,0);
while (true) {
    move(e);
    move(n);
    move(w);
    move(s);
}"

// Northern dirt-shover
@"
let n = Pos3(0,1,0);
let e = Pos3(1,0,0);
let s = Pos3(0,-1,0);
let w = Pos3(-1,0,0);
let u = Pos3(0,0,1);
let d = Pos3(0,0,-1);
while (true) {
    if (!move(n)) {
        get(n);
        if (!put(e) && !put(w) && !put(u) && !put(d)) {
            put(s);
        }
    }
}"

// Von Neumann
@"let n = Pos3(0,1,0);
let e = Pos3(1,0,0);
let s = Pos3(0,-1,0);
let w = Pos3(-1,0,0);
let u = Pos3(0,0,1);
let d = Pos3(0,0,-1);
replicate(n);
replicate(e);
replicate(s);
replicate(w);
replicate(u);
replicate(d);"

*/
