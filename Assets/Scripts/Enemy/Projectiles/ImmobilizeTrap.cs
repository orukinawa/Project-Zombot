using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]

public class ImmobilizeTrap : TrapBase
{
	public float mTrapDuration;
	float mTrapTimer;
	GameObject mTrapPlayer;
	public GameObject mTrapEffect;
	
	protected override void SelfDestruct()
	{
		//! get the dictionary
		//! decrement the trap spawned by the enemy 
		base.SelfDestruct();
		if(mTrapPlayer != null)
		{
			ResetEffects();
		}
		Destroy(this.gameObject);
	}
	
	protected override void ApplyEffects()
	{
		StatsCharacter statChar = mTrapPlayer.GetComponent<StatsCharacter>();
		statChar.ApplySlow(0);
		statChar.currentHealth -= 20;
	}
	
	protected override void ResetEffects()
	{
		StatsCharacter statChar = mTrapPlayer.GetComponent<StatsCharacter>();
		statChar.RestoreMoveSpeed();
	}
	
	//! this trap only tried to get 1 player
	void CheckForTrapPlayer()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position,collider.bounds.extents.x,mTargetLayer);
		
		if(colliders.Length > 0)
		{
			float nearestDist = Mathf.Infinity;
			foreach(Collider col in colliders)
			{
				//! get the nearest to the trap
				float sqrDist = (transform.position - col.transform.position).sqrMagnitude;
				if(sqrDist <= nearestDist)
				{
					nearestDist = sqrDist;
					mTrapPlayer = col.gameObject;
				}
			}
			//Debug.Log("SNAP!");
			//! play UI/effects/animation trap here
			ApplyEffects();
		}
		else
		{
			//! if caught no one
			SelfDestruct();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(mActivate)
		{
			mDelayTimer += Time.deltaTime;
			if(mDelayTimer >= mDelayActiveTime)
			{
				//! do trap
				if(mTrapPlayer == null)
				{
					Instantiate(mTrapEffect,transform.position,Quaternion.identity);
					CheckForTrapPlayer();
				}
				//! if caught someone
				mTrapTimer += Time.deltaTime;
				if(mTrapTimer >= mTrapDuration)
				{
					SelfDestruct();
				}
			}
		}
	}
}
