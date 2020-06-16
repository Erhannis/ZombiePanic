using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

/**
    Tiles are 1 unit wide, from [-0.5,-0.5]to(0.5,0.5).  Not gonna think about inclusivity directly until it clearly matters.
*/
public class Tile : Inventoried
{
    public World parent;

    public Pos3 pos;
    public List<Entity> contents = new List<Entity>();

    public Tile(World parent, Pos3 pos, Entity[] contents) {
        this.parent = parent;
        this.pos = pos; //TODO readonly?  setter?  updates?
        this.contents.AddRange(contents);
    }

    public void addItem(Entity entity) {
        contents.Add(entity);
        entity.parent = this;
    }

    public IEnumerable<Entity> getInventory() {
        return contents; //TODO Should not allow modification?
    }

    public bool removeItem(Entity entity) {
        if (contents.Remove(entity)) {
            entity.parent = null;
            return true;
        } else {
            return false;
        }
    }

    public void render(Pos3 center) {
        //TODO BG or something?
        Pos3 o = pos - center;
        float oz = 0;
        float dz = 0.2f / (contents.Count+1);
        foreach (Entity entity in contents) {
            entity.render(new Vector3(o.x,o.y,o.z+dz));
            oz += dz;
        }
    }

    public Color warpColor(Pos3 center, Color color) { //TODO Maybe a shader?
        return color;
        // if (pos.z > center.z) {
        //     return (color + Color.white)/2;
        // } else if (pos.z < center.z) {
        //     return (color + Color.black)/2;
        // } else {
        //     return color;
        // }
    }
}
