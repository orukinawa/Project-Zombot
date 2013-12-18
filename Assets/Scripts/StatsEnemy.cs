using UnityEngine;
using System.Collections;

public class StatsEnemy : StatsBase
{
	//ModelColorManager colorManager = null;
	EnemyBase enemyBase;
	bool decalOnHit = false;
	
	void Start ()
	{
		Debug.Log("Start called on " + gameObject.name);
		enemyBase = GetComponent<EnemyBase>();
		initializeStats();
	}
	
	public override void initializeStats ()
	{
		Debug.Log("initialize called on "+ gameObject.name);
		currentHealth = maxHealth;
		enemyBase.mCurrSpeed = enemyBase.mMaxSpeed;
	}
	
	IEnumerator HitDecal(float duration)
	{
		if(!decalOnHit)
		{
			decalOnHit = true;
			foreach(Renderer render in enemyBase.mMeshRenderers)
			{
				Debug.Log("Hit me");
				render.material.color = Color.red;
			}
		}
		yield return new WaitForSeconds(duration);	
		
		foreach(Renderer render in enemyBase.mMeshRenderers)
		{
			render.material.color = Color.white;
		}
		
		decalOnHit = false;
	}
	
	public override void ApplyDamage (float damage, GameObject player = null)
	{
		currentHealth += damage;
		
		if(currentHealth <= 0)
		{
			currentHealth = 0.0f;
		}
		else
		{
			if(damage < 0)
			{
				//Debug.Log("Hit me");
				// play the hit decal
				StartCoroutine(HitDecal(0.1f));	
			}
		}
		
		if(currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}
	
	public override void SelfDestruct ()
	{
//		EnemyBase enemyBase = GetComponent<EnemyBase>();
		
		//! instantiate an animation for death
		//enemyBase.InstantiateDeathAni();
		//! adjust all necessary data when about to be despawn
		enemyBase.ActivateDeath();
		// reset the animation
		enemyBase.Animator.Reset();
		// reset the stat
		initializeStats();
		//! deSpawn to the pool
		PoolManager.pools["Enemy Pool"].DeSpawn(gameObject);
		//! adjust talent point per enemy kill
		EventManager.ConfigureTalentPoint(EventManager.sKillPts);
		
		//! reset the stat
		//base.initializeStats();
	}
	
	public override void ApplySlow (float multiplier)
	{
		enemyBase.mCurrSpeed *= multiplier;
	}
	
	public override void RestoreMoveSpeed ()
	{
		enemyBase.mCurrSpeed = enemyBase.mMaxSpeed;
	}
	
	void Update()
	{
		// check if object falls down the valley of death!
		if(transform.position.y < -50.0f && gameObject.activeSelf)
		{
			SelfDestruct();
		}
	}
}
