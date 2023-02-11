using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GoalLocationDebug : MonoBehaviour {

	void RenameGoalLocations(GameObject overlook)
	{
		GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("goalLocation");
        int i = 1;
	    foreach (GameObject go in gos)
        {
            if (go != overlook)
	     	{
                go.name = "GL" + string.Format("{0:000}", i);
                i++;
            } 
	    }	
	}

	void OnDestroy()
	{
		RenameGoalLocations(this.gameObject);
	}

	// Use this for initialization
	void Start () {
        if (this.transform.parent.gameObject.name != "GoalLocation") return;
        RenameGoalLocations(null);
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<TextMesh>().text = this.transform.parent.gameObject.name;
	}
}
