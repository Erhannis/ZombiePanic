using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StringFloatEvent : UnityEvent <string, float> {}

public class NamedSlider : MonoBehaviour
{
    public string name;
    
    public StringFloatEvent listeners;

    public void sendFloat(float value) {
        listeners.Invoke(name, value);
    }
}
