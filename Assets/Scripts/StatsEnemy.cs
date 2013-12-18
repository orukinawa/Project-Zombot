using UnityEngine;
using System.Collections;

public class StatsEnemy : StatsBase
{
	//ModelColorManager colorManager = null;
	EnemyBase enemyBase;
	
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
	
	public override void SelfDestruct ()
	{
		EnemyBase enemyBase = GetComponent<EnemyBase>();
		//! adjust all necessary data when die
		enemyBase.ActivateDeath();
		//! instantiate an animation for death
		enemyBase.InstantiateDeathAni();
		initializeStats();
		//! deSpawn to the pool
		PoolManager.pools["Enemy Pool"].DeSpawn(gameObject);
		//! adjust talent point per enemy kill
		EventManager.ConfigureTalentPoint(EventManager.sKillPts);
		
		//! reset the stat
		base.initializeStats();
	}
	
	public override void ApplySlow (float multiplier)
	{
		enemyBase.mCurrSpeed *= multiplier;
	}
	
	public override void RestoreMoveSpeed ()
	{
		enemyBase.mCurrSpeed = enemyBase.mMaxSpeed;
	}
}
