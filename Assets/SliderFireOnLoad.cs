using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFireOnLoad : MonoBehaviour
{
    void Start()
    {
        Slider s = GetComponent<Slider>();
        s.onValueChanged.Invoke(s.value);
    }
}
