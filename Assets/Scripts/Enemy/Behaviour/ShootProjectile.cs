using UnityEngine;
using System.Collections;

class ShootProjectileData 
{
	public float mFiringRateTimer;
}

public class ShootProjectile : BehaviourBase
{
	public GameObject mProjectilePrefab;
	public float mProjectileSpd = 7;
	public int mProjectileDmg = 10;
	public LayerMask mTargetLayer;
	public float mDelayPerShot = 2;
	
	public override void Init (EnemyBase enemyBase)
	{
		ShootProjectileData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new ShootProjectileData();
			data.mFiringRateTimer = 0.0f;
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (ShootProjectileData)enemyBase.mCustomData[this];
		}
	}
	
	//! this updates every frame
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		ShootProjectileData data = (ShootProjectileData)enemyBase.mCustomData[this];
		
		if(enemyBase.mTargetPlayer == null)
		{
			SearchForNewTarget(enemyBase,enemyBase.mAttackRadius,mTargetLayer);
		}
		
		if(enemyBase.mTargetPlayer == null)return Vector3.zero;
		//! make enemy face the player based on its forward axis
		Vector3 targetDirection = enemyBase.mTargetPlayer.transform.position - enemyBase.transform.position;
		float targetAngle = GetAngleHelper.GetAngle(targetDirection,enemyBase.transform.forward,enemyBase.transform.up);
		if(Mathf.Abs(targetAngle) > 5.0f)
		{
			enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime ,Space.Self);
		}
		
		data.mFiringRateTimer += Time.deltaTime;
		
		if(data.mFiringRateTimer >= mDelayPerShot)
		{
			//! instatiate the projectile
			GameObject gameobj = (GameObject)Instantiate(mProjectilePrefab,enemyBase.transform.position,Quaternion.identity);
			gameobj.GetComponent<TempProjectile>().InitProjectile(mProjectileDmg,mProjectileSpd,targetDirection,3.0f);
			//Debug.Log("READY TO FIRE!");
			data.mFiringRateTimer = 0.0f;
		}
		
		return Vector3.zero;
	}
}
