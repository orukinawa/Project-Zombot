using UnityEngine;
using System.Collections;

public class LesserZombot : EnemyBase
{
	public override void Steer (Vector3 resultantVector)
	{
		Vector3 targetVelocity;
		float maxSpeed = mCurrSpeed;
		targetVelocity = resultantVector.normalized * maxSpeed;
		targetVelocity.y = 0;
		if(targetVelocity.sqrMagnitude > Mathf.Epsilon)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetVelocity),mSteeringForce * Time.deltaTime);
			//transform.position += transform.forward * mMaxSpeed * Time.deltaTime;
			charController.SimpleMove(transform.forward * maxSpeed);
		}
		else
		{
			charController.SimpleMove(Vector3.zero);
		}
	}
	
	void Update()
	{
		//Debug.Log("EnemyBase is calling");
		GetFinalSteerDirection();
	}
}
