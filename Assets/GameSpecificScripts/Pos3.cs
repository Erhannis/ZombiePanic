using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pos3
{
    public readonly long x;
    public readonly long y;
    public readonly long z;

    public Pos3(long x, long y, long z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public MPos3 toMPos3() {
        return new MPos3(x,y,z);
    }

    public static Pos3 operator +(Pos3 a) => a;
    public static Pos3 operator -(Pos3 a) => new Pos3(-a.x, -a.y, -a.z);

    public static Pos3 operator +(Pos3 a, Pos3 b)
        => new Pos3(a.x + b.x, a.y + b.y, a.z + b.z);

    public static Pos3 operator -(Pos3 a, Pos3 b)
        => a + (-b);

    public static Pos3 operator *(Pos3 a, long l)
        => new Pos3(a.x * l, a.y * l, a.z * l);

    public override string ToString() => $"({x}, {y}, {z})";

    public override bool Equals(object obj)
    {
        Pos3 o = obj as Pos3;

        if (o == null) 
        {
           return false;
        }

        return o.x == this.x && o.y == this.y && o.z == this.z;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(this.x);
        hash.Add(this.y);
        hash.Add(this.z);
        return hash.ToHashCode();
    }
}
