using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public delegate void SelectAction(GameObject go);
    public static event SelectAction onSelected;

    private void OnMouseDown()
    {
        onSelected(gameObject);
    }
}
