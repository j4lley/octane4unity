using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pedestrianPath : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.black;
		for(int i = 0;i<transform.childCount-1;i++){
			Gizmos.DrawLine (transform.GetChild(i).transform.position, transform.GetChild(i+1).transform.position);
			Gizmos.DrawSphere (transform.GetChild (i).transform.position, 0.2f);
		}
		Gizmos.DrawSphere (transform.GetChild (transform.childCount-1).transform.position, 0.2f);
	}
}
