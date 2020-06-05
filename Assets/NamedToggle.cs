using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StringBoolEvent : UnityEvent <string, bool> {}

public class NamedToggle : MonoBehaviour
{
    public string name;
    
    public StringBoolEvent listeners;

    public void sendBool(bool value) {
        listeners.Invoke(name, value);
    }
}
