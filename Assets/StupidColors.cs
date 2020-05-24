using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidColors
{
public static double[,] CIE_COLOR_MATCH = {
    {0.0014, 0.0000, 0.0065}, {0.0022, 0.0001, 0.0105}, {0.0042, 0.0001, 0.0201},
    {0.0076, 0.0002, 0.0362}, {0.0143, 0.0004, 0.0679}, {0.0232, 0.0006, 0.1102},
    {0.0435, 0.0012, 0.2074}, {0.0776, 0.0022, 0.3713}, {0.1344, 0.0040, 0.6456},
    {0.2148, 0.0073, 1.0391}, {0.2839, 0.0116, 1.3856}, {0.3285, 0.0168, 1.6230},
    {0.3483, 0.0230, 1.7471}, {0.3481, 0.0298, 1.7826}, {0.3362, 0.0380, 1.7721},
    {0.3187, 0.0480, 1.7441}, {0.2908, 0.0600, 1.6692}, {0.2511, 0.0739, 1.5281},
    {0.1954, 0.0910, 1.2876}, {0.1421, 0.1126, 1.0419}, {0.0956, 0.1390, 0.8130},
    {0.0580, 0.1693, 0.6162}, {0.0320, 0.2080, 0.4652}, {0.0147, 0.2586, 0.3533},
    {0.0049, 0.3230, 0.2720}, {0.0024, 0.4073, 0.2123}, {0.0093, 0.5030, 0.1582},
    {0.0291, 0.6082, 0.1117}, {0.0633, 0.7100, 0.0782}, {0.1096, 0.7932, 0.0573},
    {0.1655, 0.8620, 0.0422}, {0.2257, 0.9149, 0.0298}, {0.2904, 0.9540, 0.0203},
    {0.3597, 0.9803, 0.0134}, {0.4334, 0.9950, 0.0087}, {0.5121, 1.0000, 0.0057},
    {0.5945, 0.9950, 0.0039}, {0.6784, 0.9786, 0.0027}, {0.7621, 0.9520, 0.0021},
    {0.8425, 0.9154, 0.0018}, {0.9163, 0.8700, 0.0017}, {0.9786, 0.8163, 0.0014},
    {1.0263, 0.7570, 0.0011}, {1.0567, 0.6949, 0.0010}, {1.0622, 0.6310, 0.0008},
    {1.0456, 0.5668, 0.0006}, {1.0026, 0.5030, 0.0003}, {0.9384, 0.4412, 0.0002},
    {0.8544, 0.3810, 0.0002}, {0.7514, 0.3210, 0.0001}, {0.6424, 0.2650, 0.0000},
    {0.5419, 0.2170, 0.0000}, {0.4479, 0.1750, 0.0000}, {0.3608, 0.1382, 0.0000},
    {0.2835, 0.1070, 0.0000}, {0.2187, 0.0816, 0.0000}, {0.1649, 0.0610, 0.0000},
    {0.1212, 0.0446, 0.0000}, {0.0874, 0.0320, 0.0000}, {0.0636, 0.0232, 0.0000},
    {0.0468, 0.0170, 0.0000}, {0.0329, 0.0119, 0.0000}, {0.0227, 0.0082, 0.0000},
    {0.0158, 0.0057, 0.0000}, {0.0114, 0.0041, 0.0000}, {0.0081, 0.0029, 0.0000},
    {0.0058, 0.0021, 0.0000}, {0.0041, 0.0015, 0.0000}, {0.0029, 0.0010, 0.0000},
    {0.0020, 0.0007, 0.0000}, {0.0014, 0.0005, 0.0000}, {0.0010, 0.0004, 0.0000},
    {0.0007, 0.0002, 0.0000}, {0.0005, 0.0002, 0.0000}, {0.0003, 0.0001, 0.0000},
    {0.0002, 0.0001, 0.0000}, {0.0002, 0.0001, 0.0000}, {0.0001, 0.0000, 0.0000},
    {0.0001, 0.0000, 0.0000}, {0.0001, 0.0000, 0.0000}, {0.0000, 0.0000, 0.0000}
  };

  public static double[] spectrum_to_xyz(Func<double,double> specIntens) {
    int i;
    double lambda, X = 0, Y = 0, Z = 0, XYZ;

    for (i = 0, lambda = 380; lambda < 780.1; i++, lambda += 5) {
      double Me;

      Me = specIntens(lambda);
      X += Me * CIE_COLOR_MATCH[i,0];
      Y += Me * CIE_COLOR_MATCH[i,1];
      Z += Me * CIE_COLOR_MATCH[i,2];
    }
    XYZ = (X + Y + Z);
    return new double[]{X / XYZ, Y / XYZ, Z / XYZ};
  }

    //TODO I think something in this color stack is off, somehow
  public static double[] spectrum_to_xyz(Dictionary<double,double> specIntens, double scale) {
//        Debug.Log("spectrum_to_xyz"+specIntens);
    double lambda, X = 0, Y = 0, Z = 0, XYZ;
    foreach(KeyValuePair<double, double> entry in specIntens)
    {
//        Debug.Log("spectrum_to_xyz 1 "+entry.Key+" "+entry.Value);
        // do something with entry.Value or entry.Key
      int i = (int) (entry.Key-380)/5;
      double Me = entry.Value;
      if (i < 0 || i >= CIE_COLOR_MATCH.GetLength(0)) {
          continue;
      }
      X += Me * CIE_COLOR_MATCH[i,0];
      Y += Me * CIE_COLOR_MATCH[i,1];
      Z += Me * CIE_COLOR_MATCH[i,2];
//        Debug.Log("spectrum_to_xyz 2 "+i+" "+X+" "+Y+" "+Z);
    }
    //XYZ = (X + Y + Z);
    if (Double.IsNaN(scale)) {
        scale = (X + Y + Z);
    }
    return new double[]{X * scale, Y * scale, Z * scale};
  }

    public static int bound(int value, int min, int max) {
        if (value <= min) return min;
        if (value >= max) return max;
        return value;
    }

    public static float bound(float value, float min, float max) {
        if (value <= min) return min;
        if (value >= max) return max;
        return value;
    }
    
    public static double bound(double value, double min, double max) {
        if (value <= min) return min;
        if (value >= max) return max;
        return value;
    }
    public static double[] CIEXYZtoRGB(double X, double Y, double Z, bool bound) {
//        Debug.Log("CIEXYZtoRGB"+X+" "+Y+" "+Z+" "+bound);
      double Rl = (3.2406*X) + (-1.5372*Y) + (-0.4986*Z);
      double Gl = (-0.9689*X) + (1.8758*Y) + (0.0415*Z);
      double Bl = (0.0557*X) + (-0.2040*Y) + (1.0570*Z);
      double a = 0.055;
      double R = ((Rl <= 0.0031308) ? (12.92 * Rl) : ((1 + a) * Math.Pow(Rl, 1/2.4)) - a);
      double G = ((Gl <= 0.0031308) ? (12.92 * Gl) : ((1 + a) * Math.Pow(Gl, 1/2.4)) - a);
      double B = ((Bl <= 0.0031308) ? (12.92 * Bl) : ((1 + a) * Math.Pow(Bl, 1/2.4)) - a);
      if (bound) {
        R = StupidColors.bound(R, 0.0, 1.0);
        G = StupidColors.bound(G, 0.0, 1.0);
        B = StupidColors.bound(B, 0.0, 1.0);
      }
      return new double[]{R, G, B};
    }

    public static double[] CIEXYZtoRGB(double[] xyz, bool bound) {
        return CIEXYZtoRGB(xyz[0],xyz[1],xyz[2],bound);
    }

    public static Color RGBtoColor(double[] rgb) {
//        Debug.Log("RGBtoColor"+rgb);
        return new Color((float)rgb[0],(float)rgb[1],(float)rgb[2],1f);
    }
}
