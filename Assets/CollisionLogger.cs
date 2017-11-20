using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLogger : MonoBehaviour {
	void OnCollisionEnter (Collision c)
	{
		print ("collision " + name + " - " + c.gameObject.name);
	}
		
}
