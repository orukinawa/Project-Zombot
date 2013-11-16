//using UnityEngine;
//using System.Collections;
//
////Rifle Gun
//public class Weapon1 : RangeWeaponBase
//{
//	public GameObject bullet;
//	
//	public override void Start()
//	{
//		base.Start();
//	}
//	
//	public override void Update()
//	{
//		base.Update();
//	}
//	
//	public override void PrimaryFire()
//	{
//		//base.PrimaryFire();
//		if(canShoot)
//		{
//			if(fireRateTimer >= fireRate)
//			{
//				--currentBullets;
//				Debug.Log(weaponName + ": SHOOTING! BulletsLeft: " + currentBullets);
//				fireRateTimer = 0.0f;		
//				//Instantiate(bullet,transform.position,transform.rotation);
//				
//				Instantiate(bullet,transform.position + Vector3.left,transform.rotation);
//				Instantiate(bullet,transform.position + Vector3.right,transform.rotation);
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
//		base.Reload();
//	}
//}
