using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFireOnLoad : MonoBehaviour
{
    void Start()
    {
        Toggle t = GetComponent<Toggle>();
        t.onValueChanged.Invoke(t.isOn);
    }
}
