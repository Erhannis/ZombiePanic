using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static Dictionary<string, object> globals = new Dictionary<string, object>();

    public void loadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public void saveGlobalFloat(string name, float value) {
        Debug.Log("saveGlobalFloat " + name + " " + value);
        globals[name] = value;
    }

    public void saveGlobalBool(string name, bool value) {
        Debug.Log("saveGlobalBool " + name + " " + value);
        globals[name] = value;
    }
}
