using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public ObjectPool pool;

    private List<GameObject> objs = new List<GameObject>();
    private Random rand = new Random();

    public int targetFPS = 30;
      
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

        foreach (GameObject o in objs) {
            pool.PoolObject(o);
        }
        objs.Clear();
        
        for (int i = 0; i < 10; i++) {
            GameObject obj = pool.GetObjectForType("Circle", false);
            obj.transform.position = new Vector3(Random.Range(-4f,4f), Random.Range(-4f,4f), 1);
            objs.Add(obj);
        }        
    }
}
