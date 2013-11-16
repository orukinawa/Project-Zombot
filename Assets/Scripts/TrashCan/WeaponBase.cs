//using UnityEngine;
//using System.Collections;
//
//public class WeaponBase : MonoBehaviour 
//{
//	protected bool canShoot = true;
//	protected bool isReloading = false;
//	// Time between shots
//	public float fireRate = 0.0f;
//	protected float fireRateTimer = 0.0f;
//	
//	// Amount of bullets in a Magazine
//	public int magazineSize = 0;
//	protected int currentBullets = 0;
//	
//	// Time taken to reload weapon
//	public float reloadRate = 0.0f;
//	protected float reloadRateTimer = 0.0f;
//	
//	// Bullet Prefab
//	public GameObject bulletPrefab;
//	protected GameObject bullet = null;
//	
//	// Bullet attributes
//	public int bulletDamage;
//	public float bulletRange;
//	public float bulletSpeed;
//	
//	public virtual void Initialize()
//	{
//		currentBullets = magazineSize;
//	}
//	
//	public virtual void _Update()
//	{
//		if(!isReloading)
//		{
//			if(!canShoot)
//			{
//				if(fireRateTimer < fireRate)
//				{
//					fireRateTimer += Time.deltaTime;
//				}
//				else
//				{
//					canShoot = true;
//				}
//			}
//			
//			if(currentBullets <= 0)
//			{
//				isReloading = true;
//				canShoot = false;
//			}
//		}
//		else
//		{
//			reloadRateTimer += Time.deltaTime;
//			if(reloadRateTimer > reloadRate)
//			{
//				fireRateTimer = 0.0f;
//				currentBullets = magazineSize;
//				reloadRateTimer = 0.0f;
//				isReloading = false;
//				canShoot = true;
//			}
//		}
//	}	
//	
//	public virtual void PrimaryFire()
//	{}
//
//	public virtual void SecondaryFire()
//	{}
//
//	public virtual void Reload()
//	{}
//	
//	protected void UpdateFireRate()
//	{
//		if(!canShoot)
//		{
//			if(fireRateTimer < fireRate)
//			{
//				fireRateTimer += Time.deltaTime;
//			}
//			else
//			{
//				canShoot = true;
//			}
//		}
//	}
//	
//	protected void AfterShooting()
//	{
//		fireRateTimer = 0.0f;
//		canShoot = false;
//	}
//}
