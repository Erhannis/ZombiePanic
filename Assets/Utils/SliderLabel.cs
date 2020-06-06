using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderLabel : MonoBehaviour
{
    private Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void update(string asdf, string qwer) {
        text.text = asdf;
    }

    public void updateInt(float value) {
        text.text = ((int)value) + "";
    }

    public void updateFloat(float value) {
        text.text = value + "";
    }
}
