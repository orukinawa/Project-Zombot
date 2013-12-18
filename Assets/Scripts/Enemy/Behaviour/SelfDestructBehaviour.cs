using UnityEngine;
using System.Collections;

public class SelfDestructBehaviourData
{
	public float mKamikazeRoarTimer;
	public float mBlinkingTimer;
	public float mBlinkingDuration;
	public bool mReadyToSelfDestruct;
	public float mSelfDestructTimer;
	
	public Renderer[] mMeshRenderers;
	
	// with speed boost
	public float mCurrSpeed;
}

public class SelfDestructBehaviour : BehaviourBase
{
	public enum TYPE
	{
		//! won't self destruct upon death
		NON_VOLATILE,
		//! self destructs upon death
		VOLATILE,
	}
	
	//! the delay before setting the count down self destruct
	public float mKamikazeRoarDuration;
	public float mSelfDestructDelay;
	public GameObject mExplosionPrefab;
	public LayerMask mTargetLayer;
	public int mExplodeDmg;
	//! make it high
	public int mKnockBackForce;
	public TYPE mType;
	
	public Color DecalDestructTimerColor;
	
	// when about to self destruct
	public float SpeedMultiplier;
	
	public AnimationClip RoarAnimationClip;
	public AnimationClip WalkAnimationClip;
	public float WalkAnimationSpeed;
	
	public override void Init (EnemyBase enemyBase)
	{
		SelfDestructBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new SelfDestructBehaviourData();
			enemyBase.mCustomData[this] = data;
			data.mMeshRenderers = enemyBase.GetComponentsInChildren<Renderer>();
			
		}
		else
		{
			data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
		}
		data.mCurrSpeed = enemyBase.mMaxSpeed * SpeedMultiplier;
		data.mSelfDestructTimer = 0.0f;
		data.mKamikazeRoarTimer = 0.0f;
		data.mBlinkingTimer = 0.0f;
		data.mReadyToSelfDestruct = false;
		data.mBlinkingDuration = 0.5f;
		
	}
	
	public void SelfDestruct(EnemyBase enemyBase) 
	{
		GameObject explosion = (GameObject)Instantiate(mExplosionPrefab, enemyBase.transform.position, mExplosionPrefab.transform.rotation);
		Vector3 explodePos = explosion.transform.position;
		float radius = explosion.collider.bounds.extents.x;
		Collider[] colliders = Physics.OverlapSphere(explodePos,radius,mTargetLayer);
		
		foreach(Collider col in colliders)
		{
			//! the nearer the explosion the better
//			Vector3 dir = explodePos - col.transform.position;
//			float dmgPercent = dir.magnitude / radius;
			col.GetComponent<StatsCharacter>().currentHealth -= mExplodeDmg;
			//! knockback of the explosion
			//col.GetComponent<CharacterController>().SimpleMove(-dir * mKnockBackForce);
		}
		
		SelfDestructBehaviourData data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
		
		// reset the mesh renderers to white
		foreach(Renderer render in data.mMeshRenderers)
		{
			render.material.color = Color.white;
		}
		
		enemyBase.mStatEnemy.SelfDestruct();
	}
	
	public override void DeInit (EnemyBase enemyBase)
	{
		SelfDestructBehaviourData data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
		// if he dies by shot and not self destruct
		foreach(Renderer render in data.mMeshRenderers)
		{
			render.material.color = Color.white;
		}
	}
	
	public void BlinkingEffect(EnemyBase enemyBase, SelfDestructBehaviourData data)
	{
		data.mBlinkingTimer += Time.deltaTime;
		//Renderer[] renderers = enemyBase.GetComponentsInChildren<Renderer>();
		foreach(Renderer render in data.mMeshRenderers)
		{
			render.material.color = Color.white;
		}
		
		if(data.mBlinkingTimer >= data.mBlinkingDuration)
		{
			data.mBlinkingTimer = 0.0f;
			
			float currTimerRatio = (mSelfDestructDelay - data.mSelfDestructTimer) / mSelfDestructDelay;
			
			if(currTimerRatio < 0.3f)
			{
				data.mBlinkingDuration = 0.1f;
			}
			
			foreach(Renderer render in data.mMeshRenderers)
			{
				render.material.color = DecalDestructTimerColor;
			}
		}
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		SelfDestructBehaviourData data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
		Vector3 targetDirection = Vector3.zero;
		
		if(enemyBase.mTargetPlayer == null)
		{
			SearchForNewTarget(enemyBase,enemyBase.mAttackRadius,mTargetLayer);
		}
		else
		{
			targetDirection = enemyBase.mTargetPlayer.transform.position - enemyBase.transform.position;
			float targetAngle = GetAngleHelper.GetAngle(targetDirection,enemyBase.transform.forward,enemyBase.transform.up);
			if(Mathf.Abs(targetAngle) > 3.0f)
			{
				//Debug.Log("rotating");
				enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime, Space.Self);
				//enemyBase.transform.rotation = Quaternion.Slerp(enemyBase.transform.rotation, Quaternion.LookRotation(targetDirection), enemyBase.mSteeringForce * Time.deltaTime);
			}
		}
		
		data.mKamikazeRoarTimer += Time.deltaTime;
		if(data.mKamikazeRoarTimer >= mKamikazeRoarDuration)
		{
			data.mReadyToSelfDestruct = true;
			BlinkingEffect(enemyBase,data);
			data.mSelfDestructTimer += Time.deltaTime;
			enemyBase.mCurrSpeed = data.mCurrSpeed;
			if(data.mSelfDestructTimer > mSelfDestructDelay)
			{
				//! booms away
				SelfDestruct(enemyBase);
			}
			enemyBase.Animator.CrossFade(WalkAnimationClip,WrapMode.Loop,WalkAnimationSpeed);
			
			return targetDirection;
		}
		else
		{
			//! perform roar kamikaze animation here
			enemyBase.Animator.CrossFade(RoarAnimationClip,WrapMode.Once);
		}
		
		return Vector3.zero;
	}
}
