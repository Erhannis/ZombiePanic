using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public ObjectPool pool;

    //private List<GameObject> objs = new List<GameObject>();
    private System.Random rand = new System.Random();

    private const int TARGET_FPS = 10;

    private const int PX_PER_UNIT = 100;
    private const double BOARD_WIDTH = 22.0; //TODO Calc from screen?

    private double[] elevations; // World units

    private Tank[] tanks;

    void Start()
    {
        Init();
    }
 
    private List<Color> predefTankColors = new List<Color> {Color.blue, Color.red, Color.green, Color.magenta};

    void Init() {
        elevations = new double[(int)(BOARD_WIDTH * PX_PER_UNIT)];
        
        // Gen terrain
        double min = -0.05;
        double max = 0.05;
        elevations[0] = 0;
        for (int i = 1; i < elevations.Length; i++) {
            elevations[i] = elevations[i-1] + (rand.NextDouble() * (max - min) + min);
        }

        tanks = new Tank[2];
        for (int i = 0; i < tanks.Length; i++) {
            Tank tank = new Tank();
            tank.x = Random.Range(0,elevations.Length);
            tank.color = predefTankColors[i];
            tanks[i] = tank;
        }

        //TODO
    }
    
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }
      
    private static Color DEF_COLOR = new Color(49/255f,77/255f,121/255f);
    private static Color WIN_COLOR = new Color(40/255f,90/255f,40/255f);

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;

        // if (Input.GetKeyDown(""+(i+1))) {
        //     turn(dials[i], !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
        //     checkWin();
        // }
        // if (Input.GetKeyDown("s")) {
        //     scramble();
        //     checkWin();
        // }

        //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.Log(ray);
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

        GL.Begin(GL.LINE_STRIP);
        GL.Color(new Color(1,1,1));
        for (int i = 0; i < elevations.Length; i++) {
            double x = ((double)(i - (elevations.Length/2.0))) / PX_PER_UNIT;
            double y = elevations[i];
            GL.Vertex3((float)x, (float)-10, 1);
            GL.Vertex3((float)x, (float)y, 1);
        }
        GL.End();

        foreach (Tank tank in tanks) {
            double x = ((double)(tank.x - (elevations.Length/2.0))) / PX_PER_UNIT;
            double y = elevations[tank.x];
            drawCircle(new Vector3((float)x, (float)y, 0), 0.1f, true, tank.color);
        }

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
