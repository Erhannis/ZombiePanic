using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Inventoried {
    IEnumerable<Entity> getInventory();

    void addItem(Entity entity);

    bool removeItem(Entity entity);
}

//TODO Move somewhere else?
public class Inventories {
    public static bool move(Entity item, Inventoried from, Inventoried to) {
        if (from.removeItem(item)) {
            to.addItem(item);
            return true;
        } else {
            return false;
        }
    }
}