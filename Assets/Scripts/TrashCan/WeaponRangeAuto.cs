//using UnityEngine;
//using System.Collections;
//
//public class WeaponRangeAuto : WeaponBase 
//{
//	public virtual void Start()
//	{
//		base.Initialize();
//	}
//	
//	public virtual void Update()
//	{
//		base._Update();
//	}
//	
//	public override void PrimaryFire()
//	{
//		if(canShoot)
//		{
//			//bullet = Instantiate(bulletPrefab,transform.position,transform.rotation) as GameObject;
//			bullet = PoolManager.pools["Pool A"].Spawn(bulletPrefab,transform.position,transform.rotation) as GameObject;
//			bullet.GetComponent<BulletBase>().InitializeBullet(bulletDamage,bulletSpeed,bulletRange);
//			if(bullet != null)
//			{
//				Debug.Log(bullet.name + " fired!");
//				//bullet.GetComponent<BulletBase>().initializeBullet(bulletDamage,bulletSpeed,bulletRange);
//				bullet = null;
//				--currentBullets;
//				base.AfterShooting();
//			}
//			else
//			{
//				Debug.LogError("Bullet is null!");
//			}
//		}
//	}
//	
//	public override void SecondaryFire()
//	{
//		
//	}
//
//	public override void Reload()
//	{
//		if(currentBullets != magazineSize && !isReloading)
//		{
//			currentBullets = 0;
//		}
//	}
//	
//	//! Resets the reloadRateTimer to zero if the player was reloading when he switched out the weapon
//	void OnEnable()
//	{
//		if(isReloading)
//		{
//			reloadRateTimer = 0.0f;
//		}
//	}
//}
