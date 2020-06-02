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
    private const float BOARD_WIDTH = 22.0f; //TODO Calc from screen?
    private const float TANK_RADIUS = 0.1f;
    private const float HEAD_HEIGHT = 0.1f;
    private static Vector2 G = new Vector2(0, -9.8f)*0.1f;

    private Rect playBounds;
    private float[] elevations; // World units

    private Tank[] tanks;
    private int currentPlayer;

    void Start()
    {
        Init(3);
    }
 
    private List<Color> predefTankColors = new List<Color> {Color.blue, Color.red, Color.green, Color.magenta, Color.yellow, Color.cyan};

    void Init(int players) {
        elevations = new float[(int)(BOARD_WIDTH * PX_PER_UNIT)];
        
        // Gen terrain
        float min = -0.05f;
        float max = 0.05f;
        elevations[0] = 0;
        for (int i = 1; i < elevations.Length; i++) {
            elevations[i] = elevations[i-1] + (Random.Range(min, max));
        }

        playBounds = new Rect(i2x(0),-10f,elevations.Length*PX_PER_UNIT,200f); // Extra high, for high shots

        tanks = new Tank[players];
        for (int i = 0; i < tanks.Length; i++) {
            Tank tank = new Tank();
            tank.x = Random.Range(0,elevations.Length);
            tank.color = predefTankColors[i];
            tanks[i] = tank;
        }

        currentPlayer = tanks.Length-1;
        advancePlayer();
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

        if (Input.GetMouseButtonUp(0)) { // Left click (0-left,1-right,2-middle)
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int oi = tanks[currentPlayer].x;
            float ox = i2x(oi);
            float oy = elevations[oi]+HEAD_HEIGHT;

            Vector2 origin = new Vector2(ox, oy);
            Vector2 tap = new Vector2(ray.origin.x, ray.origin.y);

            Vector2 p = new Vector2(origin.x, origin.y);
            Vector2 v = tap-origin;
            float speed = Mathf.Max(v.magnitude,1f);
            float dt = 1/(speed * PX_PER_UNIT);
            while (true) {
                p += v*dt;
                v += G*dt;

                if (!playBounds.Contains(p)) {
                    break;
                }

                int i = x2i(p.x);
                if (0 <= i && i < elevations.Length && p.y <= elevations[i]) {
                    // Hit!
                    explode(p,0.5f); //PARAM Explosion radius
                    break;
                }
            }
            
            //explode(ray.origin,0.5f);
            advancePlayer();
        }
    }

    private void advancePlayer() {
        int started = currentPlayer;
        do {
            currentPlayer = (currentPlayer + 1) % tanks.Length;
            if (tanks[currentPlayer].alive) {
                break;
            }
        } while (currentPlayer != started);
        checkWin();
    }

    private void checkWin() {
        Tank living = null;
        int aliveCount = 0;
        foreach (Tank tank in tanks) {
            if (tank.alive) {
                aliveCount++;
                living = tank;
            }
        }
        if (aliveCount == 0) {
            // Mutual destruction
            Camera.main.backgroundColor = new Color(0,0,0);
        } else if (aliveCount == 1) {
            // Won
            Camera.main.backgroundColor = living.color;
        } else {
            // Neither - use current player's color
            Camera.main.backgroundColor = (tanks[currentPlayer].color + 2*Color.white)/4;
        }
    }

    private void hitTank(Tank tank, float r) {
        //TODO Could flash screen, something
        tank.alive = false;
        checkWin();
    }

    private void explode(Vector2 pos, float r) {
        foreach (Tank tank in tanks) {
            float x = i2x(tank.x);
            float y = elevations[tank.x];

            float edge_y = Mathf.Sqrt(Utils.sqr(r)-Utils.sqr(pos.x-x));
            if (float.IsNaN(edge_y) || float.IsInfinity(edge_y)) { // Left/right of explosion
                continue;
            }

            float reduction;
            if (y <= pos.y - edge_y) { // Under explosion
            } else if (y >= pos.y + edge_y) { // Above explosion
            } else { // Inside explosion
                hitTank(tank, r);
            }
        }

        int left = Mathf.Clamp(x2i(pos.x-r)-1,0,elevations.Length);
        int right = Mathf.Clamp(x2i(pos.x+r)+1,-1,elevations.Length-1);
        if (left == elevations.Length || right == -1) {
            // Offscreen
        } else {
            for (int i = left; i <= right; i++) {
                float x = i2x(i);
                float y = elevations[i];

                float edge_y = Mathf.Sqrt(Utils.sqr(r)-Utils.sqr(pos.x-x));
                if (float.IsNaN(edge_y) || float.IsInfinity(edge_y)) {
                    continue;
                }

                float reduction;
                if (y <= pos.y - edge_y) { // Untouched
                    reduction = 0f;
                } else if (y >= pos.y + edge_y) { // Full brunt
                    reduction = 2*edge_y;
                } else { // Partial
                    reduction = y - (pos.y - edge_y);
                }

                elevations[i] -= reduction;
            }
        }
    }

    private float i2x(int i) {
        return ((float)(i - (elevations.Length/2.0))) / PX_PER_UNIT;
    }

    private int x2i(float x) {
        return (int)((x * PX_PER_UNIT) + (elevations.Length/2.0));
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

        // Terrain
        GL.Begin(GL.LINE_STRIP);
        GL.Color(new Color(1,1,1));
        for (int i = 0; i < elevations.Length; i++) {
            float x = i2x(i);
            float y = elevations[i];
            GL.Vertex3(x, -10f, 1);
            GL.Vertex3(x, y, 1);
        }
        GL.End();

        foreach (Tank tank in tanks) {
            float x = i2x(tank.x);
            float y = elevations[tank.x];
            drawCircle(new Vector3(x, y, 0), TANK_RADIUS, tank.alive, tank.color);
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
