using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
public class Drone : Creature
{
    public Drone() {
    }

    override protected void renderEntity() {
        GL.Begin(GL.QUADS);
        GL.Color(ColorScheme.DRONE);
        vertex3(-0.2f, -0.2f, 0);
        vertex3(+0.2f, -0.2f, 0);
        vertex3(+0.2f, +0.2f, 0);
        vertex3(-0.2f, +0.2f, 0);
        GL.End();
    }

    override public bool blocksMovement() {
        return true; //TODO Not sure if like, what if has entity that can move, but does not block movement?  You can move into their space, but they not into yours?
    }
}
}