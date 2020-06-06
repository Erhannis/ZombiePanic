using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
public class Broodmother : Creature
{
    public Broodmother() {
    }

    override protected void renderEntity() {
        GL.Begin(GL.QUADS);
        GL.Color(ColorScheme.BROODMOTHER);
        vertex3(-0.3f, -0.3f, 0);
        vertex3(+0.3f, -0.3f, 0);
        vertex3(+0.3f, +0.3f, 0);
        vertex3(-0.3f, +0.3f, 0);
        GL.End();
    }

    override public bool blocksMovement() {
        return true; //TODO Not sure if like, what if has entity that can move, but does not block movement?  You can move into their space, but they not into yours?
    }
}
}