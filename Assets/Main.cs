using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    //public ObjectPool pool;

    //private List<GameObject> objs = new List<GameObject>();
    private Random rand = new Random();
    private Dial[] dials = new Dial[0];

    private int targetFPS = 10;

    void Start()
    {
        Init(2);
    }
 
    private List<Dot> allDots = new List<Dot>();
    private Dictionary<Dot, List<Dial>> dotOwners = new Dictionary<Dot, List<Dial>>();
    private Dictionary<Dot, List<Dial>> properDotOwners = new Dictionary<Dot, List<Dial>>();

    void Init(int dialCount) {
        // Initial dial
        dials = new Dial[dialCount];
        {
            //Dial dial = new Dial(Random.Range(6,16),Random.Range(380.0f+100f,780.0f-100f));
            Dial dial = new Dial(Random.Range(8,8),Random.Range(380.0f+100f,780.0f-100f));
            dials[0] = dial;
            for (int i = 0; i < dials[0].dotCount; i++) {
                Dot dot = new Dot();
                dial.dots[i] = dot;
                dot.wavelengths.Add(dial.wavelength);
                allDots.Add(dot);
                if (!dotOwners.ContainsKey(dot)) {dotOwners[dot] = new List<Dial>();}
                dotOwners[dot].Add(dial);
            }
            Debug.Log("success");
        }
        int count = 1;
        int total = 0;
        outerLoop: while (count < dialCount) {
            total++;
            if (total > 1000) {
                Debug.LogError("ERROR hit init cap");
                return;
            }
            // New dial
            Dial dial = new Dial(Random.Range(6,16),Random.Range(380.0f+100f,780.0f-100f));
            dials[count] = dial;
            Dot curDot = allDots[Random.Range(0,allDots.Count)];
            dial.dots[0] = curDot;
            Dial attachedDial;
            // Attach to neighbor dial
            if (dotOwners[curDot].Count > 1) { //TODO Yeah, I just don't wanna deal with that right now.  Maybe ever.
                foreach (KeyValuePair<Dot, List<Dial>> entry in dotOwners) {
                    entry.Value.RemoveAll(item => item == dial);
                }
                //TODO Any other cleanup
                goto outerLoop;
            } else {
                attachedDial = dotOwners[curDot][0];
            }
            int dotCount = 1;
            // Crawl along dots, maybe detach
            while (dotCount < dial.dotCount) {
                if (Random.Range(0,dial.dotCount) < 2) { //TODO Consider
                    attachedDial = null; // Detach
                }
                if (attachedDial != null) {
                    curDot = attachedDial.cwDot(curDot);
                    if (dotOwners[curDot].Count > 1) { // We've come to a junction
                        if (dotOwners[curDot].Count == 2) {
                            attachedDial = dotOwners[curDot][1-dotOwners[curDot].IndexOf(attachedDial)];
                        } else {
                            //TODO Yeah, I just don't wanna deal with that right now.  Maybe ever.
                            foreach (KeyValuePair<Dot, List<Dial>> entry in dotOwners) {
                                entry.Value.RemoveAll(item => item == dial);
                            }
                            //TODO Any other cleanup
                            goto outerLoop;
                        }
                    }
                } else {
                    curDot = new Dot();
                }
                dial.dots[dotCount] = curDot;
                dotCount++;
            }


            // Updating dots on successful dial init
            foreach (Dot d in dial.dots) {
                if (!dotOwners.ContainsKey(d)) {dotOwners[d] = new List<Dial>();}
                if (dotOwners[d].Contains(dial)) {
                    // FREAK OUT
                    Debug.LogError("FREAK OUT ; dot is owned twice by same dial - probably wrapped all the way around");
//                    System.Environment.Exit(0);
                    return;
                }
                if (!allDots.Contains(d)) {
                    allDots.Add(d);
                }
                dotOwners[d].Add(dial);
                d.wavelengths.Add(dial.wavelength);
            }


            count++;
            Debug.Log("success");
        }
        Debug.Log("init in " + count + " loops");
        Debug.Log("distributing...");
        foreach (Dot d in allDots) {
            d.pos = new Vector3(Random.Range(-3f,3f),Random.Range(-3f,3f),0);
        }
        for (int i = 0; i < 1000; i++) {
            // Apply forces
            foreach (Dot d in allDots) {
                float forceScale = 0.01f;
                Vector3 force = new Vector3(0,0,0);
                foreach (Dot d2 in allDots) { // Repulsive
                    if (d == d2) {
                        continue;
                    }
                    Vector3 diff = d.pos - d2.pos;

                    force += diff.normalized * (1 / diff.magnitude);
                }
                HashSet<Dot> neighbors = new HashSet<Dot>();
                foreach (Dial dial in dials) {
                    Dot d0;
                    d0 = dial.cwDot(d);
                    if (d0 != null) {
                        neighbors.Add(d0);
                    }
                    d0 = dial.ccwDot(d);
                    if (d0 != null) {
                        neighbors.Add(d0);
                    }
                }
                foreach (Dot d2 in neighbors) { // Attractive
                    if (d == d2) {
                        continue;
                    }
                    Vector3 diff = d.pos - d2.pos;

                    force += 1 * (diff.normalized * Mathf.Pow((1 - diff.magnitude),3));
                }

                // Apply force
                d.pos += force*forceScale;
            }

            // Normalize
            Vector3 min = new Vector3(0,0,0);
            Vector3 max = new Vector3(0,0,0);
            foreach (Dot d in allDots) {
                min.x = Mathf.Min(min.x, d.pos.x);
                min.y = Mathf.Min(min.y, d.pos.y);
                max.x = Mathf.Max(max.x, d.pos.x);
                max.y = Mathf.Max(max.y, d.pos.y);
            }
            float size = Mathf.Max(max.x-min.x,max.y-min.y);
            Vector3 center = (max+min)/2;
            float scale = 6f / size;
            foreach (Dot d in allDots) { // Apply normalization
                d.pos = (d.pos - center) * scale;
            }
        }
        Debug.Log("Result");
        foreach (Dot d in allDots) {
            Debug.Log("dot " + d.pos);
        }
        Debug.Log("Done distributing");
        
        foreach (Dot dot in allDots) {
            properDotOwners[dot] = new List<Dial>(dotOwners[dot]);
        }

        Debug.Log("Scrambling...");
        for (int i = 0; i < 0; i++) {
            Dial turnDial = dials[Random.Range(0,dials.Length)];
            if (Random.Range(0,2) == 1) {
                turn(turnDial, true);
            } else {
                turn(turnDial, false);
            }
        }
        Debug.Log("Done scrambling");
        Debug.Log("Done init");
    }
    
    private void turn(Dial turnDial, bool cw) {
        Dictionary<Dot,Dot> update;
        if (cw) {
            update = turnDial.getClockwise();
        } else {
            update = turnDial.getCounterclockwise();
        }
        foreach (Dial dial in dials) {
            dial.updateDots(update);
        }
        dotOwners.Clear();
        foreach (Dial dial in dials) {
            foreach (Dot dot in dial.dots) {
                if (!dotOwners.ContainsKey(dot)) {dotOwners[dot] = new List<Dial>();}
                dotOwners[dot].Add(dial);
            }
        }
        Dictionary<Dot,Vector3> newPositions = new Dictionary<Dot, Vector3>();
        foreach (KeyValuePair<Dot,Dot> entry in update) {
            newPositions[entry.Value] = entry.Key.pos;
        }
        foreach (KeyValuePair<Dot,Vector3> entry in newPositions) {
            entry.Key.pos = entry.Value;
        }
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
      
    private void checkWin() {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != targetFPS)
            Application.targetFrameRate = targetFPS;

        for (int i = 0; i < dials.Length; i++) {
            if (Input.GetKeyDown(""+(i+1))) {
                turn(dials[i], !Input.GetKey(KeyCode.LeftShift));
                checkWin();
            }
        }

        // foreach (GameObject o in objs) {
        //     pool.PoolObject(o);
        // }
        // objs.Clear();
        
        // for (int i = 0; i < 10; i++) {
        //     GameObject obj = pool.GetObjectForType("Circle", false);
        //     obj.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1,Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));
        //     obj.transform.position = new Vector3(Random.Range(-4f,4f), Random.Range(-4f,4f), 1);
        //     objs.Add(obj);
        // }        



            // Apply forces
            foreach (Dot d in allDots) {
                float forceScale = 0.01f;
                Vector3 force = new Vector3(0,0,0);
                foreach (Dot d2 in allDots) { // Repulsive
                    if (d == d2) {
                        continue;
                    }
                    Vector3 diff = d.pos - d2.pos;

                    force += diff.normalized * (1 / diff.magnitude);
                }
                HashSet<Dot> neighbors = new HashSet<Dot>();
                foreach (Dial dial in dials) {
                    Dot d0;
                    d0 = dial.cwDot(d);
                    if (d0 != null) {
                        neighbors.Add(d0);
                    }
                    d0 = dial.ccwDot(d);
                    if (d0 != null) {
                        neighbors.Add(d0);
                    }
                }
                foreach (Dot d2 in neighbors) { // Attractive
                    if (d == d2) {
                        continue;
                    }
                    Vector3 diff = d.pos - d2.pos;

                    force += 1 * (diff.normalized * Mathf.Pow((1 - diff.magnitude),3));
                }

                // Apply force
                d.pos += force*forceScale;
            }

            // Normalize
            Vector3 min = new Vector3(0,0,0);
            Vector3 max = new Vector3(0,0,0);
            foreach (Dot d in allDots) {
                min.x = Mathf.Min(min.x, d.pos.x);
                min.y = Mathf.Min(min.y, d.pos.y);
                max.x = Mathf.Max(max.x, d.pos.x);
                max.y = Mathf.Max(max.y, d.pos.y);
            }
            float size = Mathf.Max(max.x-min.x,max.y-min.y);
            Vector3 center = (max+min)/2;
            float scale = 6f / size;
            foreach (Dot d in allDots) { // Apply normalization
                d.pos = (d.pos - center) * scale;
            }
    }

    // When added to an object, draws colored rays from the
    // transform position.
    private int lineCount = 100;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // foreach (Dial dial in dials) {
        //     for (int i = 0; i < dial.dotCount; i++) {
        //         float a = (i*Mathf.PI*2)/dial.dotCount;
        //         //float radius = dial.getRadius();
        //         float radius = dial.dotCount * 2f / 10;
        //         //Color color = new Color(dial.wavelength,1-dial.wavelength,Mathf.Sin(dial.wavelength*Mathf.PI*2),1);
        //         Color color = StupidColors.RGBtoColor(StupidColors.CIEXYZtoRGB(StupidColors.spectrum_to_xyz(new Dictionary<double,double>{
        //             { dial.wavelength, 1.0 }
        //         }, 0.8),true));
        //         drawCircle(dial.pos + new Vector3(Mathf.Sin(a),Mathf.Cos(a),0)*radius, 0.1f, true, color);
        //     }
        // }

        foreach (Dot dot in allDots) {
            //Color color = new Color(dial.wavelength,1-dial.wavelength,Mathf.Sin(dial.wavelength*Mathf.PI*2),1);
            Dictionary<double,double> wls = new Dictionary<double,double>();
            foreach (double wl in dot.wavelengths) {
                wls[wl] = 1f;
            }
            Color color = StupidColors.RGBtoColor(StupidColors.CIEXYZtoRGB(StupidColors.spectrum_to_xyz(wls, 0.8),true));
            //Debug.Log("dot pos " + dot.pos);
            drawCircle(dot.pos, 0.1f, true, color);
        }

        foreach (Dial dial in dials) {
            GL.Begin(GL.LINE_STRIP);
            Color color = StupidColors.RGBtoColor(StupidColors.CIEXYZtoRGB(StupidColors.spectrum_to_xyz(new Dictionary<double,double>{
                { dial.wavelength, 1.0 }
            }, 0.8),true));
            GL.Color(color);
            for (int i = 0; i <= dial.dotCount; i++) {
                Vector3 pos = dial.dots[Utils.mod(i,dial.dotCount)].pos;
                GL.Vertex3(pos.x, pos.y, pos.z);
            }
            GL.End();
        }

// Color debugging
            // GL.Begin(GL.TRIANGLE_STRIP);
            // for (int i = 0; i <= lineCount; ++i)
            // {
            //     float r = 2f;
            //     float a = i / (float)lineCount;
            //     Color color = StupidColors.RGBtoColor(StupidColors.CIEXYZtoRGB(StupidColors.spectrum_to_xyz(new Dictionary<double,double>{
            //         { ((780.0-380.0)*a)+380.0, 1.0 }
            //     }, 0.8),true));
            //     GL.Color(color);
            //     float angle = a * Mathf.PI * 2;
            //     GL.Vertex3(0, 0, 0);
            //     GL.Vertex3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
            // }
            // GL.End();

        GL.PopMatrix();
    }

    private void drawCircle(Vector3 pos, float r, bool filled, Color color) {
        if (filled) {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i)
            {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(pos.x, pos.y, pos.z);
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        } else {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i)
            {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        }

    }}
