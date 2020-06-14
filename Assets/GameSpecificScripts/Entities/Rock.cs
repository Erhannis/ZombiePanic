using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
public class Rock : Entity
{
    public Rock(Inventoried parent) : base(parent) {
    }

    override protected void renderEntity() {
        GL.Begin(GL.QUADS);
        GL.Color(ColorScheme.ROCK);
        vertex3(-0.5f, -0.5f, 0);
        vertex3(+0.5f, -0.5f, 0);
        vertex3(+0.5f, +0.5f, 0);
        vertex3(-0.5f, +0.5f, 0);
        GL.End();
    }

    override public bool blocksMovement() {
        return true;
    }
}
}