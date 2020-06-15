using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Entities;
using Jibu;

public class Main : MonoBehaviour
{
    private enum ActionMode {
        MOVE,
        DIG,
        AUTODIG,
        PLACE //TODO Differentiate between block types
    }

    private System.Random rand = new System.Random();

    private const int TARGET_FPS = 1;

    private const int PX_PER_UNIT = 100;
    private const float BOARD_WIDTH = 22.0f; //TODO Calc from screen?
    private const float TIMESCALE = 2f; //TODO Time

    private World world;
    private Rect playBounds; //TODO ????

    private Broodmother player;

    private bool uiDigBtnDown = false;
    private bool uiPlaceBtnDown = false;
    private bool uiUpBtnDown = false;
    private bool uiDownBtnDown = false;
    private bool pendingUp = false;
    private bool pendingDown = false;

    private ActionMode actionMode;

    void Start()
    {
        // int playerCount = (int)(float)SceneChanger.globals["playercount_float"];
        // bool rapidfire = (bool)SceneChanger.globals["rapidfire_bool"];
        Init();
    }
 
    private List<Color> predefTankColors = new List<Color> {Color.blue, Color.red, Color.green, Color.magenta, Color.yellow, Color.cyan};

    private List<(ChannelReader<int>, ChannelWriter<int>)> syncs = new List<(ChannelReader<int>, ChannelWriter<int>)>();

    void Init() {
        world = new World();
        player = new Broodmother(null);
        actionMode = ActionMode.MOVE;
        world.getTile(new Pos3(0, 0, 0)).addItem(player);

        ChannelReader<int> syncA;
        ChannelWriter<int> syncB;
        Channel<int> syncA0 = new Channel<int>();
        Channel<int> syncB0 = new Channel<int>();
        syncs.Add((syncA0.ChannelReader, syncB0.ChannelWriter));
        CreatureRunner cr = new CreatureRunner(player, syncA0.ChannelWriter, syncB0.ChannelReader,
@"for (let i = 0; i < 10; i++) {
    move(Pos3(1,0,0));
}
for (let i = 0; i < 10; i++) {
    move(Pos3(0,1,0));
}
for (let i = 0; i < 10; i++) {
    move(Pos3(-1,0,0));
}
for (let i = 0; i < 10; i++) {
    move(Pos3(0,-1,0));
}
while (true) {
    move(Pos3(0,0,0));
}"
        );
        cr.Start();

        for (int x = -10; x <= 10; x++) {
            for (int y = -10; y <= 10; y++) {
                for (int z = -10; z <= 10; z++) {
                    Pos3 pos = new Pos3(x,y,z);
                    if (((pos.x*pos.x)+(pos.y*pos.y)+(pos.z*pos.z)) == 16) {
                        world.getTile(pos).addItem(new Drone(null));
                    }
                }
            }
        }         

