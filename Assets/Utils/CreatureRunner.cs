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
getPos() -> pos3
destroy(???)??? -> bool // (from inventory?) //TODO Health, something?
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
            ("move", new Func<Pos3, bool>(move)),
            ("get", new Func<Pos3, bool>(get)),
            ("put", new Func<Pos3, bool>(put))
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

    private bool get(Pos3 dir) { //TODO Permissions?  Fights?  Health?  Max space?
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
                Entity digged = null;
                foreach (Entity e in target.getInventory()) {
                    if (e.blocksMovement()) {
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
                Tile target = parent.parent.getTile(parent.pos + dir);
                foreach (Entity e in target.getInventory()) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                if (creature.inventory.Count == 0) {
                    return false;
                }
                Entity placed = creature.inventory[creature.inventory.Count - 1];
                return Inventories.move(placed, creature, target);
            }
            return false;
        } finally {
            int val = syncB.Read();
        }
    }

    private bool checkDead() { //TODO ???
        return false;
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

*/
