using UnityEngine;

public class ColorScheme {
    public static Color BG = Color.black;
    public static Color ROCK = intRGB(100,70,90);
    public static Color AIR = new Color(0f,0f,0f,0.25f);
    public static Color BROODMOTHER = intRGB(160,100,160);

    private static Color intRGB(float r, float g, float b) {
        return new Color(r/255,g/255,b/255);
    }
    private static Color intRGBA(float r, float g, float b, float a) {
        return new Color(r/255,g/255,b/255,a/255);
    }
}