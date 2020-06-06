using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles {
public class Air : Tile
{
    public Air(Pos3 pos) : base(pos) {
    }

    override public void render(Pos3 center) {
        base.render(center);
        //TODO *Could* do depth shading by making air slightly visible
        GL.Begin(GL.QUADS);
        GL.Color(ColorScheme.AIR);
        float ox = (float)(pos.x - center.x);
        float oy = (float)(pos.y - center.y);
        float oz = (float)(pos.z - center.z);
        GL.Vertex3(ox-0.5f, oy-0.5f, oz);
        GL.Vertex3(ox+0.5f, oy-0.5f, oz);
        GL.Vertex3(ox+0.5f, oy+0.5f, oz);
        GL.Vertex3(ox-0.5f, oy+0.5f, oz);
        GL.End();
    }
}
}