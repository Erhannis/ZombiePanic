using UnityEngine;

public class ColorScheme {
    public static Color BG = Color.black;
    public static Color ROCK = intRGB(90,90,100);
    public static Color AIR = new Color(0f,0f,0f,0.25f);
    public static Color HUMAN = intRGB(220,200,200);
    public static Color ZOMBIE = intRGB(130,190,130);

    private static Color intRGB(float r, float g, float b) {
        return new Color(r/255,g/255,b/255);
    }
    
    private static Color intRGBA(float r, float g, float b, float a) {
        return new Color(r/255,g/255,b/255,a/255);
    }
}