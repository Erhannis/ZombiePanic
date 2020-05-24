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
 
    void Init(int dialCount) {
        //TODO Make sure can't loop forever
        dials = new Dial[dialCount];
        dials[0] = new Dial(Random.Range(6,16),Random.Range(380.0f+100f,780.0f-100f));
        dials[0].pos = new Vector3(Random.Range(-3f,3f),Random.Range(-3f,3f),0);
        int count = 1;
        int total = 0;
        outerLoop: while (count < dialCount) {
            total++;
            if (total > 1000) {
                Debug.Log("ERROR hit init cap");
                return;
            }
            dials[count] = new Dial(Random.Range(6,16),Random.Range(380.0f+100f,780.0f-100f));
            dials[count].pos = new Vector3(Random.Range(-3f,3f),Random.Range(-3f,3f),0);
            // Check validity
            bool touching = false;
            for (int i = 0; i < count; i++) {
                // Check concentric
                float dist = (dials[count].pos - dials[i].pos).magnitude;
                Debug.Log("dists "+dials[count].getRadius()+" "+dials[i].getRadius()+" "+dist);
                if (dist < dials[count].getRadius()+0.25f || dist < dials[i].getRadius()+0.25f) {
                    goto outerLoop;
                }
                // Check touching
                if (dist < dials[count].getRadius()+dials[i].getRadius()-0.1f) {
                    touching = true;
                }
            }
            if (!touching) {
                goto outerLoop;
            }
            count++;
            Debug.Log("success");
        }
        for (int i = 0; i < dialCount; i++) {
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
            for (int i = 0; i < dial.dots; i++) {
                float a = (i*Mathf.PI*2)/dial.dots;
                float radius = dial.getRadius();
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
