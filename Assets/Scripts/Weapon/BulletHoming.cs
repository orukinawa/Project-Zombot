using UnityEngine;
using System.Collections;

// Looks for a target within range at the start 
public class BulletHoming : BulletCollider
{
	protected Transform target = null;
	public float targetDetectionRadius;
	protected float rotationStep;
	public float maxTurnSpeed;
	protected float initialDistance;
	protected float currDistance;
	public float seekingAngle;
	protected float cosHalfAngle;
	
	//Debug
//	Vector3 forward = Vector3.zero;
//	float f = 0.0f;
//	float l = 0.0f;
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);	
		cosHalfAngle = Mathf.Cos((seekingAngle/2.0f)*Mathf.Deg2Rad);		
		target = getClosestTarget();
		if(target != null)
		{
			initialDistance = Vector3.Distance(transform.position, target.transform.position);
		}
	}
	
	void FixedUpdate()
	{
		_Update();
		
		//Debug
//		f = targetDetectionRadius*Mathf.Cos((seekingAngle/2.0f)*Mathf.Deg2Rad);
//		l = targetDetectionRadius*Mathf.Sin((seekingAngle/2.0f)*Mathf.Deg2Rad);
//		
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward));
//		Debug.DrawLine(transform.position, transform.position + forward*targetDetectionRadius, Color.red);
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward*f + Vector3.left*l));
//		Debug.DrawLine(transform.position, transform.position + forward*targetDetectionRadius, Color.blue);
//		forward = Vector3.Normalize(transform.TransformDirection(Vector3.forward*f + Vector3.right*l));
//		Debug.DrawLine(transform.position, transform.position + forward*targetDetectionRadius, Color.blue);	
	}
	
	public override void _Update ()
	{
		if(target != null)
		{
			//currDistance = Vector3.Distance(transform.position, target.transform.position);
			//rotationStep = (distanceTravelled/initialDistance) * maxTurnSpeed;
			rotationStep = ((distanceTravelled)/initialDistance) * maxTurnSpeed;
			if(rotationStep > 0)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), rotationStep);
				direction = transform.TransformDirection(Vector3.forward);
			}
		}
		
		//base.SetVelocity();
		base._Update ();
	}
	
	void OnTriggerEnter(Collider col)
	{
		base._OnTriggerEnter(col);
	}
	
	Transform getClosestTarget()
	{
		float closestDistance = Mathf.Infinity;
		float tempSqrtDistance = 0.0f;
		Transform tranformToReturn = null;
		Vector2 localForward = convertToVector2(transform.TransformDirection(Vector3.forward));
		foreach(Collider col in Physics.OverlapSphere(transform.position, targetDetectionRadius))
		{
			if(col.GetComponent<StatsEnemy>() != null)
			{
				// Checks if collider is within angle
				Vector2 tempVect2 = convertToVector2(col.transform.position - transform.position);					
				if(Vector2.Dot(localForward.normalized,tempVect2.normalized) > cosHalfAngle)
				{
					// Checks if collider is closest
					tempSqrtDistance = (transform.position - col.transform.position).sqrMagnitude;
					if(tempSqrtDistance < closestDistance)
					{
						tranformToReturn = col.transform;
						closestDistance = tempSqrtDistance;
					}
				}
			}
		}
		
		return tranformToReturn;
	}
	
	Vector2 convertToVector2(Vector3 v)
	{
		return new Vector2(v.x,v.z);
	}
	
	
}
