using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles {
public class Rock : Tile
{
    public Rock(Pos3 pos) : base(pos) {
    }

    public void render(Pos3 center) {
        base.render(center);
        GL.Begin(GL.QUADS);
        GL.Color(warpColor(center, ColorScheme.ROCK));
        GL.Vertex3(-0.5f, -0.5f, pos.z - center.z);
        GL.Vertex3(0.5f, -0.5f, pos.z - center.z);
        GL.Vertex3(0.5f, 0.5f, pos.z - center.z);
        GL.Vertex3(-0.5f, 0.5f, pos.z - center.z);
        GL.End();
    }
}
}