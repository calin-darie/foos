using UnityEngine;

public class BallTracker : MonoBehaviour
{
    public Transform Ball;
    SphereCollider _ballCollider;
    Vector3 _center;
    bool _ballActive;
    Rigidbody _ballRigidbody;
    Vector3 _nextReseBallPositiontOffset = 2.5f*Vector3.forward + 0.5f * Vector3.right;

    private bool BallActive
    {
        get { return Ball.gameObject.activeSelf; }
        set { Ball.gameObject.SetActive(value); }
    }

    void Start ()
	{
	    _ballCollider = Ball.GetComponent<SphereCollider>();
	    _ballRigidbody = Ball.GetComponent<Rigidbody>();
	    _center = GetComponent<Renderer>().bounds.center;
	    InitBallPosition();
    }

    void Update()
	{
	    if (Input.GetKeyDown(KeyCode.R))
	    {
	        StopBall();
	        InitBallPosition();
	        BallActive = true;
	    }
	}

    private void InitBallPosition()
    {
        _nextReseBallPositiontOffset = -_nextReseBallPositiontOffset;
        Ball.position = _center + _ballCollider.radius * Vector3.up + _nextReseBallPositiontOffset;
    }

    private void StopBall()
    {
        _ballRigidbody.velocity = Vector3.zero;
        _ballRigidbody.angularVelocity = Vector3.zero;
    }


    void OnTriggerExit(Collider other)
    {
        if (other == _ballCollider)
        {
            StopBall();
            BallActive = false;
        }
    }
}
