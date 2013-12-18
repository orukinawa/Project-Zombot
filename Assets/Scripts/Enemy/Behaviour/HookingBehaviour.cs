using UnityEngine;
using System.Collections;

class HookingBehaviourData
{
	public GameObject mHookTarget;
	public GameObject mProjectileRef;
	public float mAnimationDelayTimer;
	public bool mReverseHook;
}

public class HookingBehaviour : BehaviourBase
{
	//! the visual
	public GameObject mHookProjectile;
	public LayerMask mTargetLayer;
	public float mAnimationDelay;
	public float mHookSpeed;
	//public float mHookMaxDist;
	public float mHookMinDist;
	public float mHookRadius;
	// the hook distance that can travel
	public float HookMaxDist;
	// speak for itself
	public float HookDamage = 10;
	
	float mHookMaxDistSqr;
	float mHookMinDistSqr;
	
	public AnimationClip HookAnimationClip;
	public AnimationClip IdleAnimationClip;
	
	public float HookAnimationSpeed;
	
	public GameObject HookParticleEffect;
	
	public override void Init (EnemyBase enemyBase)
	{
		HookingBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new HookingBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (HookingBehaviourData)enemyBase.mCustomData[this];
		}
		data.mHookTarget = null;
		data.mAnimationDelayTimer = 0.0f;
		data.mReverseHook = false;
		
		if(data.mProjectileRef != null)
		{
			data.mProjectileRef.transform.position = enemyBase.transform.position;
		}
		
		//! cache the min and max distance
		//mHookMaxDistSqr = mHookMaxDist * mHookMaxDist;
		
		if(HookMaxDist <= 3.0f)
		{
			HookMaxDist = 3.0f;
		}
		
		// set the max hook distance
		mHookMaxDistSqr = HookMaxDist * HookMaxDist;
		
		// set the min hook distance
		mHookMinDistSqr = mHookMinDist * mHookMinDist;
	}
	
	void ApplyHookEffect(GameObject hookTarget)
	{
		StatsCharacter statCh = hookTarget.GetComponent<StatsCharacter>();
		if(!statCh)return;
		
		Instantiate(HookParticleEffect, hookTarget.transform.position, Quaternion.identity);
		// change the layer so that it cannot be hook by another
		statCh.gameObject.layer = LayerMask.NameToLayer("HookedPlayer");
		statCh.currentHealth -= HookDamage;
		statCh.Immobilize();
	}
	
	public void ReleaseTarget(EnemyBase enemyBase)
	{
		//! TODO remove parent and child relationship with the projectile and the player in case of death
		HookingBehaviourData data = (HookingBehaviourData)enemyBase.mCustomData[this];
		if(data.mHookTarget != null)
		{
			data.mHookTarget.transform.parent = null;
			data.mHookTarget.layer = LayerMask.NameToLayer("Player");
			data.mHookTarget.transform.position += Vector3.up * 1.0f;
			data.mHookTarget.GetComponent<StatsCharacter>().RemoveImmobilize();
		}
		
		//Destroy(data.mProjectileRef);
		data.mProjectileRef.SetActive(false);
	}
	
	public override void DeInit (EnemyBase enemyBase)
	{
		ReleaseTarget(enemyBase);
	}
	
	public override void Death (EnemyBase enemyBase)
	{
		ReleaseTarget(enemyBase);
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		HookingBehaviourData data = (HookingBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		Vector3 dir = enemyBase.transform.forward;
		
		data.mAnimationDelayTimer += Time.deltaTime;
		if(data.mAnimationDelayTimer <= mAnimationDelay)
		{
			//! do animation preparing to throw the hook
			//! best to use the frame as the duration
			enemyBase.Animator.CrossFade(HookAnimationClip,WrapMode.Loop,HookAnimationSpeed);
		}
		else
		{
			enemyBase.Animator.CrossFade(IdleAnimationClip,WrapMode.Loop,2.0f);
			
			if(data.mProjectileRef == null)
			{
				data.mProjectileRef = (GameObject)Instantiate(mHookProjectile,pos,Quaternion.identity);
				data.mProjectileRef.transform.rotation = enemyBase.transform.rotation;
			}
			else
			{
				if(!data.mProjectileRef.activeSelf)
				{
					data.mProjectileRef.SetActive(true);
					data.mProjectileRef.transform.rotation = enemyBase.transform.rotation;
				}
			}
			
			Vector3 hookVector = data.mProjectileRef.transform.position - enemyBase.transform.position;
			hookVector.y = 0.0f;
			
			// pull back
			if(data.mReverseHook)
			{
				//! once an object is hook
				data.mProjectileRef.transform.position -= (hookVector.normalized * mHookSpeed) * Time.deltaTime;
				if(hookVector.sqrMagnitude < mHookMinDistSqr)
				{
					//! if player was hooked
					if(data.mHookTarget != null)
					{
						ReleaseTarget(enemyBase);
					}
					// go back to other state
					ExecuteTransition(enemyBase);
					// destroy the hook projectile
					//Destroy(data.mProjectileRef);
					if(!data.mProjectileRef.activeSelf)
					{
						data.mProjectileRef.SetActive(false);
					}
				}
				
				return Vector3.zero;
			}
			// throw hook
			else
			{
				data.mProjectileRef.transform.position += (dir * mHookSpeed) * Time.deltaTime;
				
				//! hook checks if any player target is near him
				GameObject hookObj = GetNearestToTarget(data.mProjectileRef.transform.position, mHookRadius, mTargetLayer);
				
//				Debug.Log("hookVector.sqrMagnitude: " + hookVector.sqrMagnitude);
//				Debug.Log("mHookMaxDistSqr: " + mHookMaxDistSqr);
				
				// if hook reach it's maximum length
				if(hookVector.sqrMagnitude >= mHookMaxDistSqr)
				{
					data.mReverseHook = true;
				}
				// if hook misses
				if(hookObj == null)
				{
					//Debug.Log("LOL");
					return Vector3.zero;
				}
				
				// if they hook obj is player
				if(hookObj.layer == LayerMask.NameToLayer("Player"))
				{
					// assign the player reference to the hook reference
					data.mHookTarget = hookObj;
					// apply dmg and projectile
					ApplyHookEffect(hookObj);
					// set the player's parent as the projectile
					hookObj.transform.parent = data.mProjectileRef.transform;
					// reverse the hook
					data.mReverseHook = true;
				}
				// if the hook obj is an environment
				else if(hookObj.layer == LayerMask.NameToLayer("Environment"))
				{
					// reverse the hook
					data.mReverseHook = true;
				}
			}
		}
		
		//! get the direction
		if(enemyBase.mTargetPlayer == null)
		{
			//! hardcode layer because the targetlayer may involve environment layer as well
			SearchForNewTarget(enemyBase, enemyBase.mAttackRadius, 1 << LayerMask.NameToLayer("Player"));
		}
		else
		{
			//! rotate towards the target
			Vector3 targetDirection = enemyBase.mTargetPlayer.transform.position - enemyBase.transform.position;
			float targetAngle = GetAngleHelper.GetAngle(targetDirection,enemyBase.transform.forward,enemyBase.transform.up);
			if(Mathf.Abs(targetAngle) > 3.0f)
			{
				//enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(targetDirection), enemyBase.mSteeringForce * Time.deltaTime);
				enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
			}
		}
		
		return Vector3.zero;
	}
}
