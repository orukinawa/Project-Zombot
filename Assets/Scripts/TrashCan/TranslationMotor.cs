using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]

public class TranslationMotor : MonoBehaviour
{
	[HideInInspector]
	public Vector3 movementDirection;
	
	public float defaultMoveSpeed;
	private float currentMoveSpeed;
	public float walkingSnappyness = 50.0f;
	
	//Cache
	Vector3 deltaVelocity = Vector3.zero;
	
	void Awake()
	{
		currentMoveSpeed = defaultMoveSpeed;
	}
	
	void FixedUpdate () 
	{
		// Handle the movement of the character
		
		//Cached
//		Vector3 targetVelocity = new Vector3(movementDirection * walkingSpeed);
//		Vector3 deltaVelocity = new Vector3(targetVelocity - rigidbody.velocity);
		
		deltaVelocity = (movementDirection * currentMoveSpeed) - rigidbody.velocity;
		
		if (rigidbody.useGravity)
		{
			deltaVelocity.y = 0;
		}
		rigidbody.AddForce (deltaVelocity * walkingSnappyness, ForceMode.Acceleration);
	}
}
