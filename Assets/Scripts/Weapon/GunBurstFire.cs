using UnityEngine;
using System.Collections;

public class GunBurstFire : GunBase
{
	public int shotsToFire;
	public float burstShortInterval;
	private float burstShortIntervalTimer;
	private int shotsFired;
	private bool isShooting = false;
	
	void Start()
	{
		Initialize();
	}
	
	void Update()
	{
		GunUpdate();
	}
	
	public override void Initialize ()
	{
		base.Initialize ();
		burstShortIntervalTimer = burstShortInterval*2;
		shotsFired = 0;
		isShooting = false;
	}
	
	public override void GunUpdate ()
	{
		if(!isReloading)
		{
			if(!canShoot)
			{
				if(fireRateTimer < fireRate)
				{
					fireRateTimer += Time.deltaTime;
				}
				else
				{
					canShoot = true;
				}
			}
			
			if(currMagazineSize <= 0)
			{
				isReloading = true;
				canShoot = false;
			}
		}
		else
		{
			reloadRateTimer += Time.deltaTime;
			if(reloadRateTimer > reloadRate)
			{
				fireRateTimer = 0.0f;
				currMagazineSize = maxMagazineSize;
				reloadRateTimer = 0.0f;
				isReloading = false;
				canShoot = true;
			}
		}
		
		if(isShooting)
		{
			if(shotsFired < shotsToFire)
			{
				if(burstShortIntervalTimer > burstShortInterval)
				{
					if(usePoolManager)
					{
						bullet = PoolManager.pools["Bullet Pool"].Spawn(bulletMod,bulletSpawnNode.position,transform.rotation) as GameObject;
					}
					else
					{
						bullet = Instantiate(bulletMod,transform.position,transform.rotation) as GameObject;
					}								
					if(bullet != null)
					{
						Debug.Log(bullet.name + " fired!");
						bullet.GetComponent<BulletBase>().InitializeBullet(bulletSpeed,bulletRange,bulletDamage,currentEffect,mStat);
						
						// TODO: add logic to tell bullet if pool manager was used
						bullet = null;
						++shotsFired;
						burstShortIntervalTimer = 0.0f;
					}
					else
					{
						Debug.LogError("Bullet is null!");
					}
				}
				else
				{
					burstShortIntervalTimer += Time.deltaTime;
				}
			}
			else
			{
				shotsFired = 0;
				burstShortIntervalTimer = burstShortInterval*2;
				--currMagazineSize;
				fireRateTimer = 0.0f;
				canShoot = false;
				isShooting = false;
			}
		}
	}
	
	public override void Shoot ()
	{
		if(canShoot)
		{
			isShooting = true;
		}
	}
}
