using Jint;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Jibu;

public class JintTest : MonoBehaviour
{
    private const int TARGET_FPS = 1;

    public Text text;

    ChannelReader<int> syncA;
    ChannelWriter<int> syncB;

    private void Start()
    {
        {
            Channel<int> syncA0 = new Channel<int>();
            Channel<int> syncB0 = new Channel<int>();
            syncA = syncA0.ChannelReader;
            syncB = syncB0.ChannelWriter;
            //text.text = new JibuTest().Main();
            new JintRunner(
@"for (let i = 0; i < 10; i++) {
    move(Pos3(1,0,0));
}
while (true) {
    move(Pos3(0,0,0));
}"
            ).Start();
        }
    }

    void Awake() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TARGET_FPS;
    }

    private void Update() {
        if (Application.targetFrameRate != TARGET_FPS)
            Application.targetFrameRate = TARGET_FPS;

        Debug.Log("update start");
        syncA.Read();
        Debug.Log("update w/r");
        syncB.Write(0);
        Debug.Log("update end");
    }
}