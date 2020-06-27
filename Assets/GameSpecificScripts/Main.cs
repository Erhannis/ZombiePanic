using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Entities;
using Jibu;

/*
Halfgame ideas:
    Boolean victory
        Collect X rocks
            In under TIME
        Capture opposing team
        Seal yourselves in
        Find the X in TIME

    Continuous victory
        Most rocks in TIME
        Farthest spread in TIME
        Find the X; TIME trial

*/

public class Main : MonoBehaviour {
    private enum ActionMode {
        MOVE
    }

    private System.Random rand = new System.Random();

    private int TARGET_FPS = 60;

    private const int PX_PER_UNIT = 100;
    private const float BOARD_WIDTH = 22.0f; //TODO Calc from screen?
    private const float TIMESCALE = 2f; //TODO Time

    private Color BG_COLOR = Color.black;

    private World world;
    private Rect playBounds; //TODO ????

    private Human player;

    private bool uiDigBtnDown = false;
    private bool uiPlaceBtnDown = false;
    private bool uiUpBtnDown = false;
    private bool uiDownBtnDown = false;
    private bool pendingUp = false;
    private bool pendingDown = false;

    private ActionMode actionMode;
    private int stepsCount = 0;
    private int zombieAttention;
    private string highscoreKey;

    void Start() {
        float rockDensity = (float)SceneChanger.globals["rock_density_float"];
        float zombieDensity = (float)SceneChanger.globals["zombie_density_float"];
        int zombieAttention = (int)SceneChanger.globals["zombie_attention_int"];
        Init(rockDensity, zombieDensity, zombieAttention);
    }

