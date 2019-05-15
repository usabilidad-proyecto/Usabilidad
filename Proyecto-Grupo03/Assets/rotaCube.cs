using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotaCube : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.RotateAroundLocal(Vector3.up, 0.1f);
   
	}
}
