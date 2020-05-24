using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial
{
    public Vector3 pos = new Vector3(0,0,0);
    public int dotCount;
    public Dot[] dots;
    public double wavelength;
    public Dial(int dots, double wavelength) {
        this.dotCount = dots;
        this.dots = new Dot[dotCount];
        this.wavelength = wavelength;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Dictionary<Dot,Dot> getClockwise() {
        Dictionary<Dot,Dot> update = new Dictionary<Dot,Dot>();
        for (int i = 0; i < dotCount-1; i++) {
            update[dots[i]] = dots[i+1];
        }
        if (dotCount > 0) {
            update[dots[dotCount-1]] = dots[0];
        }
    }

    public Dictionary<Dot,Dot> getCounterclockwise() {
        Dictionary<Dot,Dot> update = new Dictionary<Dot,Dot>();
        for (int i = 1; i < dotCount; i++) {
            update[dots[i]] = dots[i-1];
        }
        if (dotCount > 0) {
            update[dots[0]] = dots[dotCount-1];
        }
    }

    public void updateDots(Dictionary<Dot,Dot> update) {
        for (int i = 0; i < dotCount; i++) {
            if (update.ContainsKey(dots[i])) {
                dots[i] = update[dots[i]];
            }
        }
    }
}
