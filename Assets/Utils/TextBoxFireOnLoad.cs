using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxFireOnLoad : MonoBehaviour {
    void Start() {
        InputField tb = GetComponent<InputField>();
        tb.onValueChanged.Invoke(tb.text);
    }
}
