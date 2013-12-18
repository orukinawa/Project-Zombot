using UnityEngine;
using System.Collections.Generic;

public class SeekBehaviour : BehaviourBase
{	
	public LayerMask mTargetLayer;
 	public float mMinSeekDist = 1.2f;
	float mDetectionRange;
	float mMinSeekDistSqr;
	
	public AnimationClip WalkAnimation;
	public float WalkAnimationSpd = 8.0f;
	
	public AnimationClip IdleAnimation;
	
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
				
				if(IdleAnimation != null)
				{
					// play idle animation
					enemyBase.Animator.CrossFade(IdleAnimation,WrapMode.Loop);
				}
				
				return Vector3.zero;
			}
			else
			{
				if(WalkAnimation != null){
					// play walk animation
					enemyBase.Animator.CrossFade(WalkAnimation,WrapMode.Loop,WalkAnimationSpd);	
				}
			}
		}		
		return resultDir;
	}
}
