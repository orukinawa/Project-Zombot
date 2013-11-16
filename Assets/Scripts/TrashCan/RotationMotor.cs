using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]

public class RotationMotor : MonoBehaviour
{
	//[HideInInspector]
	public Vector3 facingDirection;
	
	public float turningSmoothing = 0.3f;
	
	void FixedUpdate()
	{
		if (facingDirection == Vector3.zero) 
		{
			rigidbody.angularVelocity = Vector3.zero;
		}
		else
		{
			float rotationAngle = AngleAroundAxis (transform.forward, facingDirection, Vector3.up);
			rigidbody.angularVelocity = (Vector3.up * rotationAngle * turningSmoothing);
		}
	}
	
	static float AngleAroundAxis (Vector3 dirA , Vector3 dirB , Vector3 axis) 
	{
	    // Project A and B onto the plane orthogonal target axis
	    dirA = dirA - Vector3.Project (dirA, axis);
	    dirB = dirB - Vector3.Project (dirB, axis);
	   
	    // Find (positive) angle between A and B
	    float angle = Vector3.Angle(dirA, dirB);
	   
	    // Return angle multiplied with 1 or -1
	    return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}
}