    void Init(float rockDensity, float zombieDensity, int zombieAttention) {
        if (world != null) {
            // Try to kill any old threads
            foreach (var (_, a, b) in world.runners) {
                a.Poison();
                b.Poison();
            }
        }

        this.highscoreKey = "highscore_" + rockDensity + "_" + zombieDensity + "_" + zombieAttention;
        Debug.Log(highscoreKey);
        int highscore = PlayerPrefs.GetInt(highscoreKey, 0);
        text_oro.text = "" + highscore;
        this.TARGET_FPS = 60;
        this.stepsCount = 0;
        text_u.text = "" + stepsCount;
        this.zombieAttention = zombieAttention;
        world = new World(rockDensity, zombieDensity);
        player = new Human(null);
        world.player = player;
        actionMode = ActionMode.MOVE;
        Tile origin = world.getTile(new Pos3(0, 0, 0));
        foreach (Entity block in origin.getInventory().Where(e => e.blocksMovement()).ToList()) {
            origin.removeItem(block);
        }
        origin.addItem(player);

        // playBounds = new Rect(i2x(0),-50f,BOARD_WIDTH,200f); // Extra high, for high shots
    }

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }

    public Text text_u;
    private long count_u = 0;
    private double ms_u = 0;
    private System.Diagnostics.Stopwatch sw_u = new System.Diagnostics.Stopwatch();
    // Update is called once per frame
    void Update() {
        sw_u.Restart();
        sw_u.Start();

        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;
        //Camera.main.GetComponent<Camera>().orthographicSize = (0.5f * playBounds.width * Screen.height) / Screen.width;

        Camera.main.backgroundColor = BG_COLOR; //TODO Move elsewhere?

        //Debug.Log("//TODO Remove reset key");
        if (Input.GetKeyDown("r")) {
            float rockDensity = (float)SceneChanger.globals["rock_density_float"];
            float zombieDensity = (float)SceneChanger.globals["zombie_density_float"];
            int zombieAttention = (int)SceneChanger.globals["zombie_attention_int"];
            Init(rockDensity, zombieDensity, zombieAttention);
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

        if (player.alive && doPlayerInput()) {
            // Check zombie 
            var ppos = getPlayerPos();
            for (long y = ppos.y - zombieAttention; y <= ppos.y + zombieAttention; y++) {
                for (long x = ppos.x - zombieAttention; x <= ppos.x + zombieAttention; x++) {
                    Tile t = world.getTile(new Pos3(x, y, 0));
                    List<Entity> zombies = t.getInventory().Where(e => e is Zombie).ToList();
                    foreach (Zombie z in zombies) {
                        if (!world.wakeZombies.ContainsKey(z)) {
                            world.addZombie(z,
@"while (true) {
  let hdir = getHumanDir();
  let dist = hdir.normLInf();
  if (dist > " + zombieAttention + @") {
    log('dying; dist ' + dist);
    return;
  }
  move(hdir.normalizeLInf());
}");
                        }
                    }
                }
            }
            world.stepRunners();
        } else if (doPlayerInput()) {
            float rockDensity = (float)SceneChanger.globals["rock_density_float"];
            float zombieDensity = (float)SceneChanger.globals["zombie_density_float"];
            int zombieAttention = (int)SceneChanger.globals["zombie_attention_int"];
            Init(rockDensity, zombieDensity, zombieAttention);
            return;
        }

        checkWin();

        sw_u.Stop();
        //TimeSpan ts = sw_u.Elapsed;
        //ms_u += ts.TotalMilliseconds;
        //count_u++;
        //if (count_u >= 10) {
        //    text_u.text = "" + (ms_u / count_u);
        //    count_u = 0;
        //    ms_u = 0;
        //}
    }

    private void checkWin() {
        if (player.alive) {
            Camera.main.backgroundColor = BG_COLOR;
        } else {
            Camera.main.backgroundColor = Color.red;
        }
    }

    private MPos3 checkInputDir(Vector3 cursorPos) {
        MPos3 dir = new MPos3(0, 0, 0);
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
                Vector2 v = tap - dirs[i];
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

    private bool doPlayerInput() {
        if (Input.GetKeyDown("m")) {
            actionMode = ActionMode.MOVE;
        }

        MPos3 dir = new MPos3(0, 0, 0);
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

        if (dir.Equals(new MPos3(0, 0, 0))) {
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
            if (!dir.Equals(new MPos3(0, 0, 0))) {
                foundMov = true;
            }
        }

        pendingUp = false;
        pendingDown = false;

        if (foundMov) {
            if (tryPlayerAction(dir, actionMode)) {
                stepsCount++; //TODO Should count waits?
                text_u.text = "" + stepsCount;
                int highscore = PlayerPrefs.GetInt(highscoreKey, 0);
                if (stepsCount > highscore) {
                    highscore = stepsCount;
                    PlayerPrefs.SetInt(highscoreKey, stepsCount);
                }
                text_oro.text = "" + highscore;
            }
        }
        return foundMov;
    }

    private Pos3 getPlayerPos() {
        Tile tile = player.parent as Tile;
        if (tile != null) {
            return tile.pos;
        }
        Inventoried p = player.parent;
        while (true) {
            if (p is Tile) {
                return (p as Tile).pos;
            }
            if (p == null) {
                return null; //TODO ???
            }
            if (p is Entity) {
                p = (p as Entity).parent;
            } else {
                return null; //TODO ???
            }
        }
    }

    private bool tryPlayerAction(MPos3 dir, ActionMode mode) {
        //TODO Test or something
        //TODO Should maybe have a world.moveEntity() or something
        MPos3 newMPos = getPlayerPos().toMPos3() + dir;
        Pos3 newPos = newMPos.toPos3();
        Tile newTile = world.getTile(newPos);

        switch (mode) {
            case ActionMode.MOVE:
                foreach (Entity e in newTile.getInventory()) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                return Inventories.move(player, world.getTile(getPlayerPos()), newTile);
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
    static void CreateLineMaterial() {
        if (!lineMaterial) {
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
    public void OnRenderObject() {
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
        Pos3 visionRadius = new Pos3(((long)(horizExtent)) + 1, ((long)(vertExtent)) + 1, 0); //TODO //PARAM z vision
        world.render(center, center - visionRadius - new Pos3(0,0,1), center + visionRadius);

        GL.PopMatrix();

        sw_oro.Stop();
        //TimeSpan ts = sw_oro.Elapsed;
        //ms_oro += ts.TotalMilliseconds;
        //count_oro++;
        //if (count_oro >= 10) {
        //    text_oro.text = "" + (((float)ms_oro) / count_oro);
        //    ms_oro = 0;
        //    count_oro = 0;
        //}
    }

    private int lineCount = 100;

    private void drawCircle(Vector3 pos, float r, bool filled, Color color) {
        if (filled) {
            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i) {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(pos.x, pos.y, pos.z);
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        } else {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for (int i = 0; i <= lineCount; ++i) {
                float a = i / (float)lineCount;
                float angle = a * Mathf.PI * 2;
                GL.Vertex3(Mathf.Cos(angle) * r + pos.x, Mathf.Sin(angle) * r + pos.y, pos.z);
            }
            GL.End();
        }

    }
}
