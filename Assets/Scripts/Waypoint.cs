using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public delegate void SelectAction(GameObject go, bool left = true);
    public static event SelectAction onSelected;

    private void OnMouseDown()
    {
        onSelected(gameObject);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            onSelected(gameObject, false);
        }
    }
}
