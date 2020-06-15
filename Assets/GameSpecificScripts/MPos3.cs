
using System;

/**
Mutable Pos3
*/
public class MPos3 {
    public long x;
    public long y;
    public long z;

    public MPos3(long x, long y, long z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Pos3 toPos3() {
        return new Pos3(x,y,z);
    }

    //TODO Merge all these with Pos3

    public static MPos3 operator +(MPos3 a) => a;
    public static MPos3 operator -(MPos3 a) => new MPos3(-a.x, -a.y, -a.z);

    public static MPos3 operator +(MPos3 a, MPos3 b)
        => new MPos3(a.x + b.x, a.y + b.y, a.z + b.z);

    public static MPos3 operator -(MPos3 a, MPos3 b)
        => a + (-b);

    public static MPos3 operator *(MPos3 a, long l)
        => new MPos3(a.x * l, a.y * l, a.z * l);

    public long normL0() {
        long sum = 0;
        foreach (long l in new long[] { x, y, z }) {
            sum += Math.Abs(l);
        }
        return sum;
    }

    public long normLInf() {
        long max = 0;
        foreach (long l in new long[] { x, y, z }) {
            max = Math.Max(Math.Abs(l), max);
        }
        return max;
    }

    public override string ToString() => $"({x}, {y}, {z})";

    public override bool Equals(object obj)
    {
        MPos3 o = obj as MPos3;

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