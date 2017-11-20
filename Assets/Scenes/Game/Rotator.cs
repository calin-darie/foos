using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour {
	private HingeJoint _hinge;
	void Start() {
		var rigidBody = GetComponent<Rigidbody>();
		rigidBody.centerOfMass = Vector3.down;
		_hinge = GetComponent<HingeJoint>();
		var rod = GetComponentsInChildren<Transform>().First (c => c.name == "rod");
		_hinge.anchor = _hinge.transform.InverseTransformPoint(
			rod.GetComponent<Renderer> ().bounds.center);
	}

	// Update is called once per frame
	void Update () {
		var motor = _hinge.motor;
		float spin = Input.GetAxis ("Mouse X") * 5000;
		motor.targetVelocity = spin;
		_hinge.motor = motor;
	}
}
