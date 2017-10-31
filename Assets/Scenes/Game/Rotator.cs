using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour {
	public Vector3 rotationCenter;
	void Start() {
		var rod = gameObject.GetComponentsInChildren<Transform> ()
			.First (o => o.name == "rod");
		var rodRenderer = rod.GetComponent<Renderer> ();
		rotationCenter = rodRenderer.bounds.center;
	}

	// Update is called once per frame
	void Update () {
		transform.RotateAround(
			rotationCenter, 
			new Vector3(1, 0, 0), 
			180 * Time.deltaTime);
	}
}
