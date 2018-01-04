using System.Linq;
using UnityEngine;

public class MouseControlledRod : MonoBehaviour {
    private const int Grip = 200;
    private ConfigurableJoint _joint;
    public Transform Rod { get; private set; }
    public ManyMouse Mouse { get; set; }

    void Start()
    {
        AddRigidbody();
        Rod = GetComponentsInChildren<Transform>().First(c => c.name == "rod");
        _joint = AddAndConfigureJoint(Rod);
    }

    private Rigidbody AddRigidbody()
    {
        var rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        return rigidBody;
    }

    private ConfigurableJoint AddAndConfigureJoint(Transform rod)
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.anchor = joint.transform.InverseTransformPoint(
            rod.GetComponent<Renderer>().bounds.center);
        joint.breakForce = Mathf.Infinity;
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
            maximumForce = 1000
        };
        joint.angularXDrive = new JointDrive
        {
            positionDamper = Grip,
            positionSpring = 0,
            maximumForce = 1000
        };
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;
        return joint;
    }

    // Update is called once per frame
	void Update ()
	{
	    if (Mouse == null) return;
	    Vector2 mouseSpeed = Mouse.Delta / (Time.deltaTime * 10);
        var horizontalSpeed = mouseSpeed.x;
	    Spin(horizontalSpeed);
        
	    float verticalSpeed = mouseSpeed.y;
	    Translate(verticalSpeed);
	}

    private void Translate(float verticalSpeed)
    {
        var movement = verticalSpeed * 5;
        _joint.targetVelocity = Vector3.right * movement;
    }

    private void Spin(float horizontalSpeed)
    {
        float spin = Mathf.Min(horizontalSpeed, 1200);
        _joint.targetAngularVelocity = Vector3.left * spin;
    }
}
