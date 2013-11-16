using UnityEngine;
using System.Collections;

public class TrapBase : MonoBehaviour {
	
	//! the delay before the trap activates
	public float mDelayActiveTime;
	protected float mDelayTimer;
	public int mDamage;
	public LayerMask mTargetLayer;
	
	protected bool mActivate;
	//! keeps trap of the object that spawn this instance
	protected EnemyBase mEnemyBase;
	//! cache the key require to access the enemyBase dictionary values
	protected PlantTraps mKeyBehaviour;
	
	public void GetTrapOwner(EnemyBase enemyBase, PlantTraps key)
	{
		mEnemyBase = enemyBase;
		mKeyBehaviour = key;
	}
	
	protected virtual void SelfDestruct()
	{
		if(mEnemyBase != null)
		{
			PlantTrapsData data = (PlantTrapsData)mEnemyBase.mCustomData[mKeyBehaviour];
			data.mNumTrapSpawned -= 1;
		}
	}
	
	protected virtual void ApplyEffects()
	{
		
	}
	
	protected virtual void ResetEffects()
	{
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(!mActivate)
		{
			if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				mActivate = true;
			}
		}
	}
}
