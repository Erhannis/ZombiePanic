using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities {
public class Air : Entity
{
    public Air() {
    }

    override protected void renderEntity() {
        //TODO *Could* do depth shading by making air slightly visible
        GL.Begin(GL.QUADS);
        GL.Color(ColorScheme.AIR);
        vertex3(-0.5f, -0.5f, 0);
        vertex3(+0.5f, -0.5f, 0);
        vertex3(+0.5f, +0.5f, 0);
        vertex3(-0.5f, +0.5f, 0);
        GL.End();
    }
}
}