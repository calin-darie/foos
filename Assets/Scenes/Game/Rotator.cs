using System.Linq;
using UnityEngine;

public class Rotator : MonoBehaviour {
	private ConfigurableJoint _joint;

    void Start() {
		var rigidBody = GetComponent<Rigidbody>();
		rigidBody.centerOfMass = Vector3.down;
		_joint = GetComponent<ConfigurableJoint>();
		var rod = GetComponentsInChildren<Transform>().First (c => c.name == "rod");
		_joint.anchor = _joint.transform.InverseTransformPoint(
			rod.GetComponent<Renderer> ().bounds.center);
    }

	// Update is called once per frame
	void Update ()
	{
	    var horizontalMovement = Input.GetAxis ("Mouse X");
	    Spin(horizontalMovement);
        
	    float verticalMovement = Input.GetAxis("Mouse Y");
	    Translate(verticalMovement);
	}

    private void Translate(float verticalMovement)
    {
        var movement = verticalMovement * 5;
        _joint.targetVelocity = Vector3.right * movement;
    }

    private void Spin(float horizontalMovement)
    {
        float spin = horizontalMovement * 100;
        _joint.targetAngularVelocity = Vector3.left * spin;
    }
}
