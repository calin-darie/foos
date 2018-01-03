using System.Linq;
using UnityEngine;

public class MouseControlledRod : MonoBehaviour {
    private const int Grip = 1000;
    private ConfigurableJoint _joint;
    public Transform Rod { get; private set; }
    public ManyMouse Mouse { get; set; }

    void Start() {
		var rigidBody = AddRigidbody();
		rigidBody.centerOfMass = Vector3.down;
		_joint = AddAndConfigureJoint();
		Rod = GetComponentsInChildren<Transform>().First (c => c.name == "rod");
		_joint.anchor = _joint.transform.InverseTransformPoint(
			Rod.GetComponent<Renderer> ().bounds.center);
    }

    private Rigidbody AddRigidbody()
    {
        var rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.mass = 5;
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
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
	    if (Mouse == null) return;
	    Vector2 delta = Mouse.Delta *  Time.deltaTime;
        var horizontalMovement = delta.x;
	    Spin(horizontalMovement);
        
	    float verticalMovement = delta.y;
	    Translate(verticalMovement);
	}

    private void Translate(float verticalMovement)
    {
        var movement = verticalMovement * 20;
        _joint.targetVelocity = Vector3.right * movement;
    }

    private void Spin(float horizontalMovement)
    {
        float spin = Mathf.Min(horizontalMovement * 50, 350);
        _joint.targetAngularVelocity = Vector3.left * spin;
    }
}
