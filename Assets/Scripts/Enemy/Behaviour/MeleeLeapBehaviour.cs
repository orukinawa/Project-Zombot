
using UnityEngine;
using System.Collections;

class MeleeLeapBehaviourData
{
	public float mCoolDownLeapTimer;
	//public float mDefaultSpeed;
	public Vector3 mLeapTarget;
	public bool mIsAttacking;
	public bool mIsLeaping;
}

public class MeleeLeapBehaviour : BehaviourBase
{
	public enum TARGET_TYPE
	{
		SINGLE,
		MULTIPLE,
	}
	
	//! debug
	//float leapMagnitude;
	//bool isAttacking;
	
	public TARGET_TYPE mTargetType;
	public float mLeapSpeed = 2;
	public float mDamage = 10;
	public LayerMask mTargetLayer;
	public float mCoolDownLeapDuration = 1;
	public float mAttackReach = 1;
	public float mAttackAngle = 45;
	//! min dist needs to be above 1 cause will be squared
	float mMinDist = 1.2f;
	float mMinDistSqr;
	
	public GameObject mClawEffect;
	
	// the animation names
	public AnimationClip LeapAnimationClip;
	public AnimationClip IdleAnimationClip;
	
//	public bool isleaping;
//	public bool isAttacking;
	
	public override void Init (EnemyBase enemyBase)
	{
		MeleeLeapBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new MeleeLeapBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (MeleeLeapBehaviourData)enemyBase.mCustomData[this];
		}
		data.mIsAttacking = false;
		data.mIsLeaping = false;
		data.mLeapTarget = Vector3.zero;
		data.mCoolDownLeapTimer = mCoolDownLeapDuration;
		mMinDistSqr = mMinDist * mMinDist;
		// set the callback when an animation is complete
		enemyBase.Animator.IsComplete(LeapAnimationClip,OnCompleteLeap);
		//! cache the max speed
	}
	
	public override void DeInit (EnemyBase enemyBase)
	{
		enemyBase.Animator.mCurrentClip = null;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		MeleeLeapBehaviourData data = (MeleeLeapBehaviourData)enemyBase.mCustomData[this];
		float attackRadius = enemyBase.mAttackRadius;
		
		if(enemyBase.mTargetPlayer == null)
		{
			SearchForNewTarget(enemyBase,attackRadius,mTargetLayer);
		}
		
		//isAttacking = data.mIsAttacking;
		
		Vector3 targetPos = enemyBase.mTargetPlayer.transform.position;
		//! rotate
		Vector3 targetDirection =  targetPos - enemyBase.transform.position;
		
		// if you are attacking or leaping don't rotate
		if(!data.mIsAttacking && !data.mIsLeaping)
		{
			//! Get the rotation angle
			float targetAngle = GetAngleHelper.GetAngle(targetDirection,enemyBase.transform.forward,enemyBase.transform.up);
			if(Mathf.Abs(targetAngle) > 3.0f)
			{
				//Debug.Log("rotating");
				//enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
				targetDirection.y = 0.0f;
				enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(targetDirection), enemyBase.mSteeringForce * Time.deltaTime);
			}
		}
		
		// calculate cooldown before the next attack attempt
		data.mCoolDownLeapTimer += Time.deltaTime;
		if(data.mCoolDownLeapTimer > mCoolDownLeapDuration)
		{
			//! check if in view of sight to leap
			float angle = Vector3.Angle(targetDirection,enemyBase.transform.forward);
			if(angle < 10.0f && !data.mIsAttacking && !data.mIsLeaping)
			{
				//Debug.Log("set variables");
				data.mIsAttacking = true;
				data.mIsLeaping = true;
				data.mLeapTarget = targetPos;
			}
			
			
			if(data.mIsAttacking)
			{
				//! do attacking animation here
				Vector3 leapDirection = data.mLeapTarget - enemyBase.transform.position;
				//leapMagnitude = leapDirection.sqrMagnitude;
				
				enemyBase.Animator.CrossFade(LeapAnimationClip,WrapMode.Once);
			
				if(data.mIsLeaping)
				{
					// if touch player will return false
					data.mIsLeaping = CheckSingleTarget(enemyBase,mAttackAngle);
					//Debug.Log("checking for hit: " + data.mIsLeaping);
					// calculate movement of the leap
					if(leapDirection.sqrMagnitude > mMinDistSqr)
					{
						enemyBase.charController.SimpleMove(enemyBase.transform.forward * enemyBase.mCurrSpeed * mLeapSpeed);
					}
					else
					{
						// if leap and misses the target
						data.mIsLeaping = false;
					}
				}
			}
		}
		
		return Vector3.zero;
	}
	
	//! use for single target type checking
	public bool CheckSingleTarget(EnemyBase enemyBase, float attackAngle)
	{
		Vector3 pos = enemyBase.transform.position;
		Vector3 dirAttack = enemyBase.transform.forward;
		//! assuming all enemy using character controller
		float colliderRad = enemyBase.charController.radius;
		
		Collider[] colliders = Physics.OverlapSphere(pos,mAttackReach + colliderRad,mTargetLayer);
		
		float smallestAngle = Mathf.Infinity;
		Collider singleTarget = null;
		foreach(Collider col in colliders)
		{
			Vector3 targetDir = col.transform.position - pos;
			float angle = Vector3.Angle(dirAttack,targetDir);
			if(angle < attackAngle)
			{
				if(angle < smallestAngle)
				{
					smallestAngle = angle;
					singleTarget = col;
				}
			}
		}
		if(singleTarget)
		{
			//! if hit an enemy
			Instantiate(mClawEffect,singleTarget.transform.position,Quaternion.identity);
			singleTarget.GetComponent<StatsCharacter>().currentHealth -= mDamage;
			return false;
		}
		return true;
	}
	
	//! use for multiple target type checking
	public bool CheckMultipleTarget(EnemyBase enemyBase, float attackAngle)
	{
		Vector3 pos = enemyBase.transform.position;
		Vector3 dirAttack = enemyBase.transform.forward;
		//! assuming all enemy using character controller
		float colliderRad = enemyBase.charController.radius;
		
		Collider[] colliders = Physics.OverlapSphere(pos,mAttackReach + colliderRad,mTargetLayer);
		bool hitSomething = false;
		
		foreach(Collider col in colliders)
		{
			Vector3 targetDir = col.transform.position - pos;
			float angle = Vector3.Angle(dirAttack,targetDir);
			if(angle < attackAngle)
			{
				hitSomething = true;
				col.GetComponent<StatsCharacter>().currentHealth -= mDamage;
			}
		}
		
		if(hitSomething)
		{
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Raises the complete leap event.
	/// </summary>
	/// <param name='enemyBase'>
	/// If set to <c>true</c> enemy base.
	/// </param>
	bool OnCompleteLeap(EnemyBase enemyBase)
	{
		MeleeLeapBehaviourData data = (MeleeLeapBehaviourData)enemyBase.mCustomData[this];
		
		//Debug.Log("[MELEE_LEAP_BEHAVIOUR] Leap is complete");
		//! if leap and miss towards the leap destination
		data.mIsAttacking = false;
		data.mIsLeaping = false;
		data.mCoolDownLeapTimer = 0.0f;
		enemyBase.Animator.CrossFade(IdleAnimationClip,WrapMode.Loop);
		
		return false;
	}
	
//	void OnGUI()
//	{
//		GUI.Label (new Rect(0,50,200,50),"Leap: " + isleaping);
//		GUI.Label(new Rect(0,100,200,50),"isAttacking: " + isAttacking);
//	}
}
