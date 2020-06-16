using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StringStringEvent : UnityEvent<string, string> { }

public class NamedTextBox : MonoBehaviour {
    public string name;

    public StringStringEvent listeners;

    public void sendString(string value) {
        listeners.Invoke(name, value);
    }
}
