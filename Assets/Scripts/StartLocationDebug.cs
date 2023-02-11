using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StartLocationDebug : MonoBehaviour
{
	void RenameStartLocations(GameObject overlook)
	{
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag("startLocation");
		int i = 1;
		foreach (GameObject go in gos)
		{
			if (go != overlook)
			{
				go.name = "SL" + string.Format("{0:000}", i);
				i++;
			}
		}
	}

	void OnDestroy()
	{
		RenameStartLocations(this.gameObject);
	}

	// Use this for initialization
	void Start()
	{
		if (this.transform.parent.gameObject.name != "StartLocation") return;
		RenameStartLocations(null);
	}

	// Update is called once per frame
	void Update()
	{
		this.GetComponent<TextMesh>().text = this.transform.parent.gameObject.name;
	}
}
