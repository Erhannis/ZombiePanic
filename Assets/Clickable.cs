using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Clickable : MonoBehaviour
{
    public Action onClick;

    void OnMouseDown()
    {
        if (onClick != null) {
            onClick();
        }
    }
}