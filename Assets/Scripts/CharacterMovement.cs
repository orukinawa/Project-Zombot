using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class CharacterMovement : MonoBehaviour
{
	CharacterController charControl;
	public float moveSpeed;
	Vector3 move = Vector3.zero;
	
	void Start ()
	{
		charControl = GetComponent<CharacterController>();
	}
	
	void FixedUpdate ()
	{
		move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		
		if(move.sqrMagnitude > 1)
		{
			move.Normalize();
		}
		charControl.SimpleMove(move * moveSpeed);
	}
}
