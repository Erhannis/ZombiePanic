/**
    A poor substitute for the HashCode of .NET 2.1

    The logic was derived from https://stackoverflow.com/a/34006336/513038
*/
public class HashCode {
    private const int SEED = 1009;
    private const int FACTOR = 9176;

    private int hash;
    public HashCode() {
        hash = SEED;
    }

    public int Add(long i) {
        hash = (hash * FACTOR) + ((int)i);
        return hash;
    }

    public int ToHashCode() {
        return hash;
    }

    // https://stackoverflow.com/a/34006336/513038
    //seed = 1009, factor = 9176
    public static int CustomHash(int seed, int factor, params int[] vals) {
        int hash = seed;
        foreach (int i in vals)
        {
            hash = (hash * factor) + i;
        }
        return hash;
    }
}