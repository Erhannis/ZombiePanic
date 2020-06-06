using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Entities;

public class Main : MonoBehaviour
{
    private enum ActionMode {
        MOVE,
        DIG,
        AUTODIG,
        PLACE //TODO Differentiate between block types
    }

    private System.Random rand = new System.Random();

    private const int TARGET_FPS = 60;

    private const int PX_PER_UNIT = 100;
    private const float BOARD_WIDTH = 22.0f; //TODO Calc from screen?
    private const float TIMESCALE = 2f; //TODO Time

    private World world;
    private Rect playBounds; //TODO ????

    private MPos3 playerPos; //TODO Note - player needs an avatar in-world
    private Broodmother player;

    private ActionMode actionMode;

    void Start()
    {
        // int playerCount = (int)(float)SceneChanger.globals["playercount_float"];
        // bool rapidfire = (bool)SceneChanger.globals["rapidfire_bool"];
        Init();
    }
 
    private List<Color> predefTankColors = new List<Color> {Color.blue, Color.red, Color.green, Color.magenta, Color.yellow, Color.cyan};

    void Init() {
        world = new World();
        playerPos = new MPos3(0,0,0);
        player = new Broodmother();
        actionMode = ActionMode.MOVE;
        world.getTile(playerPos.toPos3()).contents.Add(player);
        
        // playBounds = new Rect(i2x(0),-50f,BOARD_WIDTH,200f); // Extra high, for high shots
    }
    
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }
      
    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;
        //Camera.main.GetComponent<Camera>().orthographicSize = (0.5f * playBounds.width * Screen.height) / Screen.width;

        Camera.main.backgroundColor = new Color(0,0,0); //TODO Move elsewhere?

        Debug.Log("//TODO Remove reset key");
        if (Input.GetKeyDown("r")) {
            Init();
            return;
        }
        if (Input.GetKeyDown("q")) {
            SceneChanger.staticLoadScene("MainMenu");
            return;
        }


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
    }

    private void doPlayerInput() {
        // if (Input.GetMouseButtonUp(0)) { // Left click (0-left,1-right,2-middle)
        //     var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     Vector2 tap = new Vector2(ray.origin.x, ray.origin.y);
        // }

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
        if (Input.GetKeyDown(KeyCode.Space)) {
            dir.z++;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            dir.z--;
        }
        if (!dir.Equals(new MPos3(0,0,0))) {
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

    private bool tryPlayerAction(MPos3 dir, ActionMode mode) {
        //TODO Test or something
        //TODO Should maybe have a world.moveEntity() or something
        MPos3 newMPos = playerPos + dir;
        Pos3 newPos = newMPos.toPos3();
        Tile newTile = world.getTile(newPos);

        switch (mode) {
            case ActionMode.AUTODIG: // AUTODIG defaults first to movement
            case ActionMode.MOVE:
                foreach (Entity e in newTile.contents) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                world.getTile(playerPos.toPos3()).contents.Remove(player);
                playerPos = newMPos;
                newTile.contents.Add(player);
                return true;
            case ActionMode.DIG:
                Entity digged = null;
                foreach (Entity e in newTile.contents) {
                    if (e.blocksMovement()) {
                        digged = e;
                        break;
                    }
                }
                if (digged != null) {
                    newTile.contents.Remove(digged);
                    player.inventory.Add(digged);
                    return true;
                } else {
                    return false;
                }
            case ActionMode.PLACE:
                foreach (Entity e in newTile.contents) {
                    if (e.blocksMovement()) {
                        return false;
                    }
                }
                if (player.inventory.Count == 0) {
                    return false;
                }
                Entity placed = player.inventory[player.inventory.Count-1];
                player.inventory.RemoveAt(player.inventory.Count-1);
                newTile.contents.Add(placed);
                return true;
            default:
                return false;
        }
    }

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
        var vertExtent = Camera.main.GetComponent<Camera>().orthographicSize;    
        var horizExtent = vertExtent * Screen.width / Screen.height;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        Pos3 center = playerPos.toPos3();
        Pos3 visionRadius = new Pos3(((long)(horizExtent))+1,((long)(vertExtent))+1,2); //TODO //PARAM z vision
        world.render(center, center - visionRadius, center + visionRadius);

        GL.PopMatrix();
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
