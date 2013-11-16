using UnityEngine;
using System.Collections;

public class SpitterZombot : EnemyBase
{
	public override void Steer (Vector3 resultantVector)
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(resultantVector),mSteeringForce * Time.deltaTime);
		
		if(resultantVector.sqrMagnitude > Mathf.Epsilon)
		{	
			//transform.position += transform.forward * mMaxSpeed * Time.deltaTime;
			charController.SimpleMove(resultantVector);
		}
		
	}
	
	void Update()
	{
		GetFinalSteerDirection();
	}
}
