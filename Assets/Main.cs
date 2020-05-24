using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    //public ObjectPool pool;

    //private List<GameObject> objs = new List<GameObject>();
    private Random rand = new Random();
    private Dial[] dials = new Dial[0];

    private int targetFPS = 1;

    void Start()
    {
        Init(3);
    }
 
    private List<Dot> allDots = new List<Dot>();
    private Dictionary<Dot, List<Dial>> dotOwners = new Dictionary<Dot, List<Dial>>();

    void Init(int dialCount) {
        // Initial dial
        dials = new Dial[dialCount];
        {
            Dial dial = new Dial(Random.Range(6,16),Random.Range(380.0f+100f,780.0f-100f));
            dials[0] = dial;
            for (int i = 0; i < dials[0].dotCount; i++) {
                Dot dot = new Dot();
                dial.dots[i] = dot;
                dot.wavelengths.Add(dial.wavelength);
                allDots.Add(dot);
                dotOwners[dot] = dotOwners[dot] ?? new List<Dial>();
                dotOwners[dot].Add(dial);
            }
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
                dotOwners[d] = dotOwners[d] ?? new List<Dial>();
                if (dotOwners[d].Contains(dial)) {
                    // FREAK OUT
                    Debug.LogError("FREAK OUT ; dot is owned twice by same dial - probably wrapped all the way around");
                    System.Environment.Exit(0);
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
    }
    
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
    }
      
    // Update is called once per frame
    void Update()
    {
        if(Application.targetFrameRate != targetFPS)
            Application.targetFrameRate = targetFPS;

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

        foreach (Dial dial in dials) {
            for (int i = 0; i < dial.dotCount; i++) {
                float a = (i*Mathf.PI*2)/dial.dotCount;
                //float radius = dial.getRadius();
                float radius = dial.dotCount * 2f / 10;
                //Color color = new Color(dial.wavelength,1-dial.wavelength,Mathf.Sin(dial.wavelength*Mathf.PI*2),1);
                Color color = StupidColors.RGBtoColor(StupidColors.CIEXYZtoRGB(StupidColors.spectrum_to_xyz(new Dictionary<double,double>{
                    { dial.wavelength, 1.0 }
                }, 0.8),true));
                drawCircle(dial.pos + new Vector3(Mathf.Sin(a),Mathf.Cos(a),0)*radius, 0.1f, true, color);
            }
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
