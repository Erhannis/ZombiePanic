using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jibu;
using Entities;
using System;
using System.Runtime.InteropServices;

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
        Debug.Log("move 0");
        if (checkDead()) { //TODO ???
            return false;
        }
        Debug.Log("move start");
        syncA.Write(0);
        try {
            if (dir.normLInf() > 1) {
                return false;
            }

            Debug.Log("move " + dir);

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
            Debug.Log("move end");
        }
    }

    private bool checkDead() { //TODO ???
        return false;
    }
}
