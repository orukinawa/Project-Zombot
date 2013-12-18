using UnityEngine;
using System.Collections;

public class MeleeBehaviourData
{
	public float mAttackTimer;
	public bool mAttacked = false;
}

////! it must be below of the other class, apparent it reads require component above the class only
////! BUGGY DON USE, causes the inspector to BUG!
//[RequireComponent(typeof(SeekBehaviour))]

public class MeleeBehaviour : BehaviourBase
{
	public float mAttackAngle;
	public float mDistanceAtk;
	public float mAtkDelay;
	// interpolater 1 to 0
	public float mInitAttackDelay = 0.5f;
	
	public GameObject mHitEffect;
	
	public AnimationClip MeleeAnimationClip;
	public AnimationClip IdleAnimationClip;
	
	public float MeleeAnimationSpeed;
	public float MeleeDamage = 10;
	
	public override void Init (EnemyBase enemyBase)
	{
		MeleeBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new MeleeBehaviourData();
			enemyBase.mCustomData[this] = data;
			
		}
		else
		{
			data =(MeleeBehaviourData)enemyBase.mCustomData[this];
		}
		
		data.mAttacked = false;
		data.mAttackTimer = mAtkDelay * mInitAttackDelay;
		enemyBase.Animator.IsComplete(MeleeAnimationClip,IsCompleteMelee);
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		MeleeBehaviourData data = (MeleeBehaviourData)enemyBase.mCustomData[this];
		Vector3 pos = enemyBase.transform.position;
		Vector3 dir = enemyBase.transform.forward;
		GameObject player = enemyBase.mTargetPlayer;
		
		if(player == null)
		{
			Debug.LogWarning("Melee behaviour didn't get the player's refence");
			return Vector3.zero;
		}
		
		Vector3 targetDir = player.transform.position - pos;
		float angle = Vector3.Angle(dir, targetDir);
		
		float targetAngle = GetAngleHelper.GetAngle(targetDir,dir,enemyBase.transform.up);
		//! manual rotate
		if(Mathf.Abs(targetAngle) > 3.0f)
		{
			//Debug.Log("rotating");
			//enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
			targetDir.y = 0.0f;
			enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(targetDir), enemyBase.mSteeringForce * Time.deltaTime);
		}
		
		data.mAttackTimer += Time.deltaTime;
		
		if(data.mAttackTimer > mAtkDelay)
		{
			if(!data.mAttacked)
			{
				if(angle < mAttackAngle)
				{
					if(targetDir.sqrMagnitude < mDistanceAtk * mDistanceAtk)
					{
						//! do attack animation here
						enemyBase.Animator.CrossFade(MeleeAnimationClip,WrapMode.Once,MeleeAnimationSpeed);
						if(mHitEffect != null)
						{
							Instantiate(mHitEffect,player.transform.position,Quaternion.identity);
						}
						player.GetComponent<StatsCharacter>().currentHealth -= MeleeDamage;
						data.mAttacked = true;
					}
				}
			}
		}
		else
		{
			enemyBase.Animator.CrossFade(IdleAnimationClip,WrapMode.Loop);
		}
		
		return Vector3.zero;
	}
		
	bool IsCompleteMelee(EnemyBase enemyBase)	
	{
		MeleeBehaviourData data = (MeleeBehaviourData)enemyBase.mCustomData[this];
			
		data.mAttackTimer = 0.0f;
		data.mAttacked = false;
		Debug.LogWarning("[MELEE] CALLBACK!!!!");
		
		return true;
	}
}
