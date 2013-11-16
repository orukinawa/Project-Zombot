//using UnityEngine;
//using System.Collections;
//
//public class RangeWeaponBase : WeaponBase 
//{
//	// Attributes
//	protected bool canShoot = true;
//	
//	// Amount of bullets in a Magazine
//	public int magazineSize = 0;
//	protected int currentBullets = 0;
//	
//	// Time between shooting each bullet
//	public float fireRate = 0.0f;
//	protected float fireRateTimer = 0.0f;
//	
//	// Time taken to reload weapon
//	public float reloadRate = 0.0f;
//	protected float reloadRateTimer = 0.0f;
//	
//	// Debug
//	protected bool displayReloadingLog = true;
//	protected string weaponName = "";
//	
//	public virtual void Start()
//	{
//		weaponName = gameObject.name;
//		currentBullets = magazineSize;
//	}
//	
//	public virtual void Update()
//	{
//		if(currentBullets <= 0)
//		{
//			// Debug
//			if(displayReloadingLog)
//			{
//				Debug.Log (weaponName + ": RELOADING!");
//				displayReloadingLog = false;
//			}			
//			
//			canShoot = false;
//			reloadRateTimer += Time.deltaTime;
//			if(reloadRateTimer > reloadRate)
//			{
//				currentBullets = magazineSize;
//				reloadRateTimer = 0.0f;
//				canShoot = true;
//				Debug.Log(weaponName + ": CAN SHOOT!");
//				
//				// Debug
//				displayReloadingLog = true;
//			}
//		}
//		if(fireRateTimer < fireRate)
//		{
//			fireRateTimer += Time.deltaTime;
//		}
//	}
//	
//	public override void PrimaryFire()
//	{
//		if(canShoot)
//		{
//			if(fireRateTimer >= fireRate)
//			{
//				--currentBullets;
//				Debug.Log(weaponName + ": SHOOTING! BulletsLeft: " + currentBullets);
//				fireRateTimer = 0.0f;				
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
//		if(currentBullets != magazineSize || reloadRateTimer > 0.0f)
//		{
//			currentBullets = 0;
//		}
//	}
//}
