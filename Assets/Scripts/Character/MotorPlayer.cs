using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody))]

public class MotorPlayer : MotorBase
{
	public float defaultmoveSpeed = 5.0f;
	public float currentMoveSpeed;
	public float turningSmoothing = 0.3f;
	public float dashSpeed = 25.0f;
	
	//private Rigidbody mRigidbody;
	private CharacterController charControl;
	
	void Awake()
	{
		//mRigidbody = rigidbody;
		charControl = GetComponent<CharacterController>();
		currentMoveSpeed = defaultmoveSpeed;
	}
	
	public override void Move (Vector3 movementDirection)
	{
		Vector3 targetVelocity = movementDirection * currentMoveSpeed;
		charControl.SimpleMove(targetVelocity);
	}
	
	public override void Rotate (Vector3 facingDirection)
	{
		if(facingDirection.sqrMagnitude == 0)
		{
			return;
		}
		Vector3 faceDir = facingDirection;
		float rotationAngle = AngleAroundAxis (transform.forward, faceDir, Vector3.up);
		transform.Rotate(Vector3.up, rotationAngle * turningSmoothing);
	}
	
	public override void Dash (Vector3 dashDirection)
	{
		charControl.SimpleMove(dashDirection*dashSpeed);
	}
	
	void Update() 
	{
		// Handle the movement of the character
		//Vector3 targetVelocity = movementDirection * currentMoveSpeed;
		//Vector3 deltaVelocity = targetVelocity - mRigidbody.velocity;
//		if (mRigidbody.useGravity)
//		{
//			deltaVelocity.y = 0.0f;
//		}
		//mRigidbody.AddForce (deltaVelocity, ForceMode.Acceleration);
		//charControl.SimpleMove(targetVelocity);
		
//		// Setup player to face facingDirection, or if that is zero, then the movementDirection
		//Vector3 faceDir = facingDirection;
//		if (faceDir == Vector3.zero)
//		{
//			faceDir = movementDirection;
//		}
		
		// Make the character rotate towards the target rotation
//		if (faceDir == Vector3.zero) 
//		{
//			mRigidbody.angularVelocity = Vector3.zero;
//		}
//		else 
//		{
		//float rotationAngle = AngleAroundAxis (transform.forward, faceDir, Vector3.up);
			//mRigidbody.angularVelocity = (Vector3.up * rotationAngle * turningSmoothing);
			//Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
		//transform.Rotate(Vector3.up, rotationAngle * turningSmoothing);
//		}
	}
	
	// The angle between dirA and dirB around axis
	float AngleAroundAxis (Vector3 dirA , Vector3 dirB , Vector3 axis) 
	{
	    // Project A and B onto the plane orthogonal target axis
	    dirA = dirA - Vector3.Project (dirA, axis);
	    dirB = dirB - Vector3.Project (dirB, axis);
	   
	    // Find (positive) angle between A and B
	    float angle = Vector3.Angle (dirA, dirB);
	   
	    // Return angle multiplied with 1 or -1
	    return angle * (Vector3.Dot (axis, Vector3.Cross (dirA, dirB)) < 0 ? -1 : 1);
	}	
}