        // playBounds = new Rect(i2x(0),-50f,BOARD_WIDTH,200f); // Extra high, for high shots
    }
    
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }

    public Text text_u;
    private long count_u = 0;
    private double ms_u = 0;
    private System.Diagnostics.Stopwatch sw_u = new System.Diagnostics.Stopwatch();
    // Update is called once per frame
    void Update()
    {
        sw_u.Restart();
        sw_u.Start();

        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;
        //Camera.main.GetComponent<Camera>().orthographicSize = (0.5f * playBounds.width * Screen.height) / Screen.width;

        Camera.main.backgroundColor = new Color(0,0,0); //TODO Move elsewhere?

        foreach (var (a, b) in syncs) {
            Debug.Log("main sync 0");
            a.Read();
            Debug.Log("main sync 1");
            b.Write(0);
            Debug.Log("main sync 2");
        }

        //Debug.Log("//TODO Remove reset key");
        if (Input.GetKeyDown("r")) {
            Init();
            return;
        }
        // if (Input.GetKeyDown("q")) {
        //     SceneChanger.staticLoadScene("MainMenu");
        //     return;
        // }


        // if (Input.GetKeyDown(""+(i+1))) {
        //     turn(dials[i], !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)));
        //     checkWin();
        // }
        // if (Input.GetKeyDown("s")) {
        //     scramble();
        //     checkWin();
        // }

        // Update shells

        doPlayerInput();

        sw_u.Stop();
        TimeSpan ts = sw_u.Elapsed;
        ms_u += ts.TotalMilliseconds;
        count_u++;
        if (count_u >= 10) {
            text_u.text = "" + (ms_u/count_u);
            count_u = 0;
            ms_u = 0;
        }
    }

    private MPos3 checkInputDir(Vector3 cursorPos) {
        MPos3 dir = new MPos3(0,0,0);
        var ray = Camera.main.ScreenPointToRay(cursorPos);
        Vector2 tap = new Vector2(ray.origin.x, ray.origin.y).normalized;
        RaycastHit hit;
        Physics.Raycast(ray.origin, Vector3.forward, out hit);
        if (hit.transform == null) {
            // Didn't hit any of the UI button colliders

            List<Vector2> dirs = new List<Vector2>{
                new Vector2(0,1).normalized,
                new Vector2(1,1).normalized,
                new Vector2(1,0).normalized,                
                new Vector2(1,-1).normalized,                
                new Vector2(0,-1).normalized,                
                new Vector2(-1,-1).normalized,
                new Vector2(-1,0).normalized,
                new Vector2(-1,1).normalized
            };
            int mi = 0;
            float md = float.PositiveInfinity;
            for (int i = 0; i < 8; i++) {
                Vector2 v = tap-dirs[i];
                if (v.magnitude < md) {
                    mi = i;
                    md = v.magnitude;
                }
            }
            switch (mi) {
                case 0:
                    dir.y++;
                    break;
                case 1:
                    dir.x++;
                    dir.y++;
                    break;
                case 2:
                    dir.x++;
                    break;
                case 3:
                    dir.x++;
                    dir.y--;
                    break;
                case 4:
                    dir.y--;
                    break;
                case 5:
                    dir.x--;
                    dir.y--;
                    break;
                case 6:
                    dir.x--;
                    break;
                case 7:
                    dir.x--;
                    dir.y++;
                    break;
            }
            return dir;
        } else {
            return null;
        }
    }

    private void doPlayerInput() {
        if (Input.GetKeyDown("d")) {
            actionMode = ActionMode.DIG;
        }
        if (Input.GetKeyDown("a")) {
            actionMode = ActionMode.AUTODIG;
        }
        if (Input.GetKeyDown("m")) {
            actionMode = ActionMode.MOVE;
        }
        if (Input.GetKeyDown("p")) {
            actionMode = ActionMode.PLACE;
        }

        MPos3 dir = new MPos3(0,0,0);
        bool foundMov = false;

        if (!foundMov) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    MPos3 idir = checkInputDir(touch.position);
                    if (idir != null) {
                        dir += idir;
                        foundMov = true;
                    }
                    break;
                }
            }
        }

        if (!foundMov && Input.GetMouseButtonDown(0) && !(uiUpBtnDown || uiDownBtnDown)) { // Left click (0-left,1-right,2-middle)
            MPos3 idir = checkInputDir(Input.mousePosition);
            if (idir != null) {
                dir += idir;
                foundMov = true;
            }
        }

        if (dir.Equals(new MPos3(0,0,0))) {
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                dir.x++;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                dir.x--;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                dir.y++;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                dir.y--;
            }
            if (Input.GetKeyDown(KeyCode.Space) || pendingUp) {
                dir.z++;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || pendingDown) {
                dir.z--;
            }
            if (!dir.Equals(new MPos3(0,0,0))) {
                foundMov = true;
            }
        }

        pendingUp = false;
        pendingDown = false;

        //TODO Should probably organize this better
        if (foundMov) {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || uiPlaceBtnDown) {
                if (tryPlayerAction(dir, ActionMode.PLACE)) {
                    // Placed
                } else {
                    // Failed to place
                }
            } else if (uiDigBtnDown) {
                if (tryPlayerAction(dir, ActionMode.DIG)) {
                    // Dug
                } else {
                    // Couldn't dig; wasn't able to move. ...???
                }
            } else {
                if (tryPlayerAction(dir, actionMode)) {
                    // Did the thing
                } else {
                    if (actionMode == ActionMode.AUTODIG) {
                        // AUTODIG failed to move in a direction
                        if (tryPlayerAction(dir, ActionMode.DIG)) {
                            // Dug
                        } else {
                            // Couldn't dig; wasn't able to move. ...???
                        }
                    }
                    // Play bump sound?
                }
            }
        }
    }

    private Pos3 getPlayerPos() {
        return (player.parent as Tile).pos; //TODO Might not always be Tile
    }

    private bool tryPlayerAction(MPos3 dir, ActionMode mode) {
        //TODO Test or something
        //TODO Should maybe have a world.moveEntity() or something
        MPos3 newMPos = getPlayerPos().toMPos3() + dir;
        Pos3 newPos = newMPos.toPos3();
        Tile newTile = world.getTile(newPos);

        switch (mode) {
            case ActionMode.AUTODIG: // AUTODIG defaults first to movement
            case ActionMode.MOVE:
                foreach (Entity e in newTile.getInventory()) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                world.getTile(getPlayerPos()).removeItem(player);
                newTile.addItem(player);
                return true;
            case ActionMode.DIG:
                Entity digged = null;
                foreach (Entity e in newTile.getInventory()) {
                    if (e.blocksMovement()) {
                        digged = e;
                        break;
                    }
                }
                if (digged != null) {
                    newTile.removeItem(digged);;
                    player.addItem(digged);
                    return true;
                } else {
                    return false;
                }
            case ActionMode.PLACE:
                foreach (Entity e in newTile.getInventory()) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                if (player.inventory.Count == 0) {
                    return false;
                }
                Entity placed = player.inventory[player.inventory.Count-1];
                player.removeItem(placed);
                newTile.addItem(placed);
                return true;
            default:
                return false;
        }
    }

    //// ++++ UI
    public void uiDigBtn(bool down) {
        uiDigBtnDown = down;
    }

    public void uiPlaceBtn(bool down) {
        uiPlaceBtnDown = down;
    }

    public void uiUpBtn(bool down) {
        if (uiUpBtnDown && !down) {
            pendingUp = true;
        }
        uiUpBtnDown = down;
    }

    public void uiDownBtn(bool down) {
        if (uiDownBtnDown && !down) {
            pendingDown = true;
        }
        uiDownBtnDown = down;
    }
    //// ---- UI

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

    public Text text_oro;
    private long count_oro = 0;
    private double ms_oro = 0;
    private System.Diagnostics.Stopwatch sw_oro = new System.Diagnostics.Stopwatch();

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        sw_oro.Restart();
        sw_oro.Start();

        var vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;    
        var horizExtent = vertExtent * Screen.width / Screen.height;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        Pos3 center = getPlayerPos();
        Pos3 visionRadius = new Pos3(((long)(horizExtent))+1,((long)(vertExtent))+1,1); //TODO //PARAM z vision
        world.render(center, center - visionRadius, center + visionRadius);

        GL.PopMatrix();

        sw_oro.Stop();
        TimeSpan ts = sw_oro.Elapsed;
        ms_oro += ts.TotalMilliseconds;
        count_oro++;
        if (count_oro >= 10) {
            text_oro.text = "" + (((float)ms_oro)/count_oro);
            ms_oro = 0;
            count_oro = 0;
        }
    }

    private int lineCount = 100;

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
