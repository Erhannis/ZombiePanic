using UnityEngine;
using System.Threading;

namespace Entities {
public abstract class Entity {
    //SPEED It would be even more horrible, but assuming a single threaded environment, you could swap the ThreadLocal for a plain Vector3.
    private ThreadLocal<Vector3> offset = new ThreadLocal<Vector3>(); // This may be one of the worst things I've done in programming.

    public void render(Vector3 offset) {
        this.offset.Value = offset;
        renderEntity();
    }

    protected abstract void renderEntity();
    public abstract bool blocksMovement();

    /**
        Must only be called when render() is further root-ward in the stack.
    */
    public void vertex3(float x, float y, float z) {
        Vector3 o = offset.Value;
        GL.Vertex3(x+o.x,y+o.y,z+o.z);
    }
}
}