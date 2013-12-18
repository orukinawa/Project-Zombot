using UnityEngine;
using System.Collections;

class ShootProjectileData 
{
	public float mFiringRateTimer;
	public Vector3 targetDir;
	public bool canShoot;
}

public class ShootProjectile : BehaviourBase
{
	public GameObject mProjectilePrefab;
	public float mProjectileSpd = 7;
	public int mProjectileDmg = 10;
	public LayerMask mTargetLayer;
	public float mDelayPerShot = 2;
	
	public AnimationClip ShootAnimationClip;
	public float ShootAnimationSpeed;
		
	public AnimationClip IdleAnimationClip;
	
	public override void Init (EnemyBase enemyBase)
	{
		ShootProjectileData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new ShootProjectileData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (ShootProjectileData)enemyBase.mCustomData[this];
		}
		data.mFiringRateTimer = Random.Range(0.0f,(mDelayPerShot * 0.5f));
		data.canShoot = false;
		data.targetDir = Vector3.zero;
		
		if(ShootAnimationClip != null)
		{
			enemyBase.Animator.IsComplete(ShootAnimationClip,IsCompleteShotAnimation);
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
		data.targetDir = enemyBase.mTargetPlayer.transform.position - enemyBase.transform.position;
		float targetAngle = GetAngleHelper.GetAngle(data.targetDir,enemyBase.transform.forward,enemyBase.transform.up);
		if(Mathf.Abs(targetAngle) > 5.0f)
		{
			enemyBase.transform.Rotate(Vector3.up,targetAngle * enemyBase.mSteeringForce * Time.deltaTime ,Space.Self);
		}
		
		data.mFiringRateTimer += Time.deltaTime;
		
		if(data.mFiringRateTimer >= mDelayPerShot)
		{
			enemyBase.Animator.CrossFade(ShootAnimationClip,WrapMode.Once,ShootAnimationSpeed);
			
			if(!data.canShoot)
			{
				//! instatiate the projectile
				GameObject gameobj = (GameObject)Instantiate(mProjectilePrefab,enemyBase.transform.position,Quaternion.identity);
				gameobj.GetComponent<TempProjectile>().InitProjectile(mProjectileDmg,mProjectileSpd,data.targetDir,1.5f);
				data.canShoot = true;
			}
		}
		else
		{
			enemyBase.Animator.CrossFade(IdleAnimationClip,WrapMode.Loop);
		}
		
		return Vector3.zero;
	}
		
	bool IsCompleteShotAnimation(EnemyBase enemyBase)
	{
		ShootProjectileData data = (ShootProjectileData)enemyBase.mCustomData[this];
		data.mFiringRateTimer = 0.0f;
		data.canShoot = false;
		return true;
	}
}
