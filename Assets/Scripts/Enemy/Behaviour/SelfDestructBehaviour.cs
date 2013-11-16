using UnityEngine;
using System.Collections;

public class SelfDestructBehaviourData
{
	public float mKamikazeRoarTimer;
	public float mBlinkingTimer;
	public float mBlinkingDuration;
	public bool mReadyToSelfDestruct;
	public float mSelfDestructTimer;
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
	
	public override void Init (EnemyBase enemyBase)
	{
		SelfDestructBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new SelfDestructBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
		}
		data.mSelfDestructTimer = 0.0f;
		data.mKamikazeRoarTimer = 0.0f;
		data.mBlinkingTimer = 0.0f;
		data.mReadyToSelfDestruct = false;
		data.mBlinkingDuration = 0.5f;
	}
	
	public void SelfDestruct(EnemyBase enemyBase) 
	{
		GameObject explosion = (GameObject)Instantiate(mExplosionPrefab, enemyBase.transform.position, Quaternion.identity);
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
		
		Destroy(enemyBase.gameObject);
		
//		//! kill him and sent to the pool
//		enemyBase.GetComponent<StatsEnemy>().ApplyDamage(-9000);
//		enemyBase.GetComponent<StatsEnemy>().ApplySlow(0);
//		Debug.Log("yolo");
	}
	
	public override void Death (EnemyBase enemyBase)
	{
//		SelfDestructBehaviourData data = (SelfDestructBehaviourData)enemyBase.mCustomData[this];
//		data.mSelfDestructTimer = 0.0f;
//		data.mKamikazeRoarTimer = 0.0f;
//		data.mBlinkingTimer = 0.0f;
//		data.mReadyToSelfDestruct = false;
//		data.mBlinkingDuration = 0.5f;
//		enemyBase.GetComponent<StatsEnemy>().RestoreMoveSpeed();
	}
	
	public void BlinkingEffect(EnemyBase enemyBase, SelfDestructBehaviourData data)
	{
		data.mBlinkingTimer += Time.deltaTime;
		Renderer[] renderers = enemyBase.GetComponentsInChildren<Renderer>();
		foreach(Renderer render in renderers)
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
			
			foreach(Renderer render in renderers)
			{
				render.material.color = Color.red;
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
			if(data.mSelfDestructTimer > mSelfDestructDelay)
			{
				//! booms away
				SelfDestruct(enemyBase);
			}
			
			return targetDirection;
		}
		else
		{
			//! perform roar kamikaze animation here
		}
		
		return Vector3.zero;
	}
}
