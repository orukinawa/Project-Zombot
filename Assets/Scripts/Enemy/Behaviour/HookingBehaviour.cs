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
	float mHookMaxDistSqr;
	float mHookMinDistSqr;
	
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
		//! cache the min and max distance
		//mHookMaxDistSqr = mHookMaxDist * mHookMaxDist;
		mHookMaxDistSqr = enemyBase.mAttackRadius * enemyBase.mAttackRadius;
		mHookMinDistSqr = mHookMinDist * mHookMinDist;
	}
	
	void ApplyHookEffect(GameObject hookTarget)
	{
		StatsCharacter statCh = hookTarget.GetComponent<StatsCharacter>();
		if(!statCh)return;
		statCh.currentHealth -= 10;
		statCh.ApplySlow(0);
	}
	
	public void ReleaseTarget(EnemyBase enemyBase)
	{
		//! TODO remove parent and child relationship with the projectile and the player in case of death
		HookingBehaviourData data = (HookingBehaviourData)enemyBase.mCustomData[this];
		if(data.mHookTarget != null)
		{
			data.mHookTarget.transform.parent = null;
			data.mHookTarget.GetComponent<StatsCharacter>().RestoreMoveSpeed();
		}
		
		Destroy(data.mProjectileRef);
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
		}
		else
		{
			if(data.mProjectileRef == null)
			{
				data.mProjectileRef = (GameObject)Instantiate(mHookProjectile,pos,Quaternion.identity);
			}
			Vector3 hookVector = data.mProjectileRef.transform.position - enemyBase.transform.position;
			
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
					ExecuteTransition(enemyBase);
					Destroy(data.mProjectileRef);
				}
				
				return Vector3.zero;
			}
			else
			{
				data.mProjectileRef.transform.position += (dir * mHookSpeed) * Time.deltaTime;
				
				//! hook checking
				GameObject hookObj = GetNearestToTarget(data.mProjectileRef.transform.position, mHookRadius, mTargetLayer);
				
//				Debug.Log("hookVector.sqrMagnitude: " + hookVector.sqrMagnitude);
//				Debug.Log("mHookMaxDistSqr: " + mHookMaxDistSqr);
				
				if(hookVector.sqrMagnitude >= mHookMaxDistSqr)data.mReverseHook = true;
				if(hookObj == null)return Vector3.zero;
				if(hookObj.layer == LayerMask.NameToLayer("Player"))
				{
					data.mHookTarget = hookObj;
					ApplyHookEffect(hookObj);
					hookObj.transform.parent = data.mProjectileRef.transform;
					data.mReverseHook = true;
				}
				else if(hookObj.layer == LayerMask.NameToLayer("Environment"))
				{
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
