using System.Linq;
using UnityEngine;

public class MouseControlledRod : MonoBehaviour {
    private const int Grip = 1000;
    private ConfigurableJoint _joint;

    void Start() {
		var rigidBody = AddRigidbody();
		rigidBody.centerOfMass = Vector3.down;
		_joint = AddAndConfigureJoint();
		var rod = GetComponentsInChildren<Transform>().First (c => c.name == "rod");
		_joint.anchor = _joint.transform.InverseTransformPoint(
			rod.GetComponent<Renderer> ().bounds.center);
    }

    private Rigidbody AddRigidbody()
    {
        var rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 5;
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        return rigidBody;
    }

    private ConfigurableJoint AddAndConfigureJoint()
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.anchor = new Vector3(0.5f, 0.6f, 3.08f);
        joint.axis = Vector3.right;
        joint.autoConfigureConnectedAnchor = true;
        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Free;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.xDrive = new JointDrive
        {
            positionDamper = Grip,
            positionSpring = 0,
            maximumForce = float.MaxValue
        };
        joint.angularXDrive = new JointDrive
        {
            positionDamper = Grip,
            positionSpring = 0,
            maximumForce = float.MaxValue
        };
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        return joint;
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
