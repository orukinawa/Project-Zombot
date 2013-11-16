using UnityEngine;
using System.Collections.Generic;

public class SeekBehaviour : BehaviourBase
{	
	public LayerMask mTargetLayer;
 	public float mMinSeekDist = 1.2f;
	float mDetectionRange;
	float mMinSeekDistSqr;
	
	//! returns 0 if a is nearer and returns 1 if b is nearer to the point
	public int GetNearestPos(Vector3 a, Vector3 b, Vector3 point)
	{
		float distanceA = 0.0f;
		float distanceB = 0.0f;
		distanceA = Vector3.SqrMagnitude(a - point);
		distanceB = Vector3.SqrMagnitude(b - point);
		
		if(distanceA <= distanceB)
		{
			return 0;
		}
		else
		{
			return 1;
		}
	}
	
	public override void Init (EnemyBase enemyBase)
	{
		mMinSeekDistSqr = mMinSeekDist * mMinSeekDist;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		mDetectionRange = enemyBase.mDetectionRadius;
		
		//! get the collider radius and sqr it
		float radiusSqr = enemyBase.charController.radius * enemyBase.charController.radius;
		
		//! constantly search for the nearest enemy
		SearchForNewTarget(enemyBase,mDetectionRange,mTargetLayer);
		Vector3 resultDir = Vector3.zero;
		
		if(enemyBase.mTargetPlayer != null)
		{
			resultDir = enemyBase.mTargetPlayer.transform.position - enemyBase.gameObject.transform.position;
			//Debug.Log("radiussqr: " + radiusSqr);
			if(resultDir.sqrMagnitude < mMinSeekDistSqr + radiusSqr)
			{
				float targetAngle = GetAngleHelper.GetAngle(resultDir,enemyBase.transform.forward,enemyBase.transform.up);
				//! manual rotate
				if(Mathf.Abs(targetAngle) > 3.0f)
				{
					//Debug.Log("rotating");
					//enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
					resultDir.y = 0.0f;
					enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(resultDir), enemyBase.mSteeringForce * Time.deltaTime);
				}
				
				return Vector3.zero;
			}
		}		
		return resultDir;
	}
}
