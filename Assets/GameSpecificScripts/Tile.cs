using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
    Tiles are 1 unit wide, from [-0.5,-0.5]to(0.5,0.5).  Not gonna think about inclusivity directly until it clearly matters.
*/
public class Tile
{
    public Pos3 pos;

    public Tile(Pos3 pos) {
        this.pos = pos; //TODO readonly?  setter?  updates?
    }

    public virtual void render(Pos3 center) {
        //TODO BG or something?
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
