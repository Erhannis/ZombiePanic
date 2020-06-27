using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static Dictionary<string, object> globals = new Dictionary<string, object>();

    public void loadScene(string scene) {
        SceneChanger.staticLoadScene(scene);
    }

    public static void staticLoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public void saveGlobalFloat(string name, float value) {
        Debug.Log("saveGlobalFloat " + name + " " + value);
        globals[name] = value;
    }

    public void saveGlobalInt(string name, float value) {
        Debug.Log("saveGlobalInt " + name + " " + ((int)value));
        globals[name] = (int)value;
    }

    public void saveGlobalInt(string name, int value) {
        Debug.Log("saveGlobalInt " + name + " " + value);
        globals[name] = value;
    }

    public void saveGlobalBool(string name, bool value) {
        Debug.Log("saveGlobalBool " + name + " " + value);
        globals[name] = value;
    }

    public void saveGlobalString(string name, string value) {
        Debug.Log("saveGlobalString " + name + " " + value);
        globals[name] = value;
    }
}
